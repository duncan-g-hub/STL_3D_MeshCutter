using MeshCutter.Core;

namespace MeshCutter.Tests;

public class SpaceValidatorsTests
{

    [Fact]
    public void Validate_WhenSizeXIsZero_ReturnsOneErrorOnX()
    {
        var errors = SpaceValidator.Validate(0, 20, 20, 10, 10, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.X, KindOfError.SizeNotPositive), error);
    }

    [Fact]
    public void Validate_WhenSizeXIsUnderZero_ReturnsOneErrorOnX()
    {
        var errors = SpaceValidator.Validate(-10, 20, 20, 10, 10, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.X, KindOfError.SizeNotPositive), error);
    }

    [Fact]
    public void Validate_WhenMarginXEqualZero_ReturnsEmptyList()
    {
        Assert.Empty(SpaceValidator.Validate(20, 20, 20, 0, 10, 10));
    }

    [Fact]
    public void Validate_WhenMarginXIsUnderZero_ReturnsOneErrorOnX()
    {
        var errors = SpaceValidator.Validate(20, 20, 20, -10, 10, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.X, KindOfError.MarginNegative), error);
    }

    [Fact]
    public void Validate_WhenMarginXIsBiggerThanSizeX_ReturnsOneErrorOnX()
    {
        var errors = SpaceValidator.Validate(20, 20, 20, 30, 10, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.X, KindOfError.MarginTooBig), error);
    }

    [Fact]
    public void Validate_WhenMarginXEqualSizeX_ReturnsOneErrorOnX()
    {
        var errors = SpaceValidator.Validate(20, 20, 20, 20, 10, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.X, KindOfError.MarginTooBig), error);
    }

    [Fact]
    public void Validate_WhenMarginsXAndSizeXEqualZero_ReturnsOneErrorOnX()
    {
        var errors = SpaceValidator.Validate(0, 20, 20, 0, 10, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.X, KindOfError.SizeNotPositive), error);
    }

    [Fact]
    public void Validate_WhenMarginsXUnderZeroAndSizeXEqualZero_ReturnsTwoErrorsOnX()
    {
        var errors = SpaceValidator.Validate(0, 20, 20, -10, 10, 10);
        Assert.Equal(2, errors.Count);
        Assert.Equal(new SpaceError(Axis.X, KindOfError.SizeNotPositive), errors[0]);
        Assert.Equal(new SpaceError(Axis.X, KindOfError.MarginNegative), errors[1]);
    }






    [Fact]
    public void Validate_WhenSizeYIsZero_ReturnsOneErrorOnY()
    {
        var errors = SpaceValidator.Validate(20, 0, 20, 10, 10, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.Y, KindOfError.SizeNotPositive), error);
    }

    [Fact]
    public void Validate_WhenSizeYIsUnderZero_ReturnsOneErrorOnY()
    {
        var errors = SpaceValidator.Validate(20, -10, 20, 10, 10, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.Y, KindOfError.SizeNotPositive), error);
    }

    [Fact]
    public void Validate_WhenMarginYEqualZero_ReturnsEmptyList()
    {
        Assert.Empty(SpaceValidator.Validate(20, 20, 20, 10, 0, 10));
    }

    [Fact]
    public void Validate_WhenMarginYIsUnderZero_ReturnsOneErrorOnY()
    {
        var errors = SpaceValidator.Validate(20, 20, 20, 10, -10, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.Y, KindOfError.MarginNegative), error);
    }

    [Fact]
    public void Validate_WhenMarginYIsBiggerThanSizeY_ReturnsOneErrorOnY()
    {
        var errors = SpaceValidator.Validate(20, 20, 20, 10, 30, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.Y, KindOfError.MarginTooBig), error);
    }

    [Fact]
    public void Validate_WhenMarginYEqualSizeY_ReturnsOneErrorOnY()
    {
        var errors = SpaceValidator.Validate(20, 20, 20, 10, 20, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.Y, KindOfError.MarginTooBig), error);
    }

    [Fact]
    public void Validate_WhenMarginsYAndSizeYEqualZero_ReturnsOneErrorOnY()
    {
        var errors = SpaceValidator.Validate(20, 0, 20, 10, 0, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.Y, KindOfError.SizeNotPositive), error);
    }

    [Fact]
    public void Validate_WhenMarginsYUnderZeroAndSizeYEqualZero_ReturnsTwoErrorsOnY()
    {
        var errors = SpaceValidator.Validate(20, 0, 20, 10, -10, 10);
        Assert.Equal(2, errors.Count);
        Assert.Equal(new SpaceError(Axis.Y, KindOfError.SizeNotPositive), errors[0]);
        Assert.Equal(new SpaceError(Axis.Y, KindOfError.MarginNegative), errors[1]);
    }




    [Fact]
    public void Validate_WhenSizeZIsZero_ReturnsOneErrorOnZ()
    {
        var errors = SpaceValidator.Validate(20, 20, 0, 10, 10, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.Z, KindOfError.SizeNotPositive), error);
    }

    [Fact]
    public void Validate_WhenSizeZIsUnderZero_ReturnsOneErrorOnZ()
    {
        var errors = SpaceValidator.Validate(20, 20, -10, 10, 10, 10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.Z, KindOfError.SizeNotPositive), error);
    }

    [Fact]
    public void Validate_WhenMarginZEqualZero_ReturnsEmptyList()
    {
        Assert.Empty(SpaceValidator.Validate(20, 20, 20, 10, 10, 0));
    }

    [Fact]
    public void Validate_WhenMarginZIsUnderZero_ReturnsOneErrorOnZ()
    {
        var errors = SpaceValidator.Validate(20, 20, 20, 10, 10, -10);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.Z, KindOfError.MarginNegative), error);
    }

    [Fact]
    public void Validate_WhenMarginZIsBiggerThanSizeZ_ReturnsOneErrorOnZ()
    {
        var errors = SpaceValidator.Validate(20, 20, 20, 10, 10, 30);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.Z, KindOfError.MarginTooBig), error);
    }

    [Fact]
    public void Validate_WhenMarginZEqualSizeZ_ReturnsOneErrorOnZ()
    {
        var errors = SpaceValidator.Validate(20, 20, 20, 10, 10, 20);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.Z, KindOfError.MarginTooBig), error);
    }

    [Fact]
    public void Validate_WhenMarginsZAndSizeZEqualZero_ReturnsOneErrorOnZ()
    {
        var errors = SpaceValidator.Validate(20, 20, 0, 10, 10, 0);
        var error = Assert.Single(errors);
        Assert.Equal(new SpaceError(Axis.Z, KindOfError.SizeNotPositive), error);
    }

    [Fact]
    public void Validate_WhenMarginsZUnderZeroAndSizeZEqualZero_ReturnsTwoErrorsOnZ()
    {
        var errors = SpaceValidator.Validate(20, 20, 0, 10, 10, -10);
        Assert.Equal(2, errors.Count);
        Assert.Equal(new SpaceError(Axis.Z, KindOfError.SizeNotPositive), errors[0]);
        Assert.Equal(new SpaceError(Axis.Z, KindOfError.MarginNegative), errors[1]);
    }




    [Fact]
    public void Validate_WhenAllSizesAreZero_ReturnsThreeErrors()
    {
        var errors = SpaceValidator.Validate(0, 0, 0, 10, 10, 10);
        Assert.Equal(new List<SpaceError>
            {
                new(Axis.X, KindOfError.SizeNotPositive),
                new(Axis.Y, KindOfError.SizeNotPositive),
                new(Axis.Z, KindOfError.SizeNotPositive),
            },errors);
    }

    [Fact]
    public void Validate_WhenAllMarginsAreUnderZero_ReturnsThreeErrors()
    {
        var errors = SpaceValidator.Validate(20, 20, 20, -10, -10, -10);
        Assert.Equal(new List<SpaceError>
            {
                new(Axis.X, KindOfError.MarginNegative),
                new(Axis.Y, KindOfError.MarginNegative),
                new(Axis.Z, KindOfError.MarginNegative),
            },errors);
    }

    [Fact]
    public void Validate_WhenMarginAreBiggerThanSizes_ReturnsThreeErrors()
    {
        var errors = SpaceValidator.Validate(20, 20, 20, 30, 30, 30);
        Assert.Equal(new List<SpaceError>
            {
                new(Axis.X, KindOfError.MarginTooBig),
                new(Axis.Y, KindOfError.MarginTooBig),
                new(Axis.Z, KindOfError.MarginTooBig),
            },errors);
    }

    [Fact]
    public void Validate_WhenValid_ReturnsEmptyList()
    {
        Assert.Empty(SpaceValidator.Validate(20, 20, 20, 10, 10, 10));
    }


    private static readonly double[] NotFiniteValues =
    {
        double.NaN, double.PositiveInfinity, double.NegativeInfinity
    };

    [Fact]
    public void Validate_WhenSizeXIsNotFinite_ReturnsOnlySizeNotANumberOnX()
    {
        foreach (double bad in NotFiniteValues)
        {
            var errors = SpaceValidator.Validate(bad, 20, 20, 10, 10, 10);
            var error = Assert.Single(errors);
            Assert.Equal(new SpaceError(Axis.X, KindOfError.SizeNotANumber), error);
        }
    }

    [Fact]
    public void Validate_WhenSizeYIsNotFinite_ReturnsOnlySizeNotANumberOnY()
    {
        foreach (double bad in NotFiniteValues)
        {
            var errors = SpaceValidator.Validate(20, bad, 20, 10, 10, 10);
            var error = Assert.Single(errors);
            Assert.Equal(new SpaceError(Axis.Y, KindOfError.SizeNotANumber), error);
        }
    }

    [Fact]
    public void Validate_WhenSizeZIsNotFinite_ReturnsOnlySizeNotANumberOnZ()
    {
        foreach (double bad in NotFiniteValues)
        {
            var errors = SpaceValidator.Validate(20, 20, bad, 10, 10, 10);
            var error = Assert.Single(errors);
            Assert.Equal(new SpaceError(Axis.Z, KindOfError.SizeNotANumber), error);
        }
    }

    [Fact]
    public void Validate_WhenMarginXIsNotFinite_ReturnsOnlyMarginNotANumberOnX()
    {
        foreach (double bad in NotFiniteValues)
        {
            var errors = SpaceValidator.Validate(20, 20, 20, bad, 10, 10);
            var error = Assert.Single(errors);
            Assert.Equal(new SpaceError(Axis.X, KindOfError.MarginNotANumber), error);
        }
    }

    [Fact]
    public void Validate_WhenMarginYIsNotFinite_ReturnsOnlyMarginNotANumberOnY()
    {
        foreach (double bad in NotFiniteValues)
        {
            var errors = SpaceValidator.Validate(20, 20, 20, 10, bad, 10);
            var error = Assert.Single(errors);
            Assert.Equal(new SpaceError(Axis.Y, KindOfError.MarginNotANumber), error);
        }
    }

    [Fact]
    public void Validate_WhenMarginZIsNotFinite_ReturnsOnlyMarginNotANumberOnZ()
    {
        foreach (double bad in NotFiniteValues)
        {
            var errors = SpaceValidator.Validate(20, 20, 20, 10, 10, bad);
            var error = Assert.Single(errors);
            Assert.Equal(new SpaceError(Axis.Z, KindOfError.MarginNotANumber), error);
        }
    }

    [Fact]
    public void Validate_WhenSizeAndMarginXAreNaN_ReturnsBothErrorsInOrder()
    {
        var errors = SpaceValidator.Validate(double.NaN, 20, 20, double.NaN, 10, 10);

        Assert.Equal(
            new List<SpaceError>
            {
                new(Axis.X, KindOfError.SizeNotANumber),
                new(Axis.X, KindOfError.MarginNotANumber),
            },
            errors);
    }

    [Fact]
    public void Validate_WhenDifferentErrorsOnEachAxis_ReturnsThemInAxisOrder()
    {
        var errors = SpaceValidator.Validate(double.NaN, 0, 20, 10, 10, -5);

        Assert.Equal(
            new List<SpaceError>
            {
                new(Axis.X, KindOfError.SizeNotANumber),
                new(Axis.Y, KindOfError.SizeNotPositive),
                new(Axis.Z, KindOfError.MarginNegative),
            },
            errors);
}
}