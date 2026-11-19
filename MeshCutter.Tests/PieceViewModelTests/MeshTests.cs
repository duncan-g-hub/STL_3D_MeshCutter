using MeshCutter.Core;

namespace MeshCutter.Tests;

public partial class PieceViewModelTests
{
    [Fact]
    public void CutMesh_WhenPieceIsCutAgain_OpensTwoNewWindows()
    {
        var (piece, fake, _) = CreatePiece();

        piece.SlicerAxis = Axis.Z;
        piece.SlicerPosition = 10;
        piece.CutMesh();

        Assert.Null(piece.SlicerErrorMessage);
        Assert.Equal(2, fake.Calls.Count);
        Assert.Equal(0, fake.Calls[0].Mesh.GetBounds().Min.z, 1e-3);
        Assert.Equal(10, fake.Calls[0].Mesh.GetBounds().Max.z, 1e-3);
        Assert.Equal(0, fake.Calls[1].Mesh.GetBounds().Min.z, 1e-3);
        Assert.Equal(15, fake.Calls[1].Mesh.GetBounds().Max.z, 1e-3);
    }

    [Fact]
    public void CutMesh_WhenPieceIsCutAgain_RecordsTwoChildrenOfTheFirstChildren()
    {
        var (piece, fake, _) = CreatePiece();

        piece.SlicerAxis = Axis.Y;
        piece.SlicerPosition = 10;
        piece.CutMesh();

        Assert.Equal("P4", fake.Calls[0].CurrentPieceHistory.Id);
        Assert.Equal("P2", fake.Calls[0].CurrentPieceHistory.ParentId);
        Assert.Equal("cube 50mm origin > Z 25 bottom > Y 10 bottom", fake.Calls[0].CurrentPieceHistory.Name);

        Assert.Equal("P5", fake.Calls[1].CurrentPieceHistory.Id);
        Assert.Equal("P2", fake.Calls[1].CurrentPieceHistory.ParentId);
        Assert.Equal("cube 50mm origin > Z 25 bottom > Y 10 top", fake.Calls[1].CurrentPieceHistory.Name);

        Assert.Same(piece.SharedHistory, fake.Calls[0].SharedHistory);

        Assert.NotNull(piece.SharedHistory);
        Assert.Equal(5, piece.SharedHistory.Pieces.Count);
    }
}