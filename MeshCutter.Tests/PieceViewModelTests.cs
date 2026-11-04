using MeshCutter.App.ViewModels;
using MeshCutter.Core;
using MeshCutter.Tests.Fakes;

namespace MeshCutter.Tests;

public class PieceViewModelTests
{
    private static (PieceViewModel Piece, FakePieceWindowService Fake) CreatePiece()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var fileFake = new FakePieceWindowService();
        var fileViewModel = new FileViewModel(new FakeFileDialogService(path), fileFake);
        fileViewModel.OpenFile();
        fileViewModel.SlicerAxis = Axis.Z;
        fileViewModel.SlicerPosition = 25;
        fileViewModel.CutMesh();

        var pieceFake = new FakePieceWindowService();
        var piece = new PieceViewModel(fileFake.Calls[0].Mesh, 80, 200, 150, 40, 60, 20, pieceFake);
        return (piece, pieceFake);
    }

    [Fact]
    public void Constructor_LoadsMeshAndPrinterSettings()
    {
        var (piece, _) = CreatePiece();
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
        var (piece, _) = CreatePiece();

        // pièce 50 x 50 x 25 ; zone utile 40 / 140 / 130
        Assert.Null(piece.PrintErrorMessage);
        Assert.Equal(2, piece.PiecesX);
        Assert.Equal(1, piece.PiecesY);
        Assert.Equal(1, piece.PiecesZ);
    }

    [Fact]
    public void CutMesh_WhenPieceIsCutAgain_OpensTwoNewWindows()
    {
        var (piece, fake) = CreatePiece();
        piece.SlicerAxis = Axis.Z;
        piece.SlicerPosition = 10;

        piece.CutMesh();

        Assert.Null(piece.SlicerErrorMessage);
        Assert.Equal(2, fake.Calls.Count);
        Assert.Equal(0, fake.Calls[0].Mesh.GetBounds().Min.z, 1e-3);
        Assert.Equal(10, fake.Calls[0].Mesh.GetBounds().Max.z, 1e-3);
        Assert.Equal(10, fake.Calls[1].Mesh.GetBounds().Min.z, 1e-3);
        Assert.Equal(25, fake.Calls[1].Mesh.GetBounds().Max.z, 1e-3);
    }
}