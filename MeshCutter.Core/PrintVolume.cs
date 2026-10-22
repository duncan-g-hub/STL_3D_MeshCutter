namespace MeshCutter.Core;
public class PrintVolume
{
    public double SizeX {get;}
    public double SizeY {get;}
    public double SizeZ {get;}

    private static void ControlValue(double value, string name)
    { 
        if (!double.IsFinite(value) || value <= 0)
            throw new ArgumentOutOfRangeException(name, $"The print volume on {name} must be greater than 0.");
            
    } 


    public PrintVolume(double sizeX, double sizeY, double sizeZ)
    {
        ControlValue(sizeX, "x axis");
        SizeX = sizeX;
        ControlValue(sizeY, "y axis");
        SizeY = sizeY;
        ControlValue(sizeZ, "z axis");
        SizeZ = sizeZ;
    }

    public double Volume => SizeX * SizeY * SizeZ;

}