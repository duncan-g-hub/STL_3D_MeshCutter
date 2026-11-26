using g3;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public class MoveMeshTests
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
    public void CenterMesh_WhenCubeIsLoadedAsIs_CentersXYAndKeepsZ()
    {
        var mesh = LoadCube();

        MoveMesh.CenterMesh(mesh);

        var b = mesh.GetBounds();
        Assert.Equal(-25, b.Min.x, 1e-3);
        Assert.Equal(25, b.Max.x, 1e-3);
        Assert.Equal(-25, b.Min.y, 1e-3);
        Assert.Equal(25, b.Max.y, 1e-3);
        Assert.Equal(0, b.Min.z, 1e-3);
        Assert.Equal(50, b.Max.z, 1e-3);
    }

    [Fact]
    public void CenterMesh_WhenCubeIsOffsetDifferentlyOnEachAxis_CentersXYAndSitsOnZZero()
    {
        var mesh = LoadCube(10, -80, 5);

        MoveMesh.CenterMesh(mesh);

        var b = mesh.GetBounds();
        Assert.Equal(-25, b.Min.x, 1e-3);
        Assert.Equal(25, b.Max.x, 1e-3);
        Assert.Equal(-25, b.Min.y, 1e-3);
        Assert.Equal(25, b.Max.y, 1e-3);
        Assert.Equal(0, b.Min.z, 1e-3);
        Assert.Equal(50, b.Max.z, 1e-3);
    }

    [Fact]
    public void CenterMesh_WhenCubeIsBelowThePlate_RaisesItToZZero()
    {
        var mesh = LoadCube(0, 0, -70);

        MoveMesh.CenterMesh(mesh);

        var b = mesh.GetBounds();
        Assert.Equal(0, b.Min.z, 1e-3);
        Assert.Equal(50, b.Max.z, 1e-3);
    }

    [Fact]
    public void CenterMesh_DoesNotChangeSizesNorVolume()
    {
        var mesh = LoadCube(10, -80, 5);

        MoveMesh.CenterMesh(mesh);

        var info = MeshInfo.GetMeshInfo(mesh);
        Assert.Equal(50, info.SizeX, 1e-3);
        Assert.Equal(50, info.SizeY, 1e-3);
        Assert.Equal(50, info.SizeZ, 1e-3);
        Assert.NotNull(info.Volume);
        Assert.Equal(125000, info.Volume.Value, 1e-3);
    }

    [Fact]
    public void CenterMesh_WhenCalledTwice_GivesTheSameResult()
    {
        var mesh = LoadCube(10, -80, 5);

        MoveMesh.CenterMesh(mesh);
        MoveMesh.CenterMesh(mesh);

        var b = mesh.GetBounds();
        Assert.Equal(-25, b.Min.x, 1e-3);
        Assert.Equal(25, b.Max.x, 1e-3);
        Assert.Equal(-25, b.Min.y, 1e-3);
        Assert.Equal(25, b.Max.y, 1e-3);
        Assert.Equal(0, b.Min.z, 1e-3);
        Assert.Equal(50, b.Max.z, 1e-3);
    }

    [Fact]
    public void CenterMesh_WhenMeshIsEmpty_DoesNothing()
    {
        var mesh = new DMesh3();

        var exception = Record.Exception(() => MoveMesh.CenterMesh(mesh));

        Assert.Null(exception);
        Assert.Equal(0, mesh.VertexCount);
    }
}