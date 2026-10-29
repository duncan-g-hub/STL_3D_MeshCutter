using MeshCutter.Core;
using g3;

namespace MeshCutter.Tests;

public class SlicerValidatorTests
{
    [Fact]
    public void Validate_WhenMeshIsNotClosed_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 30, 40, 50);
        var origin = new Vector3d(0, 0, 25);
        var normal = new Vector3d(0, 0, 1);

        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, false);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.MeshIsNotClosed, error);
    }


    [Fact]
    public void Validate_WhenCoordinateIsExactlyAtLowerLimit_ReturnsNull()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 30, 40, 50);
        var origin = new Vector3d(0, 0, 0.5);
        var normal = new Vector3d(0, 0, 1);

        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.Null(error);
    }

    [Fact]
    public void Validate_WhenCoordinateIsExactlyAtUpperLimit_ReturnsNull()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 30, 40, 50);
        var origin = new Vector3d(0, 0, 49.5);
        var normal = new Vector3d(0, 0, 1);

        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.Null(error);
    }

    [Fact]
    public void Validate_WhenCoordinateOnZAndMinThicknessValid_ReturnsNull()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 30, 40, 50);
        var origin = new Vector3d(0, 0, 45);
        var normal = new Vector3d(0, 0, 1);

        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.Null(error);
    }

    [Fact]
    public void Validate_WhenCoordinateOnYAndMinThicknessValid_ReturnsNull()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 40, 50, 30);
        var origin = new Vector3d(0, 45, 0);
        var normal = new Vector3d(0, 1, 0);

        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.Null(error);
    }

    [Fact]
    public void Validate_WhenCoordinateOnXAndMinThicknessValid_ReturnsNull()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 50, 40, 30);
        var origin = new Vector3d(45, 0, 0);
        var normal = new Vector3d(1, 0, 0);

        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.Null(error);
    }


    [Fact]
    public void Validate_WhenCoordinateBiggerThanUpperLimit_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 30, 40, 50);
        var origin = new Vector3d(0, 0, 49.6);
        var normal = new Vector3d(0, 0, 1);

        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.TooHighSliceCoordinate, error);
    }

    [Fact]
    public void Validate_WhenCoordinateLowerThanLowerLimit_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 30, 40, 50);
        var origin = new Vector3d(0, 0, 0.4);
        var normal = new Vector3d(0, 0, 1);
        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.TooLowSliceCoordinate, error);
    }

    [Fact]
    public void Validate_WhenCoordinateBiggerThanMeshMaxOnZ_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(20, 10, 0, 50, 40, 30);
        var origin = new Vector3d(0, 0, 31);
        var normal = new Vector3d(0, 0, 1);
        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.TooHighSliceCoordinate, error);
    }

    [Fact]
    public void Validate_WhenCoordinateLowerThanMeshMinOnZ_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(10, 20, 30, 30, 40, 50);
        var origin = new Vector3d(0, 0, 29);
        var normal = new Vector3d(0, 0, 1);
        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.TooLowSliceCoordinate, error);
    }

    [Fact]
    public void Validate_WhenCoordinateBiggerThanMeshMaxOnY_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 50, 30, 40);
        var origin = new Vector3d(0, 31, 0);
        var normal = new Vector3d(0, 1, 0);
        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.TooHighSliceCoordinate, error);
    }

    [Fact]
    public void Validate_WhenCoordinateLowerThanMeshMinOnY_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(10, 30, 20, 30, 50, 40);
        var origin = new Vector3d(0, 29, 0);
        var normal = new Vector3d(0, 1, 0);
        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.TooLowSliceCoordinate, error);
    }

    [Fact]
    public void Validate_WhenCoordinateBiggerThanMeshMaxOnX_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 30, 50, 40);
        var origin = new Vector3d(31, 0, 0);
        var normal = new Vector3d(1, 0, 0);
        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.TooHighSliceCoordinate, error);
    }

    [Fact]
    public void Validate_WhenCoordinateLowerThanMeshMinOnX_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(30, 10, 20, 50, 30, 40);
        var origin = new Vector3d(29, 0, 0);
        var normal = new Vector3d(1, 0, 0);
        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.TooLowSliceCoordinate, error);
    }

    [Fact]
    public void Validate_WhenNormalIsZero_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 30, 40, 50);
        var origin = new Vector3d(0, 0, 25);
        var normal = new Vector3d(0, 0, 0);
        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.NormalIsZero, error);
    }

    [Fact]
    public void Validate_WhenNormalIsNegative_ReturnsNull()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 30, 40, 50);
        var origin = new Vector3d(0, 0, 25);
        var normal = new Vector3d(0, 0, -1);
        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.Null(error);
    }

    [Fact]
    public void Validate_WhenNormalIsAngledOnTwoAxis_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 30, 40, 50);
        var origin = new Vector3d(0, 0, 25);
        var normal = new Vector3d(0, 1, 1);
        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.NormalNotAligned, error);
    }

    [Fact]
    public void Validate_WhenNormalIsAngledOnThreeAxis_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 30, 40, 50);
        var origin = new Vector3d(0, 0, 25);
        var normal = new Vector3d(1, 1, 1);
        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.NormalNotAligned, error);
    }

    [Fact]
    public void Validate_WhenNormalIsAlmostAligned_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 30, 40, 50);
        var origin = new Vector3d(0, 0, 25);
        var normal = new Vector3d(0, 0.000000001, 1);
        var error = SlicerValidator.Validate(origin, normal, bounds, 0.5, true);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.NormalNotAligned, error);
    }

    
    private static readonly double[] NotFiniteValues =
        {
            double.NaN, double.PositiveInfinity, double.NegativeInfinity
        };


    [Fact]
    public void Validate_WhenOriginNotFiniteOnXOrYOrZ_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 50, 50, 50);
        var normal = new Vector3d(0, 0, 1);

        foreach (double bad in NotFiniteValues)
        {
            Assert.Equal(SlicerError.OriginNotAVector,
                SlicerValidator.Validate(new Vector3d(bad, 0, 25), normal, bounds, 0.5, true));
            Assert.Equal(SlicerError.OriginNotAVector,
                SlicerValidator.Validate(new Vector3d(0, bad, 25), normal, bounds, 0.5, true));
            Assert.Equal(SlicerError.OriginNotAVector,
                SlicerValidator.Validate(new Vector3d(0, 25, bad), normal, bounds, 0.5, true));

        }
    }

    [Fact]
    public void Validate_WhenNormalNotFiniteOnXOrYOrZ_ReturnsError()
    {
        var bounds = new AxisAlignedBox3d(0, 0, 0, 50, 50, 50);
        var origin = new Vector3d(0, 0, 25);

        foreach (double bad in NotFiniteValues)
        {
            Assert.Equal(SlicerError.NormalNotAVector,
                SlicerValidator.Validate(origin, new Vector3d(bad, 0, 1), bounds, 0.5, true));
            Assert.Equal(SlicerError.NormalNotAVector,
                SlicerValidator.Validate(origin, new Vector3d(0, bad, 1), bounds, 0.5, true));
            Assert.Equal(SlicerError.NormalNotAVector,
                SlicerValidator.Validate(origin, new Vector3d(0, 1, bad), bounds, 0.5, true));
        }
    }

    [Fact]
    public void Validate_WhenMinThicknessNotFinite_ReturnsError()
    {
        foreach (double bad in NotFiniteValues)
        {  
            var bounds = new AxisAlignedBox3d(0, 0, 0, 30, 40, 50);
            var origin = new Vector3d(0, 0, 25);
            var normal = new Vector3d(0, 0, 1);
            var error = SlicerValidator.Validate(origin, normal, bounds, bad, true);
            Assert.NotNull(error);
            Assert.Equal(SlicerError.MinThicknessNotANumber, error);
        }
    }


    
}
