using System.Dynamic;
using System.Net.NetworkInformation;
using g3;


public class MeshInfo
{   
    public int VertexCount {get; set;}
    public int TriangleCount {get; set;}
    public double SizeX {get; set;}
    public double SizeY {get; set;}
    public double SizeZ {get; set;}
    public double? Area {get; set;}
    public double? Volume {get; set;}
    public bool IsClosed {get; set;}

    public MeshInfo(int vertexCount, int triangleCount, double sizeX, double sizeY, double sizeZ, double? area, double? volume, bool isClosed)
    {
        VertexCount = vertexCount;
        TriangleCount = triangleCount;
        SizeX = sizeX;
        SizeY = sizeY;
        SizeZ = sizeZ;
        Area = area;
        Volume = volume;
        IsClosed = isClosed;
    }

    



    public static MeshInfo GetMeshInfo(DMesh3 mesh)
    {
        int vertexCount = mesh.VertexCount;
        int triangleCount = mesh.TriangleCount;
        var bounds = mesh.GetBounds();
        double sizeX = bounds.Width;
        double sizeY = bounds.Height;
        double sizeZ = bounds.Depth;

        bool meshIsClosed = mesh.IsClosed();
        if (meshIsClosed)
        {
            var measures = MeshMeasurements.VolumeArea(mesh, mesh.TriangleIndices(), i => mesh.GetVertex(i));
            double area = measures[1];
            double volume = measures[0];   
            return new MeshInfo(vertexCount, triangleCount, sizeX, sizeY, sizeZ, area, volume, meshIsClosed);
        }
        else
        {
            return new MeshInfo(vertexCount, triangleCount, sizeX, sizeY, sizeZ, null, null, meshIsClosed);
        }

        

        

        
    }
}