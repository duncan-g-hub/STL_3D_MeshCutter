using MeshCutter.Tests.Fakes;
using MeshCutter.App.ViewModels;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public partial class FileViewModelTests
{
    [Fact]
    public void CutMesh_WhenConfigurationIsValid_OpensTwoPieceWindows()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var fake = new FakePieceWindowService();
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), fake);
        viewModel.OpenFile();
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();

        Assert.Equal(2, fake.Calls.Count);
    }

    [Fact]
    public void CutMesh_WhenConfigurationIsValid_OpensBottomThenTop()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var fake = new FakePieceWindowService();
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), fake);
        viewModel.OpenFile();
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();

        Assert.NotNull(viewModel.MeshBottom);
        Assert.NotNull(viewModel.MeshTop);
        Assert.Same(viewModel.MeshBottom, fake.Calls[0].Mesh);
        Assert.Same(viewModel.MeshTop, fake.Calls[1].Mesh);
    }

    [Fact]
    public void CutMesh_WhenConfigurationIsValid_PassesPrinterSettingsToBothWindows()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var fake = new FakePieceWindowService();
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), fake);
        viewModel.OpenFile();
        viewModel.PrintSizeX = 210;
        viewModel.PrintSizeY = 170;
        viewModel.PrintSizeZ = 130;
        viewModel.MarginX = 15;
        viewModel.MarginY = 25;
        viewModel.MarginZ = 35;
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();

        Assert.Equal(2, fake.Calls.Count);
        foreach (var call in fake.Calls)
        {
            Assert.Equal(210, call.SizeX);
            Assert.Equal(170, call.SizeY);
            Assert.Equal(130, call.SizeZ);
            Assert.Equal(15, call.MarginX);
            Assert.Equal(25, call.MarginY);
            Assert.Equal(35, call.MarginZ);
        }
    }

    [Fact]
    public void CutMesh_WhenPositionIsInvalid_OpensNoWindow()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var fake = new FakePieceWindowService();
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), fake);
        viewModel.OpenFile();
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 100;

        viewModel.CutMesh();

        Assert.NotNull(viewModel.SlicerErrorMessage);
        Assert.Empty(fake.Calls);
    }

    [Fact]
    public void CutMesh_WhenMeshIsNotClosed_OpensNoWindow()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm trou.STL");
        var fake = new FakePieceWindowService();
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), fake);
        viewModel.OpenFile();
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();

        Assert.NotNull(viewModel.SlicerErrorMessage);
        Assert.Empty(fake.Calls);
    }

    [Fact]
    public void CutMesh_WhenNoFileLoaded_OpensNoWindow()
    {
        var fake = new FakePieceWindowService();
        var viewModel = new FileViewModel(new FakeFileDialogService(null, null, null), fake);

        viewModel.CutMesh();

        Assert.Empty(fake.Calls);
    }
}