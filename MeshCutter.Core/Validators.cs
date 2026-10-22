namespace MeshCutter.Core;

public enum Axis 
{
    X = 1, Y = 2, Z = 3
}

public enum KindOfError
{
    SizeNotPositive = 1, MarginNegative = 2, MarginTooBig = 3, SizeNotANumber = 4, MarginNotANumber = 5
}

 public record SpaceError(Axis Axis, KindOfError KindOfError)
{
    
}

public static class SpaceValidator
{
    public static List<SpaceError> Validate(double sizeX, double sizeY, double sizeZ, double marginX, double marginY, double marginZ)
    {
        var errors = new List<SpaceError>();

        errors.AddRange((GetErrorsOnOneAxe(Axis.X, sizeX, marginX)));
        errors.AddRange((GetErrorsOnOneAxe(Axis.Y, sizeY, marginY)));
        errors.AddRange((GetErrorsOnOneAxe(Axis.Z, sizeZ, marginZ)));

        return errors;
    }

    private static List<SpaceError> GetErrorsOnOneAxe(Axis axis, double size, double margin)
    {
        var errors = new List<SpaceError>();

        if (!double.IsFinite(size))
            errors.Add(new SpaceError(axis, KindOfError.SizeNotANumber));
        else if (size <= 0)
            errors.Add(new SpaceError(axis, KindOfError.SizeNotPositive));


        if (!double.IsFinite(margin))
            errors.Add(new SpaceError(axis, KindOfError.MarginNotANumber));
        else if (margin < 0)
            errors.Add(new SpaceError(axis, KindOfError.MarginNegative));


        if (double.IsFinite(size) && double.IsFinite(margin) && (size > 0))
            if (size - margin <= 0)
                errors.Add(new SpaceError(axis, KindOfError.MarginTooBig));
        
        return errors;
    }

}