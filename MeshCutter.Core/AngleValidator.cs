namespace MeshCutter.Core;

public enum AngleError
{
    AngleNotANumber = 1, MeshIsNotClosed = 2
}

public static class AngleValidator
{
    public static AngleError? Validate(double angle, bool closedMesh)
    {
        if (!closedMesh)
        {
            return AngleError.MeshIsNotClosed;
        }
        if (!double.IsFinite(angle))
        {
            return AngleError.AngleNotANumber;
        }
        return null;
    }
}

