using g3;

namespace MeshCutter.Core;

public class MeshSlicer
{
    public static (DMesh3, DMesh3, DMesh3) Cut(DMesh3 meshBase, Vector3d origin, Vector3d normal)
    {
        // validation        
        var error = SlicerValidator.Validate(origin, normal, meshBase.GetBounds(), Settings.MinThickness, meshBase.IsClosed());
        
        if (error != null)
        {
            throw new ArgumentException($"Invalid configuration: {error}");
        }

        // copie 
        var meshCopyBottom = new DMesh3();
        meshCopyBottom.Copy(meshBase);
        var meshCopyTop = new DMesh3();
        meshCopyTop.Copy(meshBase);

        // découpe 

        // partie basse
        var cutBottom = new MeshPlaneCut(meshCopyBottom, origin , normal);
        cutBottom.Cut();
        cutBottom.FillHoles();

        // partie haute
        var cutTop = new MeshPlaneCut(meshCopyTop, origin , -normal);
        cutTop.Cut();
        cutTop.FillHoles();

        return (meshCopyBottom, meshCopyTop, meshBase);
    }
}
