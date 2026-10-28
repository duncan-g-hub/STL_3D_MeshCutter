using MeshCutter.Core;

namespace MeshCutter.Tests;

public class SlicerValidatorTests
{
    [Fact]
    public void Validate_WhenCoordinateIsExactlyAtLowerLimit_ReturnsNull()
    {
        var error = SlicerValidator.Validate(0, 50, 0.5, 0.5);
        Assert.Null(error);
    }

    [Fact]
    public void Validate_WhenCoordinateIsExactlyAtUpperLimit_ReturnsNull()
    {
        var error = SlicerValidator.Validate(0, 50, 49.5, 0.5);
        Assert.Null(error);
    }

    [Fact]
    public void Validate_WhenCoordinateAndMinThicknessValid_ReturnsNull()
    {
        var error = SlicerValidator.Validate(0, 50, 25, 0.5);
        Assert.Null(error);
    }




    [Fact]
    public void Validate_WhenCoordinateBiggerThanUpperLimit_ReturnsError()
    {
        var error = SlicerValidator.Validate(0, 50, 49.6, 0.5);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.TooHighSliceCoordinate, error);
    }

    [Fact]
    public void Validate_WhenCoordinateLowerThanLowerLimit_ReturnsError()
    {
        var error = SlicerValidator.Validate(0, 50, 0.4, 0.5);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.TooLowSliceCoordinate, error);
    }





    [Fact]
    public void Validate_WhenCoordinateBiggerThanMeshMax_ReturnsError()
    {
        var error = SlicerValidator.Validate(0, 50, 51, 0.5);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.TooHighSliceCoordinate, error);
    }

    [Fact]
    public void Validate_WhenCoordinateLowerThanMeshMin_ReturnsError()
    {
        var error = SlicerValidator.Validate(0, 50, -1, 0.5);
        Assert.NotNull(error);
        Assert.Equal(SlicerError.TooLowSliceCoordinate, error);
    }


    
    private static readonly double[] NotFiniteValues =
        {
            double.NaN, double.PositiveInfinity, double.NegativeInfinity
        };


    [Fact]
    public void Validate_WhenCoordinateNotFinite_ReturnsError()
    {
        foreach (double bad in NotFiniteValues)
        {   
            var error = SlicerValidator.Validate(0, 50, bad, 0.5);
            Assert.NotNull(error);
            Assert.Equal(SlicerError.CoordinateNotANumber, error);
        }
    }

    [Fact]
    public void Validate_WhenMinThicknessNotFinite_ReturnsError()
    {
        foreach (double bad in NotFiniteValues)
        {  
            var error = SlicerValidator.Validate(0, 50, 25, bad);
            Assert.NotNull(error);
            Assert.Equal(SlicerError.MinThicknessNotANumber, error);
        }
    }


    
}
