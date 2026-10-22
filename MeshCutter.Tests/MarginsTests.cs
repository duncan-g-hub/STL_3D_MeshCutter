using MeshCutter.Core;

namespace MeshCutter.Tests;

public class MarginsTests
{
    [Fact]
    public void Margins_WhenMarginXIsUnderZero_SetsError()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Margins(-4, 20, 20)
        );
        Assert.Equal("x axis", exception.ParamName);
    }

    [Fact]
    public void Margins_WhenMarginYIsUnderZero_SetsError()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Margins(20, -4, 20)
        );
        Assert.Equal("y axis", exception.ParamName);
    }

    [Fact]
    public void Margins_WhenMarginZIsUnderZero_SetsError()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Margins(20, 20, -4)
        );
        Assert.Equal("z axis", exception.ParamName);
    }

    [Fact]
    public void Margins_WhenMarginIsEqualZero_CreatesMargins()
    {
        var margins = new Margins(0, 0, 0);
        Assert.Equal(0, margins.X);
        Assert.Equal(0, margins.Y);
        Assert.Equal(0, margins.Z);
    }

    [Fact]
    public void Margins_WhenAllMarginsArePositive_CreatesMargins()
    {
        var margins = new Margins(10, 20, 30);
        Assert.Equal(10, margins.X);
        Assert.Equal(20, margins.Y);
        Assert.Equal(30, margins.Z);
    }


    private static readonly double[] NotFiniteValues =
    {
        double.NaN, double.PositiveInfinity, double.NegativeInfinity
    };

    [Fact]
    public void Margins_WhenMarginXIsNotFinite_SetsError()
    {
        foreach (double bad in NotFiniteValues)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new Margins(bad, 20, 20)
            );
            Assert.Equal("x axis", exception.ParamName);
        }
    }

    [Fact]
    public void Margins_WhenMarginYIsNotFinite_SetsError()
    {
        foreach (double bad in NotFiniteValues)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new Margins(20, bad, 20)
            );
            Assert.Equal("y axis", exception.ParamName);
        }
    }

    [Fact]
    public void Margins_WhenMarginZIsNotFinite_SetsError()
    {
        foreach (double bad in NotFiniteValues)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new Margins(20, 20, bad)
            );
            Assert.Equal("z axis", exception.ParamName);
        }
    }
}