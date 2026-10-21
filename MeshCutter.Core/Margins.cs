namespace MeshCutter.Core;
public class Margins
{
    public double X {get;}
    public double Y {get;}
    public double Z {get;} 

    public Margins(double x, double y, double z)
    {
        ControlValue(x, nameof(X));
        X = x;
        ControlValue(y, nameof(Y));
        Y = y;
        ControlValue(z, nameof(Z));
        Z = z;
    }

    private static void ControlValue(double value, string name)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(name.ToLower(), $"The margin on {name.ToLower()} must be greater than or equal to 0.");
    }

}