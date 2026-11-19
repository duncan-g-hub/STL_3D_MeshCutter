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
}