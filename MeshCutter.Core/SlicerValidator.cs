using g3;

namespace MeshCutter.Core;


public enum SlicerError
{
    TooLowSliceCoordinate = 1, TooHighSliceCoordinate = 2, NormalNotAVector = 3, OriginNotAVector= 4, MinThicknessNotANumber = 5,
    NormalNotAligned = 6, NormalIsZero = 7, MeshIsNotClosed = 8
}

public static class SlicerValidator
{
    public static SlicerError? Validate(Vector3d origin, Vector3d normal, AxisAlignedBox3d bounds, double minThickness, bool closedMesh)
    {
        double coordinate = 0;
        double meshMax = 0;
        double meshMin = 0;

        if (!closedMesh)
        {
            return SlicerError.MeshIsNotClosed;
        }

        if (!double.IsFinite(normal.x) || !double.IsFinite(normal.y) || !double.IsFinite(normal.z))
        {
            return SlicerError.NormalNotAVector;
        }

        if (!double.IsFinite(origin.x) || !double.IsFinite(origin.y) || !double.IsFinite(origin.z))
        {
            return SlicerError.OriginNotAVector;
        }

        if (normal.x == 0 && normal.y == 0 && normal.z == 0)
        {
            return SlicerError.NormalIsZero;
        }

        if (normal.x != 0)
        {
            if (normal.y != 0 || normal.z != 0)
            {
                return SlicerError.NormalNotAligned;
            }
            coordinate = origin.x;
            meshMax = bounds.Max.x;
            meshMin = bounds.Min.x;
        }

        else if (normal.y != 0)
        {
            if (normal.x != 0 || normal.z != 0)
            {
                return SlicerError.NormalNotAligned;
            }
            coordinate = origin.y;
            meshMax = bounds.Max.y;
            meshMin = bounds.Min.y;
        }

        else if (normal.z != 0)
        {
            if (normal.x != 0 || normal.y != 0)
            {
                return SlicerError.NormalNotAligned;
            }
            coordinate = origin.z;
            meshMax = bounds.Max.z;
            meshMin = bounds.Min.z;
        }

        if (!double.IsFinite(minThickness))
        {
            return SlicerError.MinThicknessNotANumber;
        }
        else if (coordinate > meshMax - minThickness)
        {
            return SlicerError.TooHighSliceCoordinate;
        }
        else if ( coordinate < meshMin + minThickness)
        {
            return SlicerError.TooLowSliceCoordinate;
        }
        return null;
    }
}

