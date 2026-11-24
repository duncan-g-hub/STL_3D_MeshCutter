using g3;
using MeshCutter.Tests.Fakes;
using MeshCutter.App.ViewModels;

namespace MeshCutter.Tests;

public partial class FileViewModelTests
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

    private static FileViewModel CreateViewModelWithoutFile() =>
        new(new FakeFileDialogService(null, null, null), new FakePieceWindowService());

    [Fact]
    public void Constructor_WithoutFile_BuildsTheBoxesOfTheDefaultSettings()
    {
        var viewModel = CreateViewModelWithoutFile();

        Assert.Null(viewModel.PrintErrorMessage);
        AssertBox(viewModel.PrintVolumeBox, -100, 100, -100, 100, 0, 200);
        AssertBox(viewModel.UtilBox, -80, 80, -70, 70, 0, 180);
    }

    [Fact]
    public void PrintSizes_WhenChanged_UpdateBothBoxes()
    {
        var viewModel = CreateViewModelWithoutFile();

        viewModel.PrintSizeX = 300;
        viewModel.PrintSizeY = 150;
        viewModel.PrintSizeZ = 120;

        Assert.Null(viewModel.PrintErrorMessage);
        AssertBox(viewModel.PrintVolumeBox, -150, 150, -75, 75, 0, 120);
        AssertBox(viewModel.UtilBox, -130, 130, -45, 45, 0, 100);   // (300-40)/2, (150-60)/2, 120-20
    }

    [Fact]
    public void Margins_WhenChanged_UpdateOnlyTheUtilBox()
    {
        var viewModel = CreateViewModelWithoutFile();

        viewModel.MarginX = 100;
        viewModel.MarginY = 20;
        viewModel.MarginZ = 50;

        Assert.Null(viewModel.PrintErrorMessage);
        AssertBox(viewModel.PrintVolumeBox, -100, 100, -100, 100, 0, 200);
        AssertBox(viewModel.UtilBox, -50, 50, -90, 90, 0, 150);     // (200-100)/2, (200-20)/2, 200-50
    }

    [Fact]
    public void PrintSize_WhenInvalid_KeepsTheLastValidBoxesAndSetsTheMessage()
    {
        var viewModel = CreateViewModelWithoutFile();
        viewModel.PrintSizeX = 300;     // valeur valide différente du défaut : prouve "dernier valide" et non "défaut"

        viewModel.PrintSizeX = 0;

        Assert.NotNull(viewModel.PrintErrorMessage);
        AssertBox(viewModel.PrintVolumeBox, -150, 150, -100, 100, 0, 200);
        AssertBox(viewModel.UtilBox, -130, 130, -70, 70, 0, 180);
    }

    [Fact]
    public void PrintSize_WhenNotFinite_DoesNotThrowAndKeepsTheLastValidBoxes()
    {
        var viewModel = CreateViewModelWithoutFile();
        viewModel.PrintSizeX = 300;

        foreach (double bad in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
        {
            viewModel.PrintSizeX = bad;

            Assert.NotNull(viewModel.PrintErrorMessage);
            AssertBox(viewModel.PrintVolumeBox, -150, 150, -100, 100, 0, 200);
            AssertBox(viewModel.UtilBox, -130, 130, -70, 70, 0, 180);
        }
    }

    [Fact]
    public void Margin_WhenNotSmallerThanTheSize_KeepsTheLastValidBoxesAndSetsTheMessage()
    {
        var viewModel = CreateViewModelWithoutFile();

        viewModel.MarginZ = 200;        // marge égale à la taille Z (200)

        Assert.NotNull(viewModel.PrintErrorMessage);
        AssertBox(viewModel.PrintVolumeBox, -100, 100, -100, 100, 0, 200);
        AssertBox(viewModel.UtilBox, -80, 80, -70, 70, 0, 180);
    }

    [Fact]
    public void PrintSize_WhenCorrectedAfterAnError_UpdatesTheBoxesAndClearsTheMessage()
    {
        var viewModel = CreateViewModelWithoutFile();

        viewModel.PrintSizeX = 0;
        Assert.NotNull(viewModel.PrintErrorMessage);

        viewModel.PrintSizeX = 300;

        Assert.Null(viewModel.PrintErrorMessage);
        AssertBox(viewModel.PrintVolumeBox, -150, 150, -100, 100, 0, 200);
        AssertBox(viewModel.UtilBox, -130, 130, -70, 70, 0, 180);
    }

    [Fact]
    public void OpenFile_DoesNotChangeTheBoxes()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());

        viewModel.OpenFile();

        AssertBox(viewModel.PrintVolumeBox, -100, 100, -100, 100, 0, 200);
        AssertBox(viewModel.UtilBox, -80, 80, -70, 70, 0, 180);
    }
}