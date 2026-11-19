using MeshCutter.App.ViewModels;
using MeshCutter.Core;
using MeshCutter.Tests.Fakes;

namespace MeshCutter.Tests;

public partial class PieceViewModelTests
{
    private static (PieceViewModel Piece, FakePieceWindowService Fake, FakeFileDialogService Dialog) CreatePiece(string? savePath = null, Axis axis = Axis.Z, double position = 25, int pieceIndex = 0, string? exportPath = null)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var fileFake = new FakePieceWindowService();
        var fileViewModel = new FileViewModel(new FakeFileDialogService(path, null, null), fileFake);
        fileViewModel.OpenFile();
        fileViewModel.SlicerAxis = axis;
        fileViewModel.SlicerPosition = position;
        fileViewModel.CutMesh();

        var pieceFake = new FakePieceWindowService();
        var dialogFake = new FakeFileDialogService(path, savePath, exportPath);
        var piece = new PieceViewModel(fileFake.Calls[pieceIndex].Mesh, 80, 200, 150, 40, 60, 20, dialogFake, pieceFake, fileFake.Calls[pieceIndex].CurrentPieceHistory, fileFake.Calls[pieceIndex].SharedHistory);
        return (piece, pieceFake, dialogFake);
    }

    [Fact]
    public void Constructor_LoadsMeshAndPrinterSettings()
    {
        var (piece, _, _) = CreatePiece();
        Assert.NotNull(piece.Mesh);
        Assert.NotNull(piece.CurrentMesh);
        Assert.NotEqual("Informations : ", piece.Informations);
        Assert.Equal(80, piece.PrintSizeX);
        Assert.Equal(200, piece.PrintSizeY);
        Assert.Equal(150, piece.PrintSizeZ);
        Assert.Equal(40, piece.MarginX);
        Assert.Equal(60, piece.MarginY);
        Assert.Equal(20, piece.MarginZ);
    }

    [Fact]
    public void Constructor_ComputesPiecesWithGivenSettings()
    {
        var (piece, _, _) = CreatePiece();

        Assert.Null(piece.PrintErrorMessage);
        Assert.Equal(2, piece.PiecesX);
        Assert.Equal(1, piece.PiecesY);
        Assert.Equal(1, piece.PiecesZ);
    }
}