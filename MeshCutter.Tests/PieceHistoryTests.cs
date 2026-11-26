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
        Assert.Equal("cube origin > slice Z 25 bottom", pBottom.Name);
        Assert.Equal(Axis.Z, pBottom.SlicerAxis);
        Assert.Equal(25, pBottom.SlicerPosition);

        Assert.Equal("P3", pTop.Id);
        Assert.Equal("P1", pTop.ParentId);
        Assert.Equal("cube origin > slice Z 25 top", pTop.Name);
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
        Assert.Equal("cube origin > slice Z 25 bottom > slice Y 10 top", t1.Name);
        Assert.Equal("cube origin > slice Z 25 top > slice X 20 bottom", b2.Name);
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


    private static DMesh3 LoadCube()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var (_, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        return mesh;
    }

    [Fact]
    public void AddRotation_CreatesAChildOfTheParentWithTheRotationInfo()
    {
        var root = LoadCube();
        var rotated = MoveMesh.CopyAndRotateMesh(root, Axis.Z, 90);
        var history = new History();
        var pRoot = history.AddRoot("cube", root);

        var pRotated = history.AddRotation(pRoot, Axis.Z, 90, rotated);

        Assert.Equal("P2", pRotated.Id);
        Assert.Equal("P1", pRotated.ParentId);
        Assert.Equal("cube origin > rotate Z 90", pRotated.Name);
        Assert.Equal(90, pRotated.RotationAngle);
        Assert.Same(rotated, pRotated.Mesh);
        Assert.Equal(2, history.Pieces.Count);
    }

    [Fact]
    public void AddRotation_IsNotRecordedAsASlice()
    {
        var root = LoadCube();
        var history = new History();
        var pRoot = history.AddRoot("cube", root);

        var pRotated = history.AddRotation(pRoot, Axis.X, 45, MoveMesh.CopyAndRotateMesh(root, Axis.X, 45));

        Assert.Null(pRotated.SlicerAxis);
        Assert.Null(pRotated.SlicerPosition);
    }

    [Fact]
    public void AddSlice_IsNotRecordedAsARotation()
    {
        var root = LoadCube();
        var (bottom, top, _) = MeshSlicer.Cut(root, new Vector3d(0, 0, 25), new Vector3d(0, 0, 1));
        var history = new History();
        var pRoot = history.AddRoot("cube", root);

        var (pBottom, pTop) = history.AddSlice(pRoot, Axis.Z, 25, bottom, top);

        Assert.Null(pRoot.RotationAngle);
        Assert.Null(pBottom.RotationAngle);
        Assert.Null(pTop.RotationAngle);
    }

    [Fact]
    public void AddRotation_ThenAddSlice_ChainsTheNames()
    {
        var root = LoadCube();
        var rotated = MoveMesh.CopyAndRotateMesh(root, Axis.Z, 90);
        MoveMesh.CenterMesh(rotated);
        var (bottom, top, _) = MeshSlicer.Cut(rotated, new Vector3d(0, 0, 25), new Vector3d(0, 0, 1));
        var history = new History();
        var pRoot = history.AddRoot("cube", root);

        var pRotated = history.AddRotation(pRoot, Axis.Z, 90, rotated);
        var (pBottom, pTop) = history.AddSlice(pRotated, Axis.Z, 25, bottom, top);

        Assert.Equal("P3", pBottom.Id);
        Assert.Equal("P2", pBottom.ParentId);
        Assert.Equal("cube origin > rotate Z 90 > slice Z 25 bottom", pBottom.Name);
        Assert.Equal("P4", pTop.Id);
        Assert.Equal("P2", pTop.ParentId);
        Assert.Equal("cube origin > rotate Z 90 > slice Z 25 top", pTop.Name);
    }

    [Fact]
    public void AddRotation_KeepsTheParentMeshUntouched()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "L 65x120x40.STL");
        var (_, lMesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(lMesh);
        var rotated = MoveMesh.CopyAndRotateMesh(lMesh, Axis.Z, 90);
        var history = new History();
        var pRoot = history.AddRoot("L", lMesh);

        var pRotated = history.AddRotation(pRoot, Axis.Z, 90, rotated);

        Assert.Equal(65, pRoot.GetInfo().SizeX, 1e-3);
        Assert.Equal(120, pRoot.GetInfo().SizeY, 1e-3);
        Assert.Equal(120, pRotated.GetInfo().SizeX, 1e-3);
        Assert.Equal(65, pRotated.GetInfo().SizeY, 1e-3);
    }

    [Fact]
    public void AddRotation_WhenAngleHasDecimals_UsesInvariantCultureInTheName()
    {
        var root = LoadCube();
        var history = new History();
        var pRoot = history.AddRoot("cube", root);

        var pRotated = history.AddRotation(pRoot, Axis.Y, 12.5, MoveMesh.CopyAndRotateMesh(root, Axis.Y, 12.5));

        Assert.Equal("cube origin > rotate Y 12.5", pRotated.Name);
    }

}