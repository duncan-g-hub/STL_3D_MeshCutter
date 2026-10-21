namespace MeshCutter.Core;
public class PrintVolume
{
    public double SizeX {get;}
    public double SizeY {get;}
    public double SizeZ {get;}

    private static void ControlValue(double value, string name)
    { 
        if (value <= 0)
            throw new ArgumentOutOfRangeException(name.ToLower(), $"The print volume on {name.ToLower()} must be greater than 0.");
            
    } 


    public PrintVolume(double sizeX, double sizeY, double sizeZ)
    {
        ControlValue(sizeX, nameof(SizeX));
        SizeX = sizeX;
        ControlValue(sizeY, nameof(SizeY));
        SizeY = sizeY;
        ControlValue(sizeZ, nameof(SizeZ));
        SizeZ = sizeZ;
    }

    public double Volume => SizeX * SizeY * SizeZ;

}