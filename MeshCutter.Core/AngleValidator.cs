namespace MeshCutter.Core;

public enum AngleError
{
    AngleNotADegree = 1, MeshIsNotClosed = 2
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
            return AngleError.AngleNotADegree;
        }
        return null;
    }
}

