using MeshCutter.Core;

namespace MeshCutter.App;

public static class AxisFormatter
{
    public static string FormatAxis(Axis axis)
    {
        return axis switch
        {
            Axis.X => "Axe X",
            Axis.Y => "Axe Y",
            Axis.Z => "Axe Z",
            _ => throw new ArgumentOutOfRangeException(nameof(axis))
        };
    }
}