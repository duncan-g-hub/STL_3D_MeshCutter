using g3;

namespace MeshCutter.Core;
public static class MoveMesh
{
    
    public static void CenterMesh(DMesh3 mesh)
    {
        if (mesh.VertexCount > 0)
        {
            var bounds = mesh.GetBounds();
            Vector3d center = bounds.Center;
            center.z = bounds.Min.z;
            MeshTransforms.Translate(mesh, -center);
        }
    }

    public static DMesh3 CopyAndRotateMesh(DMesh3 mesh, Axis axis, double rotationAngle)
    {
        var meshCopy = new DMesh3();
        meshCopy.Copy(mesh);

        Vector3d origin = new Vector3d (0, 0, 0);
        if (axis == Axis.X)
        {
            origin.x = 1;
        }
        else if (axis == Axis.Y)
        {
            origin.y = 1;
        }
        else if (axis == Axis.Z)
        {
            origin.z = 1;
        }

        var quaterniond = new Quaterniond(origin, rotationAngle);

        MeshTransforms.Rotate(meshCopy, origin, quaterniond);

        return meshCopy;
    }
}