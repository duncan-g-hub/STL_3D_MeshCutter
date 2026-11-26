using MeshCutter.Tests.Fakes;
using MeshCutter.App.ViewModels;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public partial class FileViewModelTests
{
    [Fact]
    public void CutMesh_WhenConfigurationIsValid_RecordsTwoChildrenOfTheRoot()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var fake = new FakePieceWindowService();
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), fake);
        viewModel.OpenFile();
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();

        Assert.Equal("P2", fake.Calls[0].CurrentPieceHistory.Id);
        Assert.Equal("P1", fake.Calls[0].CurrentPieceHistory.ParentId);
        Assert.Equal("cube 50mm origin > slice Z 25 bottom", fake.Calls[0].CurrentPieceHistory.Name);

        Assert.Equal("P3", fake.Calls[1].CurrentPieceHistory.Id);
        Assert.Equal("P1", fake.Calls[1].CurrentPieceHistory.ParentId);
        Assert.Equal("cube 50mm origin > slice Z 25 top", fake.Calls[1].CurrentPieceHistory.Name);

        Assert.Same(viewModel.SharedHistory, fake.Calls[0].SharedHistory);

        Assert.NotNull(viewModel.SharedHistory);
        Assert.Equal(3, viewModel.SharedHistory.Pieces.Count);
    }

    [Fact]
    public void LoadFile_WhenSecondFileLoaded_StartsAFreshHistory()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;
        viewModel.CutMesh();
        var oldHistory = viewModel.SharedHistory;

        viewModel.LoadFile(path);
        Assert.NotNull(viewModel.CurrentPieceHistory);
        Assert.NotNull(viewModel.SharedHistory);
        Assert.NotSame(oldHistory, viewModel.SharedHistory);
        Assert.Equal("P1", viewModel.CurrentPieceHistory.Id);
        Assert.Single(viewModel.SharedHistory.Pieces);
    }

    [Fact]
    public void LoadFile_WhenSecondFileIsInvalid_KeepsPreviousHistory()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;
        viewModel.CutMesh();
        var oldHistory = viewModel.SharedHistory;
        var oldPiece = viewModel.CurrentPieceHistory;

        string pathError = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm error.STL");
        viewModel.LoadFile(pathError);
        Assert.NotNull(viewModel.FileErrorMessage);
        Assert.NotNull(viewModel.CurrentPieceHistory);
        Assert.NotNull(viewModel.SharedHistory);
        Assert.Same(oldHistory, viewModel.SharedHistory);
        Assert.Equal(3, viewModel.SharedHistory.Pieces.Count);
        Assert.Same(oldPiece, viewModel.CurrentPieceHistory);
    }
}