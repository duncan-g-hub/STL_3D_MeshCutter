using g3;

public class Exploration
{
    public static (IOReadResult Result, DMesh3? Mesh) ReadFile(string path)
    {
        var meshBuilder = new DMesh3Builder();
        var result = StandardMeshReader.ReadFile(@$"{path}", ReadOptions.Defaults, meshBuilder);
        if (meshBuilder.Meshes.Count > 0)
        {
            return (result, meshBuilder.Meshes[0]);
        }
        else
        {
            return (result, null);
        }
    }
}



    // void Explo()
    // {
    //     g3.DMesh3Builder
    //     g3.STLFormatReader
    //     g3.StandardMeshReader
    //     g3.StandardMeshWriter
    //     g3.MeshPlaneCut
    //     g3.PlanarHoleFiller
        
    // }