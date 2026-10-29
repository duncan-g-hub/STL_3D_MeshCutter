using MeshCutter.Core;
using g3;

namespace MeshCutter.Tests;

public class MeshSlicerTests
{

    private static DMesh3 LoadCube()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var (result, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        return mesh;
    }

    [Fact]
    public void Cut_WhenPlaneIsAboveMesh_ThrowsArgumentException()
    {
        var mesh = LoadCube();
        var exception = Assert.Throws<ArgumentException>(
            () => MeshSlicer.Cut(mesh, new Vector3d(0, 0, 100), new Vector3d(0, 0, 1)));
        Assert.Contains("TooHighSliceCoordinate", exception.Message);
    }

    [Fact]
    public void Cut_WhenPlaneIsBelowMesh_ThrowsArgumentException()
    {
        var mesh = LoadCube();
        var exception = Assert.Throws<ArgumentException>(
            () => MeshSlicer.Cut(mesh, new Vector3d(0, 0, -10), new Vector3d(0, 0, 1)));
        Assert.Contains("TooLowSliceCoordinate", exception.Message);
    }

    [Fact]
    public void Cut_WhenPlaneIsOnTopFace_ThrowsArgumentException()
    {
        var mesh = LoadCube();
        var exception = Assert.Throws<ArgumentException>(
            () => MeshSlicer.Cut(mesh, new Vector3d(0, 0, 50), new Vector3d(0, 0, 1)));
        Assert.Contains("TooHighSliceCoordinate", exception.Message);
    }

    [Fact]
    public void Cut_WhenPlaneIsTooCloseToBottomFace_ThrowsArgumentException()
    {
        var mesh = LoadCube();
        var exception = Assert.Throws<ArgumentException>(
            () => MeshSlicer.Cut(mesh, new Vector3d(0, 0, 0.3), new Vector3d(0, 0, 1)));
        Assert.Contains("TooLowSliceCoordinate", exception.Message);
    }

    [Fact]
    public void Cut_WhenNormalIsNotAligned_ThrowsArgumentException()
    {
        var mesh = LoadCube();
        var exception = Assert.Throws<ArgumentException>(
            () => MeshSlicer.Cut(mesh, new Vector3d(25, 25, 25), new Vector3d(1, 1, 0)));
        Assert.Contains("NormalNotAligned", exception.Message);
    }

    [Fact]
    public void Cut_WhenNormalIsZero_ThrowsArgumentException()
    {
        var mesh = LoadCube();
        var exception = Assert.Throws<ArgumentException>(
            () => MeshSlicer.Cut(mesh, new Vector3d(0, 0, 25), new Vector3d(0, 0, 0)));
        Assert.Contains("NormalIsZero", exception.Message);
    }

    [Fact]
    public void Cut_WhenConfigurationIsInvalid_DoesNotModifyMesh()
    {
        var mesh = LoadCube();
        Assert.Throws<ArgumentException>(
            () => MeshSlicer.Cut(mesh, new Vector3d(0, 0, 100), new Vector3d(0, 0, 1)));
        Assert.Equal(12, mesh.TriangleCount);
        Assert.Equal(8, mesh.VertexCount);
    }


    [Fact]
    public void CutOnZAxis_WhenMeshIsOk_ReturnsClosedMeshes()
    {
        var mesh = LoadCube();
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(0, 0, 25), new Vector3d(0, 0, 1));
        Assert.True(meshBottom.IsClosed());
        Assert.True(meshTop.IsClosed());
    }

    [Fact]
    public void CutOnZAxis_WhenMeshIsOk_ReturnsInitialMesh()
    {
        var mesh = LoadCube();
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(0, 0, 25), new Vector3d(0, 0, 1));
        Assert.Equal(12, meshBase.TriangleCount);
        Assert.Equal(8, meshBase.VertexCount);
    }

    [Fact]
    public void CutOnZAxis_WhenMeshIsOk_ReturnsTopAndBottomMeshes()
    {
        var mesh = LoadCube();
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(0, 0, 25), new Vector3d(0, 0, 1));
        Assert.Equal(0, meshBottom.GetBounds().Min.z, 1e-3);
        Assert.Equal(25, meshBottom.GetBounds().Max.z, 1e-3);
        Assert.Equal(25, meshTop.GetBounds().Min.z, 1e-3);
        Assert.Equal(50, meshTop.GetBounds().Max.z, 1e-3);
    }

    [Fact]
    public void CutOnZAxis_WhenMeshIsOk_ReturnsTopPlusBottomVolumeEqualInitialVolume()
    {
        var mesh = LoadCube();
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


    [Fact]
    public void CutOnXAxis_WhenMeshIsOk_ReturnsClosedMeshes()
    {
        var mesh = LoadCube();
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(25, 0, 0), new Vector3d(1, 0, 0));
        Assert.True(meshBottom.IsClosed());
        Assert.True(meshTop.IsClosed());
    }

    [Fact]
    public void CutOnXAxis_WhenMeshIsOk_ReturnsInitialMesh()
    {
        var mesh = LoadCube();
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(25, 0, 0), new Vector3d(1, 0, 0));
        Assert.Equal(12, meshBase.TriangleCount);
        Assert.Equal(8, meshBase.VertexCount);
    }

    [Fact]
    public void CutOnXAxis_WhenMeshIsOk_ReturnsTopAndBottomMeshes()
    {
        var mesh = LoadCube();
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(25, 0, 0), new Vector3d(1, 0, 0));
        Assert.Equal(0, meshBottom.GetBounds().Min.x, 1e-3);
        Assert.Equal(25, meshBottom.GetBounds().Max.x, 1e-3);
        Assert.Equal(25, meshTop.GetBounds().Min.x, 1e-3);
        Assert.Equal(50, meshTop.GetBounds().Max.x, 1e-3);
    }

    [Fact]
    public void CutOnXAxis_WhenMeshIsOk_ReturnsTopPlusBottomVolumeEqualInitialVolume()
    {
        var mesh = LoadCube();
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(25, 0, 0), new Vector3d(1, 0, 0));

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


    [Fact]
    public void CutOnYAxis_WhenMeshIsOk_ReturnsClosedMeshes()
    {
        var mesh = LoadCube();
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(0, 25, 0), new Vector3d(0, 1, 0));
        Assert.True(meshBottom.IsClosed());
        Assert.True(meshTop.IsClosed());
    }


    [Fact]
    public void CutOnYAxis_WhenMeshIsOk_ReturnsInitialMesh()
    {
        var mesh = LoadCube();
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(0, 25, 0), new Vector3d(0, 1, 0));
        Assert.Equal(12, meshBase.TriangleCount);
        Assert.Equal(8, meshBase.VertexCount);
    }

    [Fact]
    public void CutOnYAxis_WhenMeshIsOk_ReturnsTopAndBottomMeshes()
    {
        var mesh = LoadCube();
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(0, 25, 0), new Vector3d(0, 1, 0));
        Assert.Equal(0, meshBottom.GetBounds().Min.y, 1e-3);
        Assert.Equal(25, meshBottom.GetBounds().Max.y, 1e-3);
        Assert.Equal(25, meshTop.GetBounds().Min.y, 1e-3);
        Assert.Equal(50, meshTop.GetBounds().Max.y, 1e-3);
    }

    [Fact]
    public void CutOnYAxis_WhenMeshIsOk_ReturnsTopPlusBottomVolumeEqualInitialVolume()
    {
        var mesh = LoadCube();
        var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(0, 25, 0), new Vector3d(0, 1, 0));

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