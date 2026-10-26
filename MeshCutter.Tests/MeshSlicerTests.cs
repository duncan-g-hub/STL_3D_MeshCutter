using MeshCutter.Core;
using g3;

namespace MeshCutter.Tests;

public class MeshSlicerTests
{
    [Fact]
    public void Cut_WhenMeshIsOk_ReturnsClosedMeshes()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var (result, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(0, 0, 25), new Vector3d(0, 0, 1));
        Assert.True(meshBottom.IsClosed());
        Assert.True(meshTop.IsClosed());
    }

    [Fact]
    public void Cut_WhenMeshIsOk_ReturnsInitialMesh()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var (result, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(0, 0, 25), new Vector3d(0, 0, 1));
        Assert.Equal(12, meshBase.TriangleCount);
        Assert.Equal(8, meshBase.VertexCount);
    }

    [Fact]
    public void Cut_WhenMeshIsOk_ReturnsTopAndBottomMeshes()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var (result, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(0, 0, 25), new Vector3d(0, 0, 1));
        Assert.Equal(0, meshBottom.GetBounds().Min.z, 1e-3);
        Assert.Equal(25, meshBottom.GetBounds().Max.z, 1e-3);
        Assert.Equal(25, meshTop.GetBounds().Min.z, 1e-3);
        Assert.Equal(50, meshTop.GetBounds().Max.z, 1e-3);
    }

    [Fact]
    public void Cut_WhenMeshIsOk_ReturnsTopPlusBottomVolumeEqualInitialVolume()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var (result, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(0, 0, 25), new Vector3d(0, 0, 1));

        var volumeBottom = MeshInfo.GetMeshInfo(meshBottom).Volume;
        Assert.NotNull(volumeBottom);
        Assert.Equal(62500, volumeBottom.Value, 1e-3);

        var volumeTop = MeshInfo.GetMeshInfo(meshTop).Volume;
        Assert.NotNull(volumeTop);
        Assert.Equal(62500, volumeTop.Value, 1e-3);
        
        var volumeInitial = MeshInfo.GetMeshInfo(mesh).Volume;
        Assert.NotNull(volumeInitial);
        Assert.Equal(volumeInitial.Value, volumeBottom.Value + volumeTop.Value, 1e-3);
    }
}