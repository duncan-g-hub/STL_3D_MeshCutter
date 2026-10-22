namespace MeshCutter.Core;
public class Margins
{
    public double X {get;}
    public double Y {get;}
    public double Z {get;} 

    public Margins(double x, double y, double z)
    {
        ControlValue(x, "x axis");
        X = x;
        ControlValue(y, "y axis");
        Y = y;
        ControlValue(z, "z axis");
        Z = z;
    }

    private static void ControlValue(double value, string name)
    {
        if (!double.IsFinite(value) || value < 0)
            throw new ArgumentOutOfRangeException(name, $"The margin on {name} must be greater than or equal to 0.");
    }

}