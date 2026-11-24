using g3;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public class MeshSlicerConcaveTests
{
    private const double CutZ = 10;

    private static DMesh3 LoadL()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "L 65x120x40.STL");
        var (_, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        Assert.True(mesh.IsClosed());
        return mesh;
    }

    private static (DMesh3 Bottom, DMesh3 Top) CutL()
    {
        var mesh = LoadL();
        var (bottom, top, _) = MeshSlicer.Cut(mesh, new Vector3d(0, 0, CutZ), new Vector3d(0, 0, 1));
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

    private static List<int> TrianglesInCutPlane(DMesh3 mesh)
    {
        var result = new List<int>();
        foreach (int tid in mesh.TriangleIndices())
        {
            var t = mesh.GetTriangle(tid);
            if (Math.Abs(mesh.GetVertex(t.a).z - CutZ) < 1e-6
                && Math.Abs(mesh.GetVertex(t.b).z - CutZ) < 1e-6
                && Math.Abs(mesh.GetVertex(t.c).z - CutZ) < 1e-6)
            {
                result.Add(tid);
            }
        }
        return result;
    }

    [Fact]
    public void Cut_OnConcaveL_ReturnsClosedMeshesWithTheRightVolumes()
    {
        var (bottom, top) = CutL();

        Assert.True(bottom.IsClosed());
        Assert.True(top.IsClosed());
        Assert.Equal(33000, MeshInfo.GetMeshInfo(bottom).Volume!.Value, 1e-1);   // 3300 x 10
        Assert.Equal(99000, MeshInfo.GetMeshInfo(top).Volume!.Value, 1e-1);      // 3300 x 30
    }

    [Fact]
    public void Cut_OnConcaveL_ReturnsTheExpectedAreas()
    {
        var (bottom, top) = CutL();
        Assert.Equal(10300, MeshInfo.GetMeshInfo(bottom).Area, 1e-1);
        Assert.Equal(17700, MeshInfo.GetMeshInfo(top).Area, 1e-1);
    }

    [Fact]
    public void Cut_OnConcaveL_CutFaceHasTheAreaOfTheSection()
    {
        var (bottom, top) = CutL();

        foreach (var mesh in new[] { bottom, top })
        {
            var inPlane = TrianglesInCutPlane(mesh);
            double area = inPlane.Sum(tid => Math.Abs(SignedAreaZ(mesh, tid)));
            Assert.Equal(3300, area, 1e-1);
        }
    }

    [Fact]
    public void Cut_OnConcaveL_CutFaceTrianglesAllPointOutwards()
    {
        var (bottom, top) = CutL();

        var bottomWrong = TrianglesInCutPlane(bottom).Where(tid => SignedAreaZ(bottom, tid) <= 0).ToList();
        Assert.Empty(bottomWrong);

        var topWrong = TrianglesInCutPlane(top).Where(tid => SignedAreaZ(top, tid) >= 0).ToList();
        Assert.Empty(topWrong);
    }
}