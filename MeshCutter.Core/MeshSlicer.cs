using g3;
using gs;

namespace MeshCutter.Core;

public class MeshSlicer
{
    public static (DMesh3, DMesh3, DMesh3) Cut(DMesh3 meshBase, Vector3d origin, Vector3d normal)
    {      
        var error = SlicerValidator.Validate(origin, normal, meshBase.GetBounds(), Settings.MinThickness, meshBase.IsClosed());
        if (error != null)
        {
            throw new ArgumentException($"Invalid configuration: {error}");
        }
        var newMeshBottom = CopyAndCutAndFillMesh(meshBase, origin, normal);
        var newMeshTop = CopyAndCutAndFillMesh(meshBase, origin, -normal);
        return (newMeshBottom, newMeshTop, meshBase);
    }

    private static DMesh3 CopyAndCutAndFillMesh(DMesh3 mesh, Vector3d origin, Vector3d normal)
    {
        var meshCopy = new DMesh3();
        meshCopy.Copy(mesh);
        var cut = new MeshPlaneCut(meshCopy, origin , normal);
        cut.Cut();
        var filler = new PlanarHoleFiller(cut);
        if (filler.Fill() == false)
        {
            throw new InvalidOperationException($"Filling impossible. FailedInsertions: {filler.FailedInsertions} ; FailedMerges: {filler.FailedMerges} ; OutputHasCracks: {filler.OutputHasCracks}");
        }
        return meshCopy;
    }
}
