using g3;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public class MeshSlicerSuppanglameTests
{
    private const string FileName = "suppanglame 3D.STL";  
    private const double CutZ = 20;

    private const double OriginalVolume = 68456.750414615;
    private const double BottomVolume = 34228.387196484;
    private const double TopVolume = 34228.363218131;
    private const double SectionArea = 1574.334010600;     
    private const double PieceArea = 11525.33667;          

    private static readonly Vector3d Normal = new Vector3d(0, 0, 1);

    private static DMesh3 Load()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", FileName);
        var (_, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        Assert.True(mesh.IsClosed());
        return mesh;
    }

    private static (DMesh3 Bottom, DMesh3 Top) CutAt(double z)
    {
        var (bottom, top, _) = MeshSlicer.Cut(Load(), new Vector3d(0, 0, z), Normal);
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
    public void Cut_AtZ20_DoesNotThrow()
    {
        var exception = Record.Exception(() => CutAt(CutZ));

        Assert.Null(exception);
    }

    [Fact]
    public void Cut_AtZ20_ReturnsClosedPieces()
    {
        var (bottom, top) = CutAt(CutZ);

        Assert.True(bottom.IsClosed());
        Assert.True(top.IsClosed());
    }

    [Fact]
    public void Cut_AtZ20_ReturnsTheExpectedBounds()
    {
        var (bottom, top) = CutAt(CutZ);

        Assert.Equal(0, bottom.GetBounds().Min.z, 1e-3);
        Assert.Equal(20, bottom.GetBounds().Max.z, 1e-3);
        Assert.Equal(20, top.GetBounds().Min.z, 1e-3);
        Assert.Equal(40, top.GetBounds().Max.z, 1e-3);
    }

    [Fact]
    public void Cut_AtZ20_ReturnsTheExpectedVolumes()
    {
        var (bottom, top) = CutAt(CutZ);

        var volumeBottom = MeshInfo.GetMeshInfo(bottom).Volume;
        var volumeTop = MeshInfo.GetMeshInfo(top).Volume;
        Assert.NotNull(volumeBottom);
        Assert.NotNull(volumeTop);
        Assert.Equal(BottomVolume, volumeBottom.Value, 0.05);
        Assert.Equal(TopVolume, volumeTop.Value, 0.05);
        Assert.Equal(OriginalVolume, volumeBottom.Value + volumeTop.Value, 0.05);
    }

    [Fact]
    public void Cut_AtZ20_ReturnsTheExpectedAreas()
    {
        var (bottom, top) = CutAt(CutZ);

        Assert.Equal(PieceArea, MeshInfo.GetMeshInfo(bottom).Area, 0.05);
        Assert.Equal(PieceArea, MeshInfo.GetMeshInfo(top).Area, 0.05);
    }

    [Fact]
    public void Cut_AtZ20_CutFaceHasTheAreaOfTheSection()
    {
        var (bottom, top) = CutAt(CutZ);

        foreach (var mesh in new[] { bottom, top })
        {
            double area = TrianglesInCutPlane(mesh).Sum(tid => Math.Abs(SignedAreaZ(mesh, tid)));
            Assert.Equal(SectionArea, area, 0.05);
        }
    }

    [Fact]
    public void Cut_AtZ20_CutFaceTrianglesAllPointOutwards()
    {
        var (bottom, top) = CutAt(CutZ);

        var bottomWrong = TrianglesInCutPlane(bottom).Where(tid => SignedAreaZ(bottom, tid) <= 0).ToList();
        Assert.Empty(bottomWrong);
        var topWrong = TrianglesInCutPlane(top).Where(tid => SignedAreaZ(top, tid) >= 0).ToList();
        Assert.Empty(topWrong);
    }



    [Fact]
    public void Cut_NearZ20_ReturnsClosedPiecesWhoseVolumesAddUp()
    {
        foreach (double z in new[] { 19.9, 20.1 })
        {
            var (bottom, top) = CutAt(z);

            Assert.True(bottom.IsClosed());
            Assert.True(top.IsClosed());
            double sum = MeshInfo.GetMeshInfo(bottom).Volume!.Value + MeshInfo.GetMeshInfo(top).Volume!.Value;
            Assert.Equal(OriginalVolume, sum, 0.05);
        }
    }
}