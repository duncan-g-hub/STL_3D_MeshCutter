using g3;

namespace MeshCutter.Core;

public class MeshSlicer
{
    public static (DMesh3, DMesh3, DMesh3) Cut(DMesh3 meshBase, Vector3d origin, Vector3d normale)
    {
        // copie 
        var meshCopyBottom = new DMesh3();
        meshCopyBottom.Copy(meshBase);
        var meshCopyTop = new DMesh3();
        meshCopyTop.Copy(meshBase);

        // découpe 

        // partie basse
        var cutBottom = new MeshPlaneCut(meshCopyBottom, origin , normale);
        cutBottom.Cut();
        cutBottom.FillHoles();

        // partie haute
        var cutTop = new MeshPlaneCut(meshCopyTop, origin , -normale);
        cutTop.Cut();
        cutTop.FillHoles();

        return (meshCopyBottom, meshCopyTop, meshBase);
    }

}


    
    // public void explo()
    // {
    //     // faire une copie 
    //     var (result, meshBase) = MeshLoader.GetMeshFromSTLFile(@"C:\Users\Supa\Desktop\Apprentissage_DEV\C#\Projets\STL_3D_MeshCutter\samples\cube 50mm.STL");
    //     var meshCopy = new DMesh3();
    //     meshCopy.Copy(meshBase);

    //     // faire une découpe 
    //     var cut = new MeshPlaneCut(meshBase, new Vector3d(0, 0, 25) , new Vector3d(0, 0, -1));
    //     cut.Cut();
        

    //     var resultCut = cut.FillHoles();
    //     MessageBox.Show($"{resultCut}"); // -> false
    //     MessageBox.Show($"{meshBase.TriangleCount}"); // -> 22 triangles 
    //     MessageBox.Show($"{meshBase.VertexCount}"); // -> 13 sommets
    //     MessageBox.Show($"{meshBase.IsClosed()}"); // true

    //     var meshInfo = MeshInfo.GetMeshInfo(meshBase);
    //     MessageBox.Show($"{meshInfo.SizeX} {meshInfo.SizeY} {meshInfo.SizeZ}"); // 50 50 25
    //     MessageBox.Show($"{meshInfo.Volume}"); // 62500

        


    //     var cutCopy = new MeshPlaneCut(meshCopy, new Vector3d(0, 0, 25) , new Vector3d(0, 0, +1));
    //     cutCopy.Cut();
    //     var resultCutCopy = cutCopy.FillHoles();
    //     MessageBox.Show($"{meshCopy.TriangleCount}"); // -> 22 triangles 
    //     MessageBox.Show($"{meshCopy.VertexCount}"); // -> 13 sommets
    //     MessageBox.Show($"{meshCopy.IsClosed()}"); // true

    //     var meshInfoCopy = MeshInfo.GetMeshInfo(meshCopy);
    //     MessageBox.Show($"{meshInfoCopy.SizeX} {meshInfoCopy.SizeY} {meshInfoCopy.SizeZ}"); // 50 50 25
    //     MessageBox.Show($"{meshInfoCopy.Volume}"); // 62500

    //     MessageBox.Show($"{meshBase.GetBounds().Min.z} {meshBase.GetBounds().Max.z}"); // 25 50
    //     MessageBox.Show($"{meshCopy.GetBounds().Min.z} {meshCopy.GetBounds().Max.z}"); // 0 25
    // }