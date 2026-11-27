using System;
using System.Collections.Generic;
using g3;

namespace MeshCutter.Core;

/// <summary>
/// Remplit le trou plan laissé par une coupe (copie adaptée de g3.PlanarHoleFiller).
///
/// Principe : les contours du trou sont mis à plat en 2D, insérés dans un rectangle maillé,
/// puis les triangles hors du contour sont supprimés et le remplissage est recollé à la pièce.
///
/// Deux adaptations par rapport à la bibliothèque :
/// - l'insertion d'un contour est essayée avec plusieurs points de départ, car elle échoue
///   parfois selon le sommet par lequel on commence (micro-pas dans le contour) ;
/// - si l'insertion laisse des sommets en trop alignés sur un segment du contour, l'arête
///   correspondante de la pièce est coupée pour que les deux côtés aient les mêmes sommets.
/// </summary>
public class PlanarHoleFillerCopy
{
    const int MaxAttempts = 6;

    public DMesh3 Mesh;
    public Vector3d PlaneOrigin;
    public Vector3d PlaneNormal;

    public bool OutputHasCracks = false;
    public int FailedInsertions = 0;

    Vector3d PlaneX, PlaneY;

    class FillLoop
    {
        public EdgeLoop edgeLoop;
        public Polygon2d poly = new Polygon2d();

        public FillLoop(EdgeLoop loop)
        {
            edgeLoop = loop;
        }
    }
    List<FillLoop> Loops = new List<FillLoop>();
    AxisAlignedBox2d Bounds;

    public PlanarHoleFillerCopy(MeshPlaneCut cut)
    {
        Mesh = cut.Mesh;
        foreach (EdgeLoop loop in cut.CutLoops)
            Loops.Add(new FillLoop(loop));
        PlaneOrigin = cut.PlaneOrigin;
        PlaneNormal = cut.PlaneNormal;
        Vector3d.ComputeOrthogonalComplement(1, PlaneNormal, ref PlaneX, ref PlaneY);
    }

    /// <summary>
    /// Calcule le remplissage et l'ajoute à la pièce.
    /// Renvoie false si une insertion ou un recollement a échoué.
    /// </summary>
    public bool Fill()
    {
        FailedInsertions = 0;
        OutputHasCracks = false;

        compute_polygons();

        // on travaille dans une boîte unité, plus précis
        Vector2d shiftOrigin = Bounds.Center;
        double scale = 1.0 / Bounds.MaxDim;
        foreach (var floop in Loops) {
            floop.poly.Translate(-shiftOrigin);
            floop.poly.Scale(scale * Vector2d.One, Vector2d.Zero);
        }

        var elemToLoop = new Dictionary<PlanarComplex.Element, int>();
        var complex = new PlanarComplex();
        for (int i = 0; i < Loops.Count; ++i)
            elemToLoop[complex.Add(Loops[i].poly)] = i;

        // un "solide" 2D = un contour extérieur + ses trous
        var solids = complex.FindSolidRegions(PlanarComplex.FindSolidsOptions.SortPolygons);

        for (int fi = 0; fi < solids.Polygons.Count; ++fi) {
            var gpoly = solids.Polygons[fi];
            var gsolid = solids.PolygonsSources[fi];

            var polys = new List<Polygon2d>() { gpoly.Outer };
            polys.AddRange(gpoly.Holes);

            // insertion des contours dans un rectangle, avec plusieurs points de départ possibles
            var rect = new TrivialRectGenerator() { IndicesMap = new Index2i(1, 2), Width = 1.5f, Height = 1.5f };
            DMesh3 fillMesh = new DMesh3();
            int[][] polyVertices = Array.Empty<int[]>();     // sommets insérés pour chaque contour, dans l'ordre d'origine
            List<List<int>>[] extras = Array.Empty<List<List<int>>>();   // séries [A, V1..Vn, B] de sommets en trop
            bool inserted = false;

            for (int attempt = 0; attempt < MaxAttempts && !inserted; attempt++) {
                fillMesh = rect.Generate().MakeDMesh();
                fillMesh.ReverseOrientation();
                polyVertices = new int[polys.Count][];
                extras = new List<List<int>>[polys.Count];
                inserted = true;

                for (int pi = 0; pi < polys.Count && inserted; ++pi) {
                    int shift = attempt * polys[pi].VertexCount / MaxAttempts;
                    inserted = TryInsert(fillMesh, polys[pi], shift, scale, out polyVertices[pi], out extras[pi]);
                }
            }
            if (!inserted) {
                FailedInsertions++;
                continue;
            }

            // supprime les triangles hors du contour
            var outside = new List<int>();
            foreach (int tid in fillMesh.TriangleIndices()) {
                if (!gpoly.Contains(fillMesh.GetTriCentroid(tid).xy))
                    outside.Add(tid);
            }
            foreach (int tid in outside)
                fillMesh.RemoveTriangle(tid, true, false);

            // retour en 3D
            MeshTransforms.PerVertexTransform(fillMesh, (v) => to3D(v.xy / scale + shiftOrigin));

            // recollement : sommets du remplissage <-> sommets de la pièce
            var mergeMap = new IndexMap(true);
            for (int pi = 0; pi < polys.Count; ++pi) {
                var sourceElem = (pi == 0) ? gsolid.Outer : gsolid.Holes[pi - 1];
                int[] sourceLoop = Loops[elemToLoop[sourceElem]].edgeLoop.Vertices;

                if (!BuildMergeMap(fillMesh, polyVertices[pi], sourceLoop, mergeMap)) {
                    FailedInsertions++;
                    OutputHasCracks = true;
                    continue;
                }

                // sommets en trop : on coupe l'arête de la pièce pour avoir les mêmes sommets des deux côtés
                foreach (List<int> run in extras[pi]) {
                    int end = mergeMap[run[run.Count - 1]];
                    int current = mergeMap[run[0]];
                    for (int i = 1; i < run.Count - 1; i++) {
                        if (Mesh.SplitEdge(current, end, out DMesh3.EdgeSplitInfo split) != MeshResult.Ok) {
                            FailedInsertions++;
                            OutputHasCracks = true;
                            break;
                        }
                        Mesh.SetVertex(split.vNew, fillMesh.GetVertex(run[i]));
                        mergeMap[run[i]] = split.vNew;
                        current = split.vNew;
                    }
                }
            }

            // ajoute le remplissage à la pièce
            MeshEditor editor = new MeshEditor(Mesh);
            int[] mapV;
            editor.AppendMesh(fillMesh, mergeMap, out mapV, Mesh.AllocateTriangleGroup());
        }

        return FailedInsertions == 0;
    }

    /// <summary>
    /// Insère un contour dans le maillage de remplissage, en commençant au sommet 'shift'.
    /// Renvoie true si la boucle obtenue passe par tous les sommets du contour. Les éventuels
    /// sommets en trop sont rendus dans 'runs' (séries [A, V1..Vn, B], alignées sur le segment A-B).
    /// 'vertices' contient les sommets insérés dans l'ordre d'origine du contour.
    /// </summary>
    bool TryInsert(DMesh3 fillMesh, Polygon2d poly, int shift, double scale,
                   out int[] vertices, out List<List<int>> runs)
    {
        vertices = Array.Empty<int>();
        runs = new List<List<int>>();
        int nv = poly.VertexCount;
        var points = new List<Vector2d>(nv);
        for (int k = 0; k < nv; k++)
            points.Add(poly[(k + shift) % nv]);

        var insert = new MeshInsertUVPolyCurve(fillMesh, new Polygon2d(points));
        if (insert.Validate(MathUtil.ZeroTolerancef * scale) != ValidationStatus.Ok) return false;
        if (!insert.Apply()) return false;
        insert.Simplify();
        if (insert.Loops.Count != 1) return false;

        var curveSet = new HashSet<int>(insert.CurveVertices);
        var loop = new List<int>(insert.Loops[0].Vertices);
        int start = loop.FindIndex(v => curveSet.Contains(v));
        if (start < 0) return false;

        // on parcourt la boucle et on regroupe les sommets en trop entre deux vrais sommets
        var found = new List<List<int>>();
        int extraCount = 0;
        var run = new List<int>();
        for (int t = 0; t <= loop.Count; t++) {
            int vid = loop[(start + t) % loop.Count];
            if (!curveSet.Contains(vid)) {
                run.Add(vid);
                continue;
            }
            if (run.Count > 1) {
                run.Add(vid);
                if (!AreAligned(fillMesh, run)) return false;
                found.Add(run);
                extraCount += run.Count - 2;
            }
            run = new List<int> { vid };
        }
        if (loop.Count - extraCount != nv) return false;

        vertices = new int[nv];
        for (int k = 0; k < nv; k++)
            vertices[(k + shift) % nv] = insert.CurveVertices[k];
        runs = found;
        return true;
    }

    /// <summary>Vrai si tous les sommets intermédiaires de la série sont sur le segment premier-dernier.</summary>
    static bool AreAligned(DMesh3 mesh, List<int> run)
    {
        Vector3d a = mesh.GetVertex(run[0]);
        Vector3d b = mesh.GetVertex(run[run.Count - 1]);
        double abx = b.x - a.x, aby = b.y - a.y;
        double len = Math.Sqrt(abx * abx + aby * aby);
        for (int i = 1; i < run.Count - 1; i++) {
            Vector3d p = mesh.GetVertex(run[i]);
            double dist = Math.Abs(abx * (p.y - a.y) - aby * (p.x - a.x)) / len;
            if (!(dist < 1e-9)) return false;
        }
        return true;
    }

    /// <summary>
    /// Associe chaque sommet du contour inséré à son sommet sur la boucle de la pièce.
    /// Cas simple : même ordre. Sinon : recherche par position (correspondance unique seulement).
    /// </summary>
    bool BuildMergeMap(DMesh3 fillMesh, int[] fillLoop, int[] sourceLoop, IndexMap mergeMap)
    {
        double tol = MathUtil.ZeroTolerancef;

        if (fillLoop.Length == sourceLoop.Length) {
            bool sameOrder = true;
            for (int k = 0; k < fillLoop.Length && sameOrder; k++) {
                sameOrder = fillMesh.IsVertex(fillLoop[k])
                    && fillMesh.GetVertex(fillLoop[k]).Distance(Mesh.GetVertex(sourceLoop[k])) <= tol;
            }
            if (sameOrder) {
                for (int k = 0; k < fillLoop.Length; k++)
                    mergeMap[fillLoop[k]] = sourceLoop[k];
                return true;
            }
        }

        bool allMatched = true;
        foreach (int fv in fillLoop) {
            if (!fillMesh.IsVertex(fv)) {
                allMatched = false;
                continue;
            }
            Vector3d p = fillMesh.GetVertex(fv);
            int match = -1, count = 0;
            foreach (int sv in sourceLoop) {
                if (p.Distance(Mesh.GetVertex(sv)) < tol) {
                    match = sv;
                    count++;
                }
            }
            if (count == 1) mergeMap[fv] = match;
            else allMatched = false;
        }
        return allMatched;
    }

    void compute_polygons()
    {
        Bounds = AxisAlignedBox2d.Empty;
        foreach (FillLoop floop in Loops) {
            Polygon2d poly = new Polygon2d();
            foreach (int vid in floop.edgeLoop.Vertices)
                poly.AppendVertex(to2D(Mesh.GetVertex(vid)));
            floop.poly = poly;
            Bounds.Contain(poly.Bounds);
        }
    }

    Vector2d to2D(Vector3d v)
    {
        Vector3d dv = v - PlaneOrigin;
        dv -= dv.Dot(PlaneNormal) * PlaneNormal;
        return new Vector2d(PlaneX.Dot(dv), PlaneY.Dot(dv));
    }

    Vector3d to3D(Vector2d v)
    {
        return PlaneOrigin + PlaneX * v.x + PlaneY * v.y;
    }
}