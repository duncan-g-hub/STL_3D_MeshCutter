using System.Dynamic;
using System.Net.NetworkInformation;
using g3;


public class MeshInfo
{   
    public int VertexCount {get;}
    public int TriangleCount {get;}
    public double SizeX {get;}
    public double SizeY {get;}
    public double SizeZ {get;} 
    public double Area {get;}
    public double? Volume {get;}
    public bool IsClosed {get;}

    public MeshInfo(int vertexCount, int triangleCount, double sizeX, double sizeY, double sizeZ, double area, double? volume, bool isClosed)
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
        var measures = MeshMeasurements.VolumeArea(mesh, mesh.TriangleIndices(), i => mesh.GetVertex(i));
        double area = measures[1];
        double volume = measures[0]; 
        if (meshIsClosed)
        {
            return new MeshInfo(vertexCount, triangleCount, sizeX, sizeY, sizeZ, area, volume, meshIsClosed);
        }
        else
        {
            return new MeshInfo(vertexCount, triangleCount, sizeX, sizeY, sizeZ, area, null, meshIsClosed);
        }
    }
}