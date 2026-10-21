using MeshCutter.Core;

namespace MeshCutter.Tests;

public class PrintVolumeTests
{
    [Fact]
    public void PrintVolume_WhenPrintVolumeXIsUnderZero_SetsError()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new PrintVolume(-4, 200, 200)
        );
        Assert.Equal("x axis", exception.ParamName);
    }

    [Fact]
    public void PrintVolume_WhenPrintVolumeYIsUnderZero_SetsError()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new PrintVolume(200, -4, 200)
        );
        Assert.Equal("y axis", exception.ParamName);
    }

    [Fact]
    public void PrintVolume_WhenPrintVolumeZIsUnderZero_SetsError()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new PrintVolume(200, 200, -4)
        );
        Assert.Equal("z axis", exception.ParamName);
    }

    [Fact]
    public void PrintVolume_WhenPrintVolumeXIsEqualZero_SetsError()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new PrintVolume(0, 200, 200)
        );
        Assert.Equal("x axis", exception.ParamName);
    }

    [Fact]
    public void PrintVolume_WhenPrintVolumeYIsEqualZero_SetsError()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new PrintVolume(200, 0, 200)
        );
        Assert.Equal("y axis", exception.ParamName);
    }

    [Fact]
    public void PrintVolume_WhenPrintVolumeZIsEqualZero_SetsError()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new PrintVolume(200, 200, 0)
        );
        Assert.Equal("z axis", exception.ParamName);
    }

    [Fact]
    public void PrintVolume_WhenAllAxesArePositive_CreatesPrintVolume()
    {
        var printVolume = new PrintVolume(100, 200, 300);
        Assert.Equal(100, printVolume.SizeX);
        Assert.Equal(200, printVolume.SizeY);
        Assert.Equal(300, printVolume.SizeZ);
    }
}