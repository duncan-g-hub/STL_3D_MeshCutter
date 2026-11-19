using g3;

namespace MeshCutter.Core;


public static class UtilVolumeBox
{
    public static (AxisAlignedBox3d printVolumeBox, AxisAlignedBox3d utilBox) CreatePrintVolumeAndUtilBoxes(PrintVolume printVolume, Margins margins)
    {
        var errors = SpaceValidator.Validate(printVolume.SizeX, printVolume.SizeY, printVolume.SizeZ, margins.X, margins.Y, margins.Z);
        if (errors.Count > 0)
        {
            throw new ArgumentException($"Invalid configuration: {string.Join(", ", errors)}");
        }
        var printVolumeBox = new AxisAlignedBox3d(-printVolume.SizeX/2, -printVolume.SizeY/2, 0, printVolume.SizeX/2, printVolume.SizeY/2, printVolume.SizeZ);
        var utilBox = new AxisAlignedBox3d(-(printVolume.SizeX -margins.X)/2, -(printVolume.SizeY-margins.Y)/2, 0, (printVolume.SizeX -margins.X)/2, (printVolume.SizeY -margins.Y)/2, printVolume.SizeZ -margins.Z);
        return (printVolumeBox, utilBox);
    }
}