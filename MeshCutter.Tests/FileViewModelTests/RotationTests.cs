using MeshCutter.Tests.Fakes;
using MeshCutter.App.ViewModels;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public partial class FileViewModelTests
{
    private static (FileViewModel ViewModel, FakePieceWindowService Fake) OpenL()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "L 65x120x40.STL");
        var fake = new FakePieceWindowService();
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), fake);
        viewModel.OpenFile();
        return (viewModel, fake);
    }

    [Fact]
    public void RotateMesh_WhenNoFileLoaded_DoesNothing()
    {
        var viewModel = CreateViewModelWithoutFile();
        viewModel.RotationAxis = Axis.Z;
        viewModel.RotationAngle = 90;

        viewModel.RotateMesh();

        Assert.Null(viewModel.CurrentMesh);
        Assert.Null(viewModel.RotateErrorMessage);
    }

    [Fact]
    public void RotateMesh_WhenAngleIsZero_ChangesNothing()
    {
        var (viewModel, _) = OpenL();
        var meshBefore = viewModel.CurrentMesh;
        var historyBefore = viewModel.CurrentPieceHistory;
        viewModel.RotationAngle = 0;

        viewModel.RotateMesh();

        Assert.Same(meshBefore, viewModel.CurrentMesh);
        Assert.Same(historyBefore, viewModel.CurrentPieceHistory);
        Assert.Single(viewModel.SharedHistory!.Pieces);
    }

    [Theory]
    [InlineData(360)]
    [InlineData(720)]
    [InlineData(-360)]
    public void RotateMesh_WhenAngleIsAFullTurn_ChangesNothing(double angle)
    {
        var (viewModel, _) = OpenL();
        var meshBefore = viewModel.CurrentMesh;
        viewModel.RotationAngle = angle;

        viewModel.RotateMesh();

        Assert.Same(meshBefore, viewModel.CurrentMesh);
        Assert.Single(viewModel.SharedHistory!.Pieces);
    }

    [Fact]
    public void RotateMesh_OnZ90_SwapsXAndYAndKeepsThePieceOnThePlate()
    {
        var (viewModel, _) = OpenL();
        viewModel.RotationAxis = Axis.Z;
        viewModel.RotationAngle = 90;

        viewModel.RotateMesh();

        Assert.Null(viewModel.RotateErrorMessage);
        Assert.NotNull(viewModel.CurrentMesh);
        AssertBox(viewModel.CurrentMesh.GetBounds(), -60, 60, -32.5, 32.5, 0, 40);
    }

    [Fact]
    public void RotateMesh_OnX90_PutsTheLengthOfYAlongZ()
    {
        var (viewModel, _) = OpenL();
        viewModel.RotationAxis = Axis.X;
        viewModel.RotationAngle = 90;

        viewModel.RotateMesh();

        Assert.NotNull(viewModel.CurrentMesh);
        AssertBox(viewModel.CurrentMesh.GetBounds(), -32.5, 32.5, -20, 20, 0, 120);
    }

    [Fact]
    public void RotateMesh_OnY90_PutsTheLengthOfXAlongZ()
    {
        var (viewModel, _) = OpenL();
        viewModel.RotationAxis = Axis.Y;
        viewModel.RotationAngle = 90;

        viewModel.RotateMesh();

        Assert.NotNull(viewModel.CurrentMesh);
        AssertBox(viewModel.CurrentMesh.GetBounds(), -20, 20, -60, 60, 0, 65);
    }

    [Fact]
    public void RotateMesh_ReplacesCurrentMeshByACopyAndLeavesTheOldOneUntouched()
    {
        var (viewModel, _) = OpenL();
        var meshBefore = viewModel.CurrentMesh;
        Assert.NotNull(meshBefore);
        viewModel.RotationAxis = Axis.Z;
        viewModel.RotationAngle = 90;

        viewModel.RotateMesh();

        Assert.NotSame(meshBefore, viewModel.CurrentMesh);
        AssertBox(meshBefore.GetBounds(), -32.5, 32.5, -60, 60, 0, 40);
    }

    [Fact]
    public void RotateMesh_RefreshesTheDisplayTheInfosAndThePieceCount()
    {
        var (viewModel, _) = OpenL();
        viewModel.PrintSizeX = 100;
        viewModel.MarginX = 0;
        viewModel.PrintSizeY = 100;
        viewModel.MarginY = 0;
        Assert.Equal(1, viewModel.PiecesX);     // 65 de long sur 100 utiles
        Assert.Equal(2, viewModel.PiecesY);     // 120 de long sur 100 utiles
        viewModel.RotationAxis = Axis.Z;
        viewModel.RotationAngle = 90;

        viewModel.RotateMesh();

        Assert.NotNull(viewModel.Mesh);
        Assert.Equal(120, viewModel.Mesh.Bounds.SizeX, 1e-3);
        Assert.Equal(65, viewModel.Mesh.Bounds.SizeY, 1e-3);
        Assert.Equal(2, viewModel.PiecesX);
        Assert.Equal(1, viewModel.PiecesY);
        Assert.Contains("Axe X : 120", viewModel.Informations);
    }

    [Fact]
    public void RotateMesh_WhenAngleIsNegative_IsRecordedAsItsPositiveEquivalent()
    {
        var (viewModel, _) = OpenL();
        viewModel.RotationAxis = Axis.Z;
        viewModel.RotationAngle = -90;

        viewModel.RotateMesh();

        Assert.NotNull(viewModel.CurrentPieceHistory);
        Assert.Equal(270, viewModel.CurrentPieceHistory.RotationAngle);
        Assert.Equal("L 65x120x40 origin > rotate Z 270", viewModel.CurrentPieceHistory.Name);
    }

    [Fact]
    public void RotateMesh_WhenAngleIsAboveAFullTurn_IsRecordedModuloAFullTurn()
    {
        var (viewModel, _) = OpenL();
        viewModel.RotationAxis = Axis.Z;
        viewModel.RotationAngle = 450;

        viewModel.RotateMesh();

        Assert.NotNull(viewModel.CurrentPieceHistory);
        Assert.Equal(90, viewModel.CurrentPieceHistory.RotationAngle);
    }

    [Fact]
    public void RotateMesh_AddsOneEntryToTheHistory()
    {
        var (viewModel, _) = OpenL();
        var rootHistory = viewModel.CurrentPieceHistory;
        viewModel.RotationAxis = Axis.Z;
        viewModel.RotationAngle = 90;

        viewModel.RotateMesh();

        Assert.NotNull(viewModel.SharedHistory);
        Assert.NotNull(viewModel.CurrentPieceHistory);
        Assert.Equal(2, viewModel.SharedHistory.Pieces.Count);
        Assert.Equal("P2", viewModel.CurrentPieceHistory.Id);
        Assert.Equal("P1", viewModel.CurrentPieceHistory.ParentId);
        Assert.Equal("L 65x120x40 origin > rotate Z 90", viewModel.CurrentPieceHistory.Name);
        Assert.Equal(90, viewModel.CurrentPieceHistory.RotationAngle);
        Assert.Same(viewModel.CurrentMesh, viewModel.CurrentPieceHistory.Mesh);
        Assert.NotSame(rootHistory!.Mesh, viewModel.CurrentPieceHistory.Mesh);
        AssertBox(rootHistory.Mesh.GetBounds(), -32.5, 32.5, -60, 60, 0, 40);
    }

    [Fact]
    public void RotateMesh_ThenCutMesh_ChainsTheHistory()
    {
        var (viewModel, fake) = OpenL();
        viewModel.RotationAxis = Axis.Z;
        viewModel.RotationAngle = 90;
        viewModel.RotateMesh();
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();

        Assert.Null(viewModel.SlicerErrorMessage);
        Assert.Equal(2, fake.Calls.Count);
        Assert.Equal("P3", fake.Calls[0].CurrentPieceHistory.Id);
        Assert.Equal("P2", fake.Calls[0].CurrentPieceHistory.ParentId);
        Assert.Equal("L 65x120x40 origin > rotate Z 90 > slice Z 25 bottom", fake.Calls[0].CurrentPieceHistory.Name);
        Assert.Equal("P4", fake.Calls[1].CurrentPieceHistory.Id);
        Assert.Equal("L 65x120x40 origin > rotate Z 90 > slice Z 25 top", fake.Calls[1].CurrentPieceHistory.Name);
        Assert.Equal(4, viewModel.SharedHistory!.Pieces.Count);
    }

    [Fact]
    public void RotateMesh_ThenCutMesh_CutsTheRotatedPiece()
    {
        var (viewModel, fake) = OpenL();
        viewModel.RotationAxis = Axis.Z;
        viewModel.RotationAngle = 90;
        viewModel.RotateMesh();
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();
        AssertBox(fake.Calls[0].Mesh.GetBounds(), -60, 60, -32.5, 32.5, 0, 25);
    }

    [Fact]
    public void RotateMesh_WhenAngleIsNotANumber_SetsErrorAndChangesNothing()
    {
        var (viewModel, _) = OpenL();
        var meshBefore = viewModel.CurrentMesh;
        viewModel.RotationAxis = Axis.Z;

        foreach (double bad in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
        {
            viewModel.RotationAngle = bad;
            viewModel.RotateMesh();

            Assert.Equal("L'angle de rotation doit être un nombre valide.", viewModel.RotateErrorMessage);
            Assert.Same(meshBefore, viewModel.CurrentMesh);
            Assert.Single(viewModel.SharedHistory!.Pieces);
        }
    }

    [Fact]
    public void RotateMesh_WhenMeshIsNotClosed_SetsErrorAndChangesNothing()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm trou.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();
        var meshBefore = viewModel.CurrentMesh;
        viewModel.RotationAxis = Axis.Z;
        viewModel.RotationAngle = 90;

        viewModel.RotateMesh();

        Assert.Equal("Le modèle doit être fermé pour procéder à une rotation.", viewModel.RotateErrorMessage);
        Assert.Same(meshBefore, viewModel.CurrentMesh);
        Assert.Single(viewModel.SharedHistory!.Pieces);
    }

    [Fact]
    public void RotateMesh_WhenCorrectedAfterAnError_ClearsTheMessage()
    {
        var (viewModel, _) = OpenL();
        viewModel.RotationAxis = Axis.Z;
        viewModel.RotationAngle = double.NaN;
        viewModel.RotateMesh();
        Assert.NotNull(viewModel.RotateErrorMessage);

        viewModel.RotationAngle = 90;
        viewModel.RotateMesh();

        Assert.Null(viewModel.RotateErrorMessage);
    }

    [Fact]
    public void RotateMesh_WhenAngleBecomesZeroAfterAnError_ClearsTheMessage()
    {
        var (viewModel, _) = OpenL();
        viewModel.RotationAngle = double.NaN;
        viewModel.RotateMesh();
        Assert.NotNull(viewModel.RotateErrorMessage);

        viewModel.RotationAngle = 0;
        viewModel.RotateMesh();

        Assert.Null(viewModel.RotateErrorMessage);
    }
}