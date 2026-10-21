namespace MeshCutter.Core;

public enum Axis 
{
    X = 1, Y = 2, Z = 3
}

public enum KindOfError
{
    SizeNotPositive = 1, MarginNegative = 2, MarginTooBig = 3
}

 public record SpaceError(Axis Axis, KindOfError KindOfError)
{
    
}

public static class SpaceValidator
{
    public static List<SpaceError> Validate(double sizeX, double sizeY, double sizeZ, double marginX, double marginY, double marginZ)
    {
        var errors = new List<SpaceError>();

        if (sizeX <= 0)
            errors.Add(new SpaceError(Axis.X, KindOfError.SizeNotPositive));
        else if (sizeX - marginX <= 0)
            errors.Add(new SpaceError(Axis.X, KindOfError.MarginTooBig));
        if (marginX < 0)
            errors.Add(new SpaceError(Axis.X, KindOfError.MarginNegative));

        if (sizeY <= 0)
            errors.Add(new SpaceError(Axis.Y, KindOfError.SizeNotPositive));
        else if (sizeY - marginY <= 0)
            errors.Add(new SpaceError(Axis.Y, KindOfError.MarginTooBig));
        if (marginY < 0)
            errors.Add(new SpaceError(Axis.Y, KindOfError.MarginNegative));

        if (sizeZ <= 0)
            errors.Add(new SpaceError(Axis.Z, KindOfError.SizeNotPositive));
        else if (sizeZ - marginZ <= 0)
            errors.Add(new SpaceError(Axis.Z, KindOfError.MarginTooBig));
        if (marginZ < 0)
            errors.Add(new SpaceError(Axis.Z, KindOfError.MarginNegative));

        return errors;
    }

}