


using g3;

namespace MeshCutter.Core;
public class PieceHistory
{
    //id, nom complet, id du parent, axe, position, dimensions, triangles
    public string Id {get;}
    public string Name {get;}
    public Axis SlicerAxis {get;}
    public double SlicerPosition {get;}
    public DMesh3 Mesh {get;}
    public AxisAlignedBox3d Bounds {get;}
    public int Triangles {get;}
    public double? Volume {get;}

    public PieceHistory(string id, string name, Axis slicerAxis, double slicerPosition, DMesh3 mesh)
    {
        Id = id;
        Name = name;
        SlicerAxis = slicerAxis;
        SlicerPosition = slicerPosition;
        Mesh = mesh;
        Bounds = mesh.GetBounds();
        Triangles = mesh.TriangleCount;
        Volume = MeshInfo.GetMeshInfo(mesh).Volume;
    }


    public (PieceHistory bottom, PieceHistory top) GetSlicedPiecesHistory(PieceHistory parent, Axis slicerAxis, double slicerPosition, DMesh3 meshBottom, DMesh3 meshTop)
    {
        int parentIdInt = int.Parse(parent.Id.Replace("P",""));
        string idBottom = "P" + parentIdInt + 1;
        string idTop = "P" + parentIdInt + 2;
        string nameBottom = parent.Name + " > " + slicerAxis + slicerPosition + " bottom";
        string nameTop = parent.Name + " > " + slicerAxis + slicerPosition + " top";
        PieceHistory bottom = new PieceHistory(idBottom, nameBottom, slicerAxis, slicerPosition, meshBottom);
        PieceHistory top = new PieceHistory(idTop, nameTop, slicerAxis, slicerPosition, meshTop);
        return(bottom, top);
    }


}