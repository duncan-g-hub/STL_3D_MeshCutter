using g3;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public class UtilVolumeBoxTests
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

    [Fact]
    public void Create_WithDefaultSettings_ReturnsTheExpectedBoxes()
    {
        var printVolume = new PrintVolume(200, 200, 200);
        var margins = new Margins(40, 60, 20);

        var (printVolumeBox, utilBox) = UtilVolumeBox.CreatePrintVolumeAndUtilBoxes(printVolume, margins);

        AssertBox(printVolumeBox, -100, 100, -100, 100, 0, 200);
        AssertBox(utilBox, -80, 80, -70, 70, 0, 180);
    }

    [Fact]
    public void Create_WhenSizesAndMarginsDifferOnEachAxis_PutsEachValueOnTheRightAxis()
    {
        var printVolume = new PrintVolume(220, 180, 250);
        var margins = new Margins(20, 40, 30);

        var (printVolumeBox, utilBox) = UtilVolumeBox.CreatePrintVolumeAndUtilBoxes(printVolume, margins);

        AssertBox(printVolumeBox, -110, 110, -90, 90, 0, 250);
        AssertBox(utilBox, -100, 100, -70, 70, 0, 220);
    }

    [Fact]
    public void Create_WhenMarginsAreZero_UtilBoxEqualsThePrintVolumeBox()
    {
        var printVolume = new PrintVolume(220, 180, 250);
        var margins = new Margins(0, 0, 0);

        var (printVolumeBox, utilBox) = UtilVolumeBox.CreatePrintVolumeAndUtilBoxes(printVolume, margins);

        AssertBox(utilBox, -110, 110, -90, 90, 0, 250);
        AssertBox(printVolumeBox, -110, 110, -90, 90, 0, 250);
    }

    [Fact]
    public void Create_WhenMarginXEqualsSizeX_ThrowsErrorOnXOnly()
    {
        var printVolume = new PrintVolume(100, 100, 100);
        var margins = new Margins(100, 10, 10);

        var exception = Assert.Throws<ArgumentException>(
            () => UtilVolumeBox.CreatePrintVolumeAndUtilBoxes(printVolume, margins));

        Assert.Contains("Axis = X", exception.Message);
        Assert.DoesNotContain("Axis = Y", exception.Message);
        Assert.DoesNotContain("Axis = Z", exception.Message);
    }

    [Fact]
    public void Create_WhenMarginsAreBiggerThanSizes_ThrowsErrorOnEveryAxis()
    {
        var printVolume = new PrintVolume(100, 100, 100);
        var margins = new Margins(150, 150, 150);

        var exception = Assert.Throws<ArgumentException>(
            () => UtilVolumeBox.CreatePrintVolumeAndUtilBoxes(printVolume, margins));

        Assert.Contains("Axis = X", exception.Message);
        Assert.Contains("Axis = Y", exception.Message);
        Assert.Contains("Axis = Z", exception.Message);
    }
}