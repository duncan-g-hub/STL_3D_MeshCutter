using g3;
using MeshCutter.App.ViewModels;
using MeshCutter.Core;
using MeshCutter.Tests.Fakes;

namespace MeshCutter.Tests;

public class FileViewModelPlacementTests
{
    private static string CubePath() =>
        Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");

    private static string CreateOffsetCubeFile(double dx, double dy, double dz)
    {
        var (_, mesh) = MeshLoader.GetMeshFromSTLFile(CubePath());
        Assert.NotNull(mesh);
        MeshTransforms.Translate(mesh, dx, dy, dz);

        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".stl");
        var result = StandardMeshWriter.WriteMeshes(path, new List<DMesh3> { mesh }, WriteOptions.Defaults);
        Assert.Equal(IOCode.Ok, result.code);
        return path;
    }

    private static void AssertBounds(AxisAlignedBox3d b,
        double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
    {
        Assert.Equal(minX, b.Min.x, 1e-3);
        Assert.Equal(maxX, b.Max.x, 1e-3);
        Assert.Equal(minY, b.Min.y, 1e-3);
        Assert.Equal(maxY, b.Max.y, 1e-3);
        Assert.Equal(minZ, b.Min.z, 1e-3);
        Assert.Equal(maxZ, b.Max.z, 1e-3);
    }

    private static (FileViewModel ViewModel, FakePieceWindowService Fake) OpenCube()
    {
        var fake = new FakePieceWindowService();
        FileViewModel viewModel = new(new FakeFileDialogService(CubePath(), null, null), fake);
        viewModel.OpenFile();
        return (viewModel, fake);
    }

    [Fact]
    public void LoadFile_WhenFileIsOffset_CentersCurrentMeshOnThePlate()
    {
        string path = CreateOffsetCubeFile(10, -80, 5);
        try
        {
            FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
            viewModel.OpenFile();

            Assert.Null(viewModel.FileErrorMessage);
            Assert.NotNull(viewModel.CurrentMesh);
            AssertBounds(viewModel.CurrentMesh.GetBounds(), -25, 25, -25, 25, 0, 50);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadFile_WhenFileIsOffset_DisplaysTheCenteredMesh()
    {
        string path = CreateOffsetCubeFile(10, -80, 5);
        try
        {
            FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
            viewModel.OpenFile();

            Assert.NotNull(viewModel.Mesh);
            var bounds = viewModel.Mesh.Bounds;
            Assert.Equal(-25, bounds.X, 1e-3);
            Assert.Equal(-25, bounds.Y, 1e-3);
            Assert.Equal(0, bounds.Z, 1e-3);
            Assert.Equal(50, bounds.SizeX, 1e-3);
            Assert.Equal(50, bounds.SizeY, 1e-3);
            Assert.Equal(50, bounds.SizeZ, 1e-3);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadFile_WhenFileIsOffset_HistoryRootHoldsTheCenteredMesh()
    {
        string path = CreateOffsetCubeFile(10, -80, 5);
        try
        {
            FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
            viewModel.OpenFile();

            Assert.NotNull(viewModel.CurrentPieceHistory);
            Assert.Same(viewModel.CurrentMesh, viewModel.CurrentPieceHistory.Mesh);
            AssertBounds(viewModel.CurrentPieceHistory.Mesh.GetBounds(), -25, 25, -25, 25, 0, 50);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void CutMeshOnZ_WhenValid_RecentersBothChildren()
    {
        var (viewModel, fake) = OpenCube();   // cube centré : X -25..25, Y -25..25, Z 0..50
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 10;

        viewModel.CutMesh();

        Assert.Equal(2, fake.Calls.Count);
        AssertBounds(fake.Calls[0].Mesh.GetBounds(), -25, 25, -25, 25, 0, 10);   // bas : déjà posé
        AssertBounds(fake.Calls[1].Mesh.GetBounds(), -25, 25, -25, 25, 0, 40);   // haut : 10..50 redescendu à 0..40
    }

    [Fact]
    public void CutMeshOnX_WhenValid_RecentersBothChildren()
    {
        var (viewModel, fake) = OpenCube();
        viewModel.SlicerAxis = Axis.X;
        viewModel.SlicerPosition = 10;

        viewModel.CutMesh();

        Assert.Equal(2, fake.Calls.Count);
        AssertBounds(fake.Calls[0].Mesh.GetBounds(), -17.5, 17.5, -25, 25, 0, 50);   // -25..10 (35) recentré
        AssertBounds(fake.Calls[1].Mesh.GetBounds(), -7.5, 7.5, -25, 25, 0, 50);     // 10..25 (15) recentré
    }

    [Fact]
    public void CutMeshOnY_WhenValid_RecentersBothChildren()
    {
        var (viewModel, fake) = OpenCube();
        viewModel.SlicerAxis = Axis.Y;
        viewModel.SlicerPosition = -10;

        viewModel.CutMesh();

        Assert.Equal(2, fake.Calls.Count);
        AssertBounds(fake.Calls[0].Mesh.GetBounds(), -25, 25, -7.5, 7.5, 0, 50);     // -25..-10 (15) recentré
        AssertBounds(fake.Calls[1].Mesh.GetBounds(), -25, 25, -17.5, 17.5, 0, 50);   // -10..25 (35) recentré
    }

    [Fact]
    public void CutMesh_WhenValid_HistoryHoldsTheCenteredChildren()
    {
        var (viewModel, fake) = OpenCube();
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 10;

        viewModel.CutMesh();

        Assert.Same(fake.Calls[0].Mesh, fake.Calls[0].CurrentPieceHistory.Mesh);
        Assert.Same(fake.Calls[1].Mesh, fake.Calls[1].CurrentPieceHistory.Mesh);
        AssertBounds(fake.Calls[1].CurrentPieceHistory.Mesh.GetBounds(), -25, 25, -25, 25, 0, 40);
    }

    [Fact]
    public void CutMesh_WhenValid_DoesNotMoveTheParentMesh()
    {
        var (viewModel, _) = OpenCube();
        viewModel.SlicerAxis = Axis.X;
        viewModel.SlicerPosition = 10;

        viewModel.CutMesh();

        Assert.NotNull(viewModel.CurrentMesh);
        AssertBounds(viewModel.CurrentMesh.GetBounds(), -25, 25, -25, 25, 0, 50);
    }

    [Fact]
    public void CutMeshOnX_WhenPositionIsBeyondTheCenteredMesh_IsRefused()
    {
        var (viewModel, fake) = OpenCube();
        viewModel.SlicerAxis = Axis.X;
        viewModel.SlicerPosition = 30;

        viewModel.CutMesh();

        Assert.NotNull(viewModel.SlicerErrorMessage);
        Assert.Empty(fake.Calls);
    }

    [Fact]
    public void CutMeshOnX_WhenPositionIsZero_CutsTheCenteredMeshInTwoEqualHalves()
    {
        var (viewModel, fake) = OpenCube();
        viewModel.SlicerAxis = Axis.X;
        viewModel.SlicerPosition = 0;

        viewModel.CutMesh();

        Assert.Null(viewModel.SlicerErrorMessage);
        Assert.Equal(2, fake.Calls.Count);
        AssertBounds(fake.Calls[0].Mesh.GetBounds(), -12.5, 12.5, -25, 25, 0, 50);
        AssertBounds(fake.Calls[1].Mesh.GetBounds(), -12.5, 12.5, -25, 25, 0, 50);
    }
}