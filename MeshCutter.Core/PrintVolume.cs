namespace MeshCutter.Core;
public class PrintVolume
{
    private double _sizeX;
    public double SizeX 
    { 
        get{ return _sizeX; } 
        set{ ControlValue(value, nameof(SizeX)); _sizeX = value; } 
    }

    private double _sizeY;
    public double SizeY 
    { 
        get{ return _sizeY; } 
        set{ ControlValue(value, nameof(SizeY)); _sizeY = value; } 
    }

    private double _sizeZ;
    public double SizeZ 
    { 
        get{ return _sizeZ; } 
        set{ ControlValue(value, nameof(SizeZ)); _sizeZ = value; } 
    }

    private static void ControlValue(double value, string name)
    { 
        if (value <= 0)
            throw new ArgumentOutOfRangeException(name, $"{name} doit être supérieur à 0.");
    } 


    public PrintVolume(double sizeX, double sizeY, double sizeZ)
    {
        SizeX = sizeX;
        SizeY = sizeY;
        SizeZ = sizeZ;
    }

    public double Volume => SizeX * SizeY * SizeZ;

}