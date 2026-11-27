using g3;
using MeshCutter.Core;

namespace MeshCutter.Tests;

/// <summary>
/// Cas particuliers de coupe : des sections qui ont fait échouer le remplissage de la face de coupe
/// (contours avec de très petits pas, sommets alignés...). Chaque cas garde son fichier d'exemple.
/// </summary>
public class MeshSlicerSpecialCasesTests
{
    private static readonly Vector3d Normal = new Vector3d(0, 0, 1);

    // ---------- Fichier "suppanglame 3D" ----------

    private const string SuppanglameFile = "suppanglame 3D.STL";
    private const double SuppanglameCutZ = 20;

    private const double SuppanglameOriginalVolume = 68456.750414615;
    private const double SuppanglameBottomVolume = 34228.387196484;
    private const double SuppanglameTopVolume = 34228.363218131;
    private const double SuppanglameSectionArea = 1574.334010600;
    private const double SuppanglamePieceArea = 11525.33667;

    // ---------- Fichier "Speaker_Body_V2_NO_BOTTOM" ----------

    private const string SpeakerFile = "Speaker_Body_V2_NO_BOTTOM.STL";
    private const double SpeakerCutZ = 20;

    // ---------- Outils ----------

    private static DMesh3 Load(string fileName, bool center)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", fileName);
        var (_, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        Assert.True(mesh.IsClosed());
        // dans l'app, la pièce est centrée à l'import : on reproduit ce placement
        if (center)
            MoveMesh.CenterMesh(mesh);
        return mesh;
    }

    private static (DMesh3 Bottom, DMesh3 Top) CutSuppanglame(double z)
    {
        var (bottom, top, _) = MeshSlicer.Cut(Load(SuppanglameFile, false), new Vector3d(0, 0, z), Normal);
        return (bottom, top);
    }

    private static (DMesh3 Bottom, DMesh3 Top) CutSpeaker(double z)
    {
        var (bottom, top, _) = MeshSlicer.Cut(Load(SpeakerFile, true), new Vector3d(0, 0, z), Normal);
        return (bottom, top);
    }

    private static double SignedAreaZ(DMesh3 mesh, int triangleId)
    {
        var t = mesh.GetTriangle(triangleId);
        var a = mesh.GetVertex(t.a);
        var b = mesh.GetVertex(t.b);
        var c = mesh.GetVertex(t.c);
        return 0.5 * ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x));
    }

    private static List<int> TrianglesInCutPlane(DMesh3 mesh, double z)
    {
        var result = new List<int>();
        foreach (int tid in mesh.TriangleIndices())
        {
            var t = mesh.GetTriangle(tid);
            if (Math.Abs(mesh.GetVertex(t.a).z - z) < 1e-6
                && Math.Abs(mesh.GetVertex(t.b).z - z) < 1e-6
                && Math.Abs(mesh.GetVertex(t.c).z - z) < 1e-6)
            {
                result.Add(tid);
            }
        }
        return result;
    }

    // ---------- suppanglame 3D : coupe à z = 20 ----------

    [Fact]
    public void Cut_Suppanglame_AtZ20_DoesNotThrow()
    {
        var exception = Record.Exception(() => CutSuppanglame(SuppanglameCutZ));

        Assert.Null(exception);
    }

    [Fact]
    public void Cut_Suppanglame_AtZ20_ReturnsClosedPieces()
    {
        var (bottom, top) = CutSuppanglame(SuppanglameCutZ);

        Assert.True(bottom.IsClosed());
        Assert.True(top.IsClosed());
    }

    [Fact]
    public void Cut_Suppanglame_AtZ20_ReturnsTheExpectedBounds()
    {
        var (bottom, top) = CutSuppanglame(SuppanglameCutZ);

        Assert.Equal(0, bottom.GetBounds().Min.z, 1e-3);
        Assert.Equal(20, bottom.GetBounds().Max.z, 1e-3);
        Assert.Equal(20, top.GetBounds().Min.z, 1e-3);
        Assert.Equal(40, top.GetBounds().Max.z, 1e-3);
    }

    [Fact]
    public void Cut_Suppanglame_AtZ20_ReturnsTheExpectedVolumes()
    {
        var (bottom, top) = CutSuppanglame(SuppanglameCutZ);

        var volumeBottom = MeshInfo.GetMeshInfo(bottom).Volume;
        var volumeTop = MeshInfo.GetMeshInfo(top).Volume;
        Assert.NotNull(volumeBottom);
        Assert.NotNull(volumeTop);
        Assert.Equal(SuppanglameBottomVolume, volumeBottom.Value, 0.05);
        Assert.Equal(SuppanglameTopVolume, volumeTop.Value, 0.05);
        Assert.Equal(SuppanglameOriginalVolume, volumeBottom.Value + volumeTop.Value, 0.05);
    }

    [Fact]
    public void Cut_Suppanglame_AtZ20_ReturnsTheExpectedAreas()
    {
        var (bottom, top) = CutSuppanglame(SuppanglameCutZ);

        Assert.Equal(SuppanglamePieceArea, MeshInfo.GetMeshInfo(bottom).Area, 0.05);
        Assert.Equal(SuppanglamePieceArea, MeshInfo.GetMeshInfo(top).Area, 0.05);
    }

    [Fact]
    public void Cut_Suppanglame_AtZ20_CutFaceHasTheAreaOfTheSection()
    {
        var (bottom, top) = CutSuppanglame(SuppanglameCutZ);

        foreach (var mesh in new[] { bottom, top })
        {
            double area = TrianglesInCutPlane(mesh, SuppanglameCutZ).Sum(tid => Math.Abs(SignedAreaZ(mesh, tid)));
            Assert.Equal(SuppanglameSectionArea, area, 0.05);
        }
    }

    [Fact]
    public void Cut_Suppanglame_AtZ20_CutFaceTrianglesAllPointOutwards()
    {
        var (bottom, top) = CutSuppanglame(SuppanglameCutZ);

        var bottomWrong = TrianglesInCutPlane(bottom, SuppanglameCutZ).Where(tid => SignedAreaZ(bottom, tid) <= 0).ToList();
        Assert.Empty(bottomWrong);
        var topWrong = TrianglesInCutPlane(top, SuppanglameCutZ).Where(tid => SignedAreaZ(top, tid) >= 0).ToList();
        Assert.Empty(topWrong);
    }

    [Fact]
    public void Cut_Suppanglame_NearZ20_ReturnsClosedPiecesWhoseVolumesAddUp()
    {
        foreach (double z in new[] { 19.9, 20.1 })
        {
            var (bottom, top) = CutSuppanglame(z);

            Assert.True(bottom.IsClosed());
            Assert.True(top.IsClosed());
            double sum = MeshInfo.GetMeshInfo(bottom).Volume!.Value + MeshInfo.GetMeshInfo(top).Volume!.Value;
            Assert.Equal(SuppanglameOriginalVolume, sum, 0.05);
        }
    }

    // ---------- Speaker body : coupe à z = 20 (fichier centré comme dans l'app) ----------

    [Fact]
    public void Cut_SpeakerBody_AtZ20_DoesNotThrow()
    {
        var exception = Record.Exception(() => CutSpeaker(SpeakerCutZ));

        Assert.Null(exception);
    }

    [Fact]
    public void Cut_SpeakerBody_AtZ20_ReturnsClosedPieces()
    {
        var (bottom, top) = CutSpeaker(SpeakerCutZ);

        Assert.True(bottom.IsClosed());
        Assert.True(top.IsClosed());
    }

    [Fact]
    public void Cut_SpeakerBody_AtZ20_ReturnsTheExpectedBounds()
    {
        var (bottom, top) = CutSpeaker(SpeakerCutZ);

        Assert.Equal(0, bottom.GetBounds().Min.z, 1e-3);
        Assert.Equal(20, bottom.GetBounds().Max.z, 1e-3);
        Assert.Equal(20, top.GetBounds().Min.z, 1e-3);
        Assert.Equal(80, top.GetBounds().Max.z, 1e-3);
    }

    [Fact]
    public void Cut_SpeakerBody_AtZ20_ReturnsPiecesWhoseVolumesAddUpToTheOriginal()
    {
        double originalVolume = MeshInfo.GetMeshInfo(Load(SpeakerFile, true)).Volume!.Value;

        var (bottom, top) = CutSpeaker(SpeakerCutZ);

        double sum = MeshInfo.GetMeshInfo(bottom).Volume!.Value + MeshInfo.GetMeshInfo(top).Volume!.Value;
        Assert.Equal(originalVolume, sum, originalVolume * 1e-6);
    }
}