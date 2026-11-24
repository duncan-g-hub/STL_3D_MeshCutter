using g3;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public class MeshInfoTests
{
    private static DMesh3 LoadCube(double dx = 0, double dy = 0, double dz = 0)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var (_, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        MeshTransforms.Translate(mesh, dx, dy, dz);
        return mesh;
    }


    [Fact]
    public void Constructor_DerivesSizesAndBoundsFromTheBox()
    {
        var bounds = new AxisAlignedBox3d(-32.5, -60, 0, 32.5, 60, 40);
        var info = new MeshInfo(12, 20, bounds, 21400, 132000, true);
        Assert.Equal(65, info.SizeX, 1e-3);
        Assert.Equal(120, info.SizeY, 1e-3);
        Assert.Equal(40, info.SizeZ, 1e-3);
        Assert.Equal(-32.5, info.MinX, 1e-3);
        Assert.Equal(-60, info.MinY, 1e-3);
        Assert.Equal(0, info.MinZ, 1e-3);
        Assert.Equal(32.5, info.MaxX, 1e-3);
        Assert.Equal(60, info.MaxY, 1e-3);
        Assert.Equal(40, info.MaxZ, 1e-3);
    }

    [Fact]
    public void Constructor_KeepsTheOtherValues()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 10, 10, 10);
        var info = new MeshInfo(12, 20, bounds, 21400, 132000, true);
        Assert.Equal(12, info.VertexCount);
        Assert.Equal(20, info.TriangleCount);
        Assert.Equal(21400, info.Area, 1e-3);
        Assert.NotNull(info.Volume);
        Assert.Equal(132000, info.Volume.Value, 1e-3);
        Assert.True(info.IsClosed);
    }

    [Fact]
    public void GetMeshInfo_OnSampleCube_ReturnsCountsAreaAndVolume()
    {
        var info = MeshInfo.GetMeshInfo(LoadCube());
        Assert.Equal(8, info.VertexCount);
        Assert.Equal(12, info.TriangleCount);
        Assert.Equal(15000, info.Area, 1e-3);          
        Assert.NotNull(info.Volume);
        Assert.Equal(125000, info.Volume.Value, 1e-3);
        Assert.True(info.IsClosed);
    }

    [Fact]
    public void GetMeshInfo_OnSampleCube_ReturnsSizesAndBounds()
    {
        var info = MeshInfo.GetMeshInfo(LoadCube());
        Assert.Equal(50, info.SizeX, 1e-3);
        Assert.Equal(50, info.SizeY, 1e-3);
        Assert.Equal(50, info.SizeZ, 1e-3);
        Assert.Equal(0, info.MinX, 1e-3);
        Assert.Equal(0, info.MinY, 1e-3);
        Assert.Equal(0, info.MinZ, 1e-3);
        Assert.Equal(50, info.MaxX, 1e-3);
        Assert.Equal(50, info.MaxY, 1e-3);
        Assert.Equal(50, info.MaxZ, 1e-3);
    }

    [Fact]
    public void GetMeshInfo_WhenCubeIsOffsetDifferentlyOnEachAxis_ReturnsTheRightBoundsOnTheRightAxis()
    {
        var info = MeshInfo.GetMeshInfo(LoadCube(10, -80, 5));
        Assert.Equal(10, info.MinX, 1e-3);
        Assert.Equal(60, info.MaxX, 1e-3);
        Assert.Equal(-80, info.MinY, 1e-3);
        Assert.Equal(-30, info.MaxY, 1e-3);
        Assert.Equal(5, info.MinZ, 1e-3);
        Assert.Equal(55, info.MaxZ, 1e-3);
    }

    [Fact]
    public void GetMeshInfo_WhenCubeIsOffset_SizesVolumeAndAreaDoNotChange()
    {
        var info = MeshInfo.GetMeshInfo(LoadCube(10, -80, 5));
        Assert.Equal(50, info.SizeX, 1e-3);
        Assert.Equal(50, info.SizeY, 1e-3);
        Assert.Equal(50, info.SizeZ, 1e-3);
        Assert.Equal(15000, info.Area, 1e-3);
        Assert.NotNull(info.Volume);
        Assert.Equal(125000, info.Volume.Value, 1e-3);
    }

    [Fact]
    public void GetMeshInfo_WhenMeshIsNotClosed_HasNoVolume()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm trou.STL");
        var (_, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        var info = MeshInfo.GetMeshInfo(mesh);
        Assert.False(info.IsClosed);
        Assert.Null(info.Volume);
    }
}