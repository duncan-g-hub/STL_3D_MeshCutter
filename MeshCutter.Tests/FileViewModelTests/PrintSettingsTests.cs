using MeshCutter.Tests.Fakes;
using MeshCutter.App.ViewModels;

namespace MeshCutter.Tests;

public partial class FileViewModelTests
{
    [Fact]
    public void CutPlanMesh_WhenMeshSmallerThanPrintableVolume_ReturnOnePiece()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();
        viewModel.PrintSizeX = 180;
        viewModel.MarginX = 40;
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(1, viewModel.PiecesX);
    }

    [Fact]
    public void CutPlanMesh_WhenMeshBiggerThanPrintableVolume_ReturnTwoPieces()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();
        viewModel.PrintSizeX = 80;
        viewModel.MarginX = 40;
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(2, viewModel.PiecesX);
    }

    [Fact]
    public void CutPlanMesh_WhenPrintableVolumeEqualZero_SetsError()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();
        viewModel.PrintSizeX = 0;
        viewModel.MarginX = 40;
        Assert.Equal("Axe X : l'espace de l'imprimante doit être supérieur à 0mm.",viewModel.PrintErrorMessage);
        Assert.Equal(0, viewModel.PiecesX);
        Assert.Equal(0, viewModel.PiecesY);
        Assert.Equal(0, viewModel.PiecesZ);
        viewModel.PrintSizeX = 200;
        Assert.Null(viewModel.PrintErrorMessage);
    }

    [Fact]
    public void UpdateMarginsAndPrintableVolume_WhenNoFileLoaded_ReturnZeroPiece()
    {
        FileViewModel viewModel = new(new FakeFileDialogService(null, null, null), new FakePieceWindowService());
        viewModel.PrintSizeX = 0;
        viewModel.MarginX = -5;
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(0, viewModel.PiecesX);
    }

    [Fact]
    public void CutPlanMesh_WhenInvalidValueIsCorrected_ErrorDisappearsAndPiecesComeBack()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();

        viewModel.PrintSizeX = 0;
        Assert.NotNull(viewModel.PrintErrorMessage);
        Assert.Equal(0, viewModel.PiecesX);

        viewModel.PrintSizeX = 200;
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(1, viewModel.PiecesX);
    }

    [Fact]
    public void CutPlanMesh_WhenSeveralValuesAreInvalid_ShowsAllErrors()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();

        viewModel.PrintSizeX = 0;
        viewModel.MarginZ = -5;

        Assert.Equal(
            "Axe X : l'espace de l'imprimante doit être supérieur à 0mm.\n" +
            "Axe Z : la marge doit être supérieure ou égale à 0mm.",
            viewModel.PrintErrorMessage);
    }

    [Fact]
    public void CutPlanMesh_WhenPrintSizeIsNotANumber_SetsError()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();

        viewModel.PrintSizeX = double.NaN;

        Assert.Equal("Axe X : l'espace de l'imprimante doit être un nombre valide.", viewModel.PrintErrorMessage);
        Assert.Equal(0, viewModel.PiecesX);
        Assert.Equal(0, viewModel.PiecesY);
        Assert.Equal(0, viewModel.PiecesZ);
    }

    [Fact]
    public void CutPlanMesh_WhenMarginIsInfinite_SetsError()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();

        viewModel.MarginY = double.PositiveInfinity;

        Assert.Equal("Axe Y : la marge doit être un nombre valide.", viewModel.PrintErrorMessage);
        Assert.Equal(0, viewModel.PiecesX);
    }
}