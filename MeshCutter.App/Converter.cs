
using g3;
using System.Windows.Media.Media3D;
using System.Windows.Media;

class Converter
{
    public static MeshGeometry3D MeshConverter(DMesh3 mesh)
    {
        // gestion sommets
        IList<Point3D> positions = new List<Point3D>();
        IDictionary<int,int> indexMap = new Dictionary<int,int>();

        foreach (int i in mesh.VertexIndices())
        {
            indexMap.Add(i, positions.Count);
            var v = mesh.GetVertex(i);
            positions.Add(new Point3D(v.x, v.y, v.z));
        }

        // gestion triangles 
        IList<int> triangleIndices = new List<int>();
        foreach (int i in mesh.TriangleIndices())
        {            
            var t = mesh.GetTriangle(i);
            triangleIndices.Add(indexMap[t.a]); triangleIndices.Add(indexMap[t.b]); triangleIndices.Add(indexMap[t.c]);
        }

        // nouveau mesh convertit
        var convertedPositions = new Point3DCollection(positions);
        var convertedTriangles = new Int32Collection(triangleIndices);
        var convertedMesh = new MeshGeometry3D{Positions=convertedPositions, TriangleIndices=convertedTriangles};
        
        return convertedMesh;
    }
}