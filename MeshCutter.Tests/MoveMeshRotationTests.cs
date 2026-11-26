using g3;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public class MoveMeshRotationTests
{
    private static readonly double[] NotFiniteValues =
    {
        double.NaN, double.PositiveInfinity, double.NegativeInfinity
    };

    private static DMesh3 LoadL()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "L 65x120x40.STL");
        var (_, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        return mesh;
    }

    private static void AssertBox(AxisAlignedBox3d b,
        double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
    {
        Assert.Equal(minX, b.Min.x, 1e-3);
        Assert.Equal(maxX, b.Max.x, 1e-3);
        Assert.Equal(minY, b.Min.y, 1e-3);
        Assert.Equal(maxY, b.Max.y, 1e-3);
        Assert.Equal(minZ, b.Min.z, 1e-3);
        Assert.Equal(maxZ, b.Max.z, 1e-3);
    }


    [Fact]
    public void CopyAndRotateMesh_OnZ90_TurnsXIntoYAndYIntoMinusX()
    {
        var rotated = MoveMesh.CopyAndRotateMesh(LoadL(), Axis.Z, 90);

        AssertBox(rotated.GetBounds(), -120, 0, 0, 65, 0, 40);
    }

    [Fact]
    public void CopyAndRotateMesh_OnX90_TurnsYIntoZAndZIntoMinusY()
    {
        var rotated = MoveMesh.CopyAndRotateMesh(LoadL(), Axis.X, 90);

        AssertBox(rotated.GetBounds(), 0, 65, -40, 0, 0, 120);
    }

    [Fact]
    public void CopyAndRotateMesh_OnY90_TurnsZIntoXAndXIntoMinusZ()
    {
        var rotated = MoveMesh.CopyAndRotateMesh(LoadL(), Axis.Y, 90);

        AssertBox(rotated.GetBounds(), 0, 40, 0, 120, -65, 0);
    }

    [Fact]
    public void CopyAndRotateMesh_ReturnsACopyAndLeavesTheOriginalUntouched()
    {
        var original = LoadL();

        var rotated = MoveMesh.CopyAndRotateMesh(original, Axis.Z, 90);

        Assert.NotSame(original, rotated);
        AssertBox(original.GetBounds(), 0, 65, 0, 120, 0, 40);
    }

    [Fact]
    public void CopyAndRotateMesh_KeepsTopologyVolumeAndArea()
    {
        var rotated = MoveMesh.CopyAndRotateMesh(LoadL(), Axis.X, 90);

        var info = MeshInfo.GetMeshInfo(rotated);
        Assert.Equal(12, info.VertexCount);
        Assert.Equal(20, info.TriangleCount);
        Assert.True(info.IsClosed);
        Assert.Equal(21400, info.Area, 1e-1);
        Assert.NotNull(info.Volume);
        Assert.Equal(132000, info.Volume.Value, 1e-1);
    }


    [Fact]
    public void CopyAndRotateMesh_WhenAngleIsNotFinite_ThrowsArgumentException()
    {
        foreach (double bad in NotFiniteValues)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => MoveMesh.CopyAndRotateMesh(LoadL(), Axis.Z, bad));
            Assert.Contains("AngleNotANumber", exception.Message);
        }
    }

    [Fact]
    public void CopyAndRotateMesh_WhenMeshIsNotClosed_ThrowsArgumentException()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm trou.STL");
        var (_, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);

        var exception = Assert.Throws<ArgumentException>(
            () => MoveMesh.CopyAndRotateMesh(mesh, Axis.Z, 90));
        Assert.Contains("MeshIsNotClosed", exception.Message);
    }
}