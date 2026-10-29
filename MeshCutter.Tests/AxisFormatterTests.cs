using MeshCutter.App;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public class AxisFormatterTests
{
    [Fact]
    public void Format_WhenWrongAxis_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => AxisFormatter.FormatAxis((Axis)999));
        Assert.Equal("axis", exception.ParamName);
    }

    [Fact]
    public void Format_WhenAxisX_ReturnsValidAxisString()
    {
        var axisString = AxisFormatter.FormatAxis(Axis.X);
        Assert.Equal("Axe X", axisString);
    }

    [Fact]
    public void Format_WhenAxisY_ReturnsValidAxisString()
    {
        var axisString = AxisFormatter.FormatAxis(Axis.Y);
        Assert.Equal("Axe Y", axisString);
    }

    [Fact]
    public void Format_WhenAxisZ_ReturnsValidAxisString()
    {
        var axisString = AxisFormatter.FormatAxis(Axis.Z);
        Assert.Equal("Axe Z", axisString);
    }
}