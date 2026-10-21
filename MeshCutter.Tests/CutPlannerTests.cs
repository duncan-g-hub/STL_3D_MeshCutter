using MeshCutter.Core;

namespace MeshCutter.Tests;

public class CutPlannerTest
{

    public static MeshInfo GetFakeMeshInfo()
    {
        return new MeshInfo(12, 20, 65, 120, 40, 21400, 132000, true);
    }
    
    public static Margins GetFakeMargins()
    {
        return new Margins(40, 60, 20);
    }

    

    [Fact]
    public void CutMesh_WhenMeshSmallerThanPrintableVolume_ReturnOnePiece()
    {
        var margins = GetFakeMargins();
        PrintVolume printVolume = new PrintVolume(220 + margins.X, 220 + margins.Y, 220 + margins.Z);
        CutPlanner cutPlanner = new (GetFakeMeshInfo(), printVolume, margins);
        Assert.Equal(1, cutPlanner.PiecesX);
        Assert.Equal(1, cutPlanner.PiecesY);
        Assert.Equal(1, cutPlanner.PiecesZ);
    }

    [Fact]
    public void CutMesh_WhenMeshEqualPrintableVolume_ReturnOnePiece()
    {
        var margins = GetFakeMargins();
        PrintVolume printVolume = new PrintVolume(65 + margins.X, 120 + margins.Y, 40 + margins.Z);
        CutPlanner cutPlanner = new (GetFakeMeshInfo(), printVolume, margins);
        Assert.Equal(1, cutPlanner.PiecesX);
        Assert.Equal(1, cutPlanner.PiecesY);
        Assert.Equal(1, cutPlanner.PiecesZ);
    }

    [Fact]
    public void CutMesh_WhenMeshBiggerThanPrintableVolume_ReturnTwoPieces()
    {
        var margins = GetFakeMargins();
        PrintVolume printVolume = new PrintVolume(64 + margins.X, 119 + margins.Y, 39 + margins.Z);
        CutPlanner cutPlanner = new (GetFakeMeshInfo(), printVolume, margins);
        Assert.Equal(2, cutPlanner.PiecesX);
        Assert.Equal(2, cutPlanner.PiecesY);
        Assert.Equal(2, cutPlanner.PiecesZ);
    }

    [Fact]
    public void CutMesh_WhenMeshBiggerTwiceThanPrintableVolume_ReturnTwoPieces()
    {
        var margins = GetFakeMargins();
        PrintVolume printVolume = new PrintVolume(32.5 + margins.X, 60 + margins.Y, 20 + margins.Z);
        CutPlanner cutPlanner = new (GetFakeMeshInfo(), printVolume, margins);
        Assert.Equal(2, cutPlanner.PiecesX);
        Assert.Equal(2, cutPlanner.PiecesY);
        Assert.Equal(2, cutPlanner.PiecesZ);
    }

    [Fact]
    public void CutMesh_WhenMargesBiggerThanPrintableVolume_ThrowsError()
    {
        var margins = GetFakeMargins();
        PrintVolume printVolume = new PrintVolume(margins.X -1, margins.Y -1, margins.Z -1);
        var exception = Assert.Throws<ArgumentException>(
            () => new CutPlanner(GetFakeMeshInfo(), printVolume, margins)
        );
        Assert.Contains("Axis = X", exception.Message);
        Assert.Contains("Axis = Y", exception.Message);
        Assert.Contains("Axis = Z", exception.Message);
    }

    [Fact]
    public void CutMesh_WhenMarginZBiggerThanPrintableVolumeZ_ThrowsError()
    {
        var margins = GetFakeMargins();
        PrintVolume printVolume = new PrintVolume(100 + margins.X , 100 + margins.Y, margins.Z -1);
        var exception = Assert.Throws<ArgumentException>(
            () => new CutPlanner(GetFakeMeshInfo(), printVolume, margins)
        );
        Assert.Contains("Axis = Z", exception.Message);
        Assert.DoesNotContain("Axis = X", exception.Message);
        Assert.DoesNotContain("Axis = Y", exception.Message);
    }

    [Fact]
    public void CutMesh_WhenMargesEqualPrintableVolume_ThrowsError()
    {
        var margins = GetFakeMargins();
        PrintVolume printVolume = new PrintVolume(margins.X , margins.Y , margins.Z);
        var exception = Assert.Throws<ArgumentException>(
            () => new CutPlanner(GetFakeMeshInfo(), printVolume, margins)
        );
        Assert.Contains("Axis = X", exception.Message);
        Assert.Contains("Axis = Y", exception.Message);
        Assert.Contains("Axis = Z", exception.Message);
    }

    [Fact]
    public void CutMesh_WhenMeshATenthBiggerThanPrintableVolume_ReturnTwoPieces()
    {
        var margins = GetFakeMargins();
        PrintVolume printVolume = new PrintVolume(64.9 + margins.X, 119.9 + margins.Y, 39.9 + margins.Z);
        CutPlanner cutPlanner = new (GetFakeMeshInfo(), printVolume, margins);
        Assert.Equal(2, cutPlanner.PiecesX);
        Assert.Equal(2, cutPlanner.PiecesY);
        Assert.Equal(2, cutPlanner.PiecesZ);
    }

    [Fact]
    public void CutMesh_WhenMeshThanAHundredthBiggerThanPrintableVolume_ReturnOnePiece()
    {
        var margins = GetFakeMargins();
        PrintVolume printVolume = new PrintVolume(64.99 + margins.X, 119.99 + margins.Y, 39.99 + margins.Z);
        CutPlanner cutPlanner = new (GetFakeMeshInfo(), printVolume, margins);
        Assert.Equal(1, cutPlanner.PiecesX);
        Assert.Equal(1, cutPlanner.PiecesY);
        Assert.Equal(1, cutPlanner.PiecesZ);
    }

    [Fact]
    public void CutMesh_WhenMeshIsBiggerOnDifferentAxesThanPrintableVolume_ReturnTwoPiecesOnXThreeonYOneOnZ()
    {
        var margins = GetFakeMargins();
        PrintVolume printVolume = new PrintVolume(40 + margins.X, 40 + margins.Y, 40 + margins.Z);
        CutPlanner cutPlanner = new (GetFakeMeshInfo(), printVolume, margins);
        Assert.Equal(2, cutPlanner.PiecesX);
        Assert.Equal(3, cutPlanner.PiecesY);
        Assert.Equal(1, cutPlanner.PiecesZ);
    }
    
}


