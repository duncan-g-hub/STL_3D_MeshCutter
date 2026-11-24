using g3;
using MeshCutter.App.ViewModels;
using MeshCutter.Core;
using MeshCutter.Tests.Fakes;

namespace MeshCutter.Tests;

public partial class PieceViewModelTests
{
    private static void AssertBox(AxisAlignedBox3d b,
        double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
    {
        Assert.Equal(minX, b.Min.x, 1e-3);
        Assert.Equal(maxX, b.Max.x, 1e-3);
        Assert.Equal(minY, b.Min.y, 1e-3);
        Assert.Equal(maxY, b.Max.y, 1e-3);
        Assert.Equal(minZ, b.Min.z, 1e-3);
        Assert.Equal(maxZ, b.Max.z, 1e-3);
    }

    private static PieceViewModel CreatePieceWithSettings(
        double sizeX, double sizeY, double sizeZ, double marginX, double marginY, double marginZ)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var fileFake = new FakePieceWindowService();
        var fileViewModel = new FileViewModel(new FakeFileDialogService(path, null, null), fileFake);
        fileViewModel.OpenFile();
        fileViewModel.SlicerAxis = Axis.Z;
        fileViewModel.SlicerPosition = 25;
        fileViewModel.CutMesh();

        var call = fileFake.Calls[0];
        return new PieceViewModel(call.Mesh, sizeX, sizeY, sizeZ, marginX, marginY, marginZ,
            new FakeFileDialogService(path, null, null), new FakePieceWindowService(),
            call.CurrentPieceHistory, call.SharedHistory);
    }

    [Fact]
    public void Constructor_BuildsTheBoxesFromTheGivenSettings()
    {
        var (piece, _, _) = CreatePiece();

        Assert.Null(piece.PrintErrorMessage);
        AssertBox(piece.PrintVolumeBox, -40, 40, -100, 100, 0, 150);
        AssertBox(piece.UtilBox, -20, 20, -70, 70, 0, 130);     // (80-40)/2, (200-60)/2, 150-20
    }

    [Fact]
    public void Constructor_WhenSettingsPassThroughInvalidIntermediateStates_StillBuildsTheFinalBoxes()
    {
        var piece = CreatePieceWithSettings(30, 40, 15, 10, 10, 5);

        Assert.Null(piece.PrintErrorMessage);
        AssertBox(piece.PrintVolumeBox, -15, 15, -20, 20, 0, 15);
        AssertBox(piece.UtilBox, -10, 10, -15, 15, 0, 10);      // (30-10)/2, (40-10)/2, 15-5
    }
}