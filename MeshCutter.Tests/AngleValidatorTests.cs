using MeshCutter.Core;

namespace MeshCutter.Tests;

public class AngleValidatorTests
{
    private static readonly double[] NotFiniteValues =
    {
        double.NaN, double.PositiveInfinity, double.NegativeInfinity
    };

    [Fact]
    public void Validate_WhenAngleIsFiniteAndMeshIsClosed_ReturnsNull()
    {
        var error = AngleValidator.Validate(90, true);
        Assert.Null(error);
    }

    [Fact]
    public void Validate_WhenAngleIsZero_ReturnsNull()
    {
        var error = AngleValidator.Validate(0, true);
        Assert.Null(error);
    }

    [Fact]
    public void Validate_WhenAngleIsOutsideOneTurn_ReturnsNull()
    {
        foreach (double angle in new[] { -45.0, 270.0, 720.0 })
        {
            var error = AngleValidator.Validate(angle, true);
            Assert.Null(error);
        }
    }

    [Fact]
    public void Validate_WhenAngleIsNotFinite_ReturnsError()
    {
        foreach (double bad in NotFiniteValues)
        {
            var error = AngleValidator.Validate(bad, true);
            Assert.NotNull(error);
            Assert.Equal(AngleError.AngleNotANumber, error);
        }
    }

    [Fact]
    public void Validate_WhenMeshIsNotClosed_ReturnsError()
    {
        var error = AngleValidator.Validate(90, false);
        Assert.NotNull(error);
        Assert.Equal(AngleError.MeshIsNotClosed, error);
    }

    [Fact]
    public void Validate_WhenMeshIsNotClosedAndAngleIsNotFinite_ReturnsMeshError()
    {
        foreach (double bad in NotFiniteValues)
        {
            var error = AngleValidator.Validate(bad, false);
            Assert.Equal(AngleError.MeshIsNotClosed, error);
        }
    }
}