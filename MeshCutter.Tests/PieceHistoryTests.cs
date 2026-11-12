using g3;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public class PieceHistoryTests
{
    private static (DMesh3 Base, DMesh3 Bottom, DMesh3 Top) CutCube()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var (_, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        var (bottom, top, _) = MeshSlicer.Cut(mesh, new Vector3d(0, 0, 25), new Vector3d(0, 0, 1));
        return (mesh, bottom, top);
    }

    [Fact]
    public void History_WhenAddRoot_CreatesRootWithoutParentNorCut()
    {
        var (mesh, _, _) = CutCube();
        var history = new History();

        var root = history.AddRoot("cube", mesh);

        Assert.Equal("P1", root.Id);
        Assert.Null(root.ParentId);
        Assert.Equal("cube origin", root.Name);
        Assert.Null(root.SlicerAxis);
        Assert.Null(root.SlicerPosition);
        Assert.Same(mesh, root.Mesh);
        Assert.Single(history.Pieces);
    }

    [Fact]
    public void History_WhenAddSlice_CreatesTwoChildrenWithCorrectIdentity()
    {
        var (mesh, meshBottom, meshTop) = CutCube();
        var history = new History();
        var root = history.AddRoot("cube", mesh);

        var (pBottom, pTop) = history.AddSlice(root, Axis.Z, 25, meshBottom, meshTop);

        Assert.Equal("P2", pBottom.Id);
        Assert.Equal("P1", pBottom.ParentId);
        Assert.Equal("cube origin > Z 25 bottom", pBottom.Name);
        Assert.Equal(Axis.Z, pBottom.SlicerAxis);
        Assert.Equal(25, pBottom.SlicerPosition);

        Assert.Equal("P3", pTop.Id);
        Assert.Equal("P1", pTop.ParentId);
        Assert.Equal("cube origin > Z 25 top", pTop.Name);
        Assert.Equal(Axis.Z, pTop.SlicerAxis);
        Assert.Equal(25, pTop.SlicerPosition);

        Assert.Equal(3, history.Pieces.Count);
    }

    [Fact]
    public void History_WhenAddSlice_KeepsTheRightMeshOnTheRightSide()
    {
        var (mesh, meshBottom, meshTop) = CutCube();
        var history = new History();
        var root = history.AddRoot("cube", mesh);

        var (pBottom, pTop) = history.AddSlice(root, Axis.Z, 25, meshBottom, meshTop);

        Assert.Same(meshBottom, pBottom.Mesh);
        Assert.Same(meshTop, pTop.Mesh);
    }

    [Fact]
    public void History_WhenAddSliceOnAChild_KeepsIdsUniqueAndNamesUnique()
    {
        var (mesh, meshBottom, meshTop) = CutCube();
        var history = new History();
        var root = history.AddRoot("cube", mesh);
        var (pBottom, pTop) = history.AddSlice(root, Axis.Z, 25, meshBottom, meshTop);

        var (b1, t1) = history.AddSlice(pBottom, Axis.Y, 10, meshBottom, meshTop);
        var (b2, t2) = history.AddSlice(pTop, Axis.X, 20, meshBottom, meshTop);

        var ids = history.Pieces.Select(p => p.Id).ToList();
        var names = history.Pieces.Select(p => p.Name).ToList();
        Assert.Equal(7, history.Pieces.Count);
        Assert.Equal(ids.Count, ids.Distinct().Count());
        Assert.Equal(names.Count, names.Distinct().Count());

        Assert.Equal(pBottom.Id, b1.ParentId);
        Assert.Equal(pTop.Id, b2.ParentId);
        Assert.Equal("cube origin > Z 25 bottom > Y 10 top", t1.Name);
        Assert.Equal("cube origin > Z 25 top > X 20 bottom", b2.Name);
    }

    [Fact]
    public void Pieces_IsReadOnlyView_ReflectingLaterAdds()
    {
        var (mesh, meshBottom, meshTop) = CutCube();
        var history = new History();
        var pieces = history.Pieces;

        var root = history.AddRoot("cube", mesh);
        history.AddSlice(root, Axis.Z, 25, meshBottom, meshTop);

        Assert.Equal(3, pieces.Count);
    }

    [Fact]
    public void PieceHistory_WhenGetInfo_ReturnsTheInfoOfItsOwnMesh()
    {
        var (mesh, meshBottom, meshTop) = CutCube();
        var history = new History();
        var root = history.AddRoot("cube", mesh);
        var (pBottom, _) = history.AddSlice(root, Axis.Z, 25, meshBottom, meshTop);

        var info = pBottom.GetInfo();

        Assert.Equal(50, info.SizeX, 1e-3);
        Assert.Equal(50, info.SizeY, 1e-3);
        Assert.Equal(25, info.SizeZ, 1e-3);
        Assert.Equal(62500, info.Volume!.Value, 1e-3);
    }

}