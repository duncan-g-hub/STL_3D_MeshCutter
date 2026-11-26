using g3;
using System.Globalization;

namespace MeshCutter.Core;

public record PieceHistory(string Id, string? ParentId, string Name, Axis? SlicerAxis, double? SlicerPosition, double? RotationAngle, DMesh3 Mesh)
{
    public  MeshInfo GetInfo() => MeshInfo.GetMeshInfo(Mesh);
}

public class History
{
    private int _idCounter = 0;
    private readonly List<PieceHistory> _piecesHistory = new();
    public IReadOnlyList<PieceHistory> Pieces => _piecesHistory;

    public PieceHistory AddRoot(string name, DMesh3 mesh)
    {
        _idCounter++;
        var p = new PieceHistory("P" + _idCounter, null, name + " origin", null, null, null, mesh);
        _piecesHistory.Add(p);
        return p;
    }

    public (PieceHistory pBottom, PieceHistory pTop) AddSlice(PieceHistory parent, Axis slicerAxis, double slicerPosition, DMesh3 meshBottom, DMesh3 meshTop)
    {        
        var pBottom = AddPiece(parent, slicerAxis, slicerPosition, meshBottom, "bottom");
        var pTop = AddPiece(parent, slicerAxis, slicerPosition, meshTop, "top");
        return (pBottom, pTop);
    }

    private PieceHistory AddPiece(PieceHistory parent, Axis slicerAxis, double slicerPosition, DMesh3 mesh, string side)
    {
        _idCounter++;
        string pId = "P" + _idCounter;
        string pName = $"{parent.Name} > {slicerAxis} {slicerPosition.ToString(CultureInfo.InvariantCulture)} {side}";
        var p = new PieceHistory(pId, parent.Id, pName, slicerAxis, slicerPosition, null, mesh);
        _piecesHistory.Add(p);
        return p;
    }

    public PieceHistory AddRotation(PieceHistory parent, Axis rotationAxis, double rotationAngle, DMesh3 meshRotate)
    {        
        _idCounter++;
        string pId = "P" + _idCounter;
        string pName = $"{parent.Name} > rotate {rotationAxis} {rotationAngle.ToString(CultureInfo.InvariantCulture)}";
        var p = new PieceHistory(pId, parent.Id, pName, rotationAxis, null, rotationAngle, meshRotate);
        _piecesHistory.Add(p);
        return p;
    }

}