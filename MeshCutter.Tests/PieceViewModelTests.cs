using MeshCutter.App.ViewModels;
using MeshCutter.Core;
using MeshCutter.Tests.Fakes;

namespace MeshCutter.Tests;

public class PieceViewModelTests
{
    private static (PieceViewModel Piece, FakePieceWindowService Fake, FakeFileDialogService Dialog) CreatePiece(string? savePath = null, Axis axis = Axis.Z, double position = 25, int pieceIndex = 0)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var fileFake = new FakePieceWindowService();
        var fileViewModel = new FileViewModel(new FakeFileDialogService(path, null), fileFake);
        fileViewModel.OpenFile();
        fileViewModel.SlicerAxis = axis;
        fileViewModel.SlicerPosition = position;
        fileViewModel.CutMesh();

        var pieceFake = new FakePieceWindowService();
        var dialogFake = new FakeFileDialogService(path, savePath);
        var piece = new PieceViewModel(fileFake.Calls[pieceIndex].Mesh, 80, 200, 150, 40, 60, 20, dialogFake, pieceFake);
        return (piece, pieceFake, dialogFake);
    }

    [Fact]
    public void Constructor_LoadsMeshAndPrinterSettings()
    {
        var (piece, _, _) = CreatePiece();
        Assert.NotNull(piece.Mesh);
        Assert.NotNull(piece.CurrentMesh);
        Assert.NotEqual("Informations : ", piece.Informations);
        Assert.Equal(80, piece.PrintSizeX);
        Assert.Equal(200, piece.PrintSizeY);
        Assert.Equal(150, piece.PrintSizeZ);
        Assert.Equal(40, piece.MarginX);
        Assert.Equal(60, piece.MarginY);
        Assert.Equal(20, piece.MarginZ);
    }

    [Fact]
    public void Constructor_ComputesPiecesWithGivenSettings()
    {
        var (piece, _, _) = CreatePiece();

        Assert.Null(piece.PrintErrorMessage);
        Assert.Equal(2, piece.PiecesX);
        Assert.Equal(1, piece.PiecesY);
        Assert.Equal(1, piece.PiecesZ);
    }

    [Fact]
    public void CutMesh_WhenPieceIsCutAgain_OpensTwoNewWindows()
    {
        var (piece, fake, _) = CreatePiece();

        piece.SlicerAxis = Axis.Z;
        piece.SlicerPosition = 10;
        piece.CutMesh();

        Assert.Null(piece.SlicerErrorMessage);
        Assert.Equal(2, fake.Calls.Count);
        Assert.Equal(0, fake.Calls[0].Mesh.GetBounds().Min.z, 1e-3);
        Assert.Equal(10, fake.Calls[0].Mesh.GetBounds().Max.z, 1e-3);
        Assert.Equal(10, fake.Calls[1].Mesh.GetBounds().Min.z, 1e-3);
        Assert.Equal(25, fake.Calls[1].Mesh.GetBounds().Max.z, 1e-3);
    }


    [Fact]
    public void SaveFile_WhenPathAndCutOnZAreValid_CreateValidStlFileBottom()
    {
        string savePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".stl");

        var (piece, _, _) = CreatePiece(savePath, axis:Axis.Z, position:10, pieceIndex:0);
        try
        {
            piece.SaveFile();
            Assert.Null(piece.SaveErrorMessage);
            Assert.True(new FileInfo(savePath).Length > 0);

            var (result, mesh) = MeshLoader.GetMeshFromSTLFile(savePath);
            Assert.NotNull(mesh);

            Assert.Equal(0, mesh.GetBounds().Min.z, 1e-3);
            Assert.Equal(10, mesh.GetBounds().Max.z, 1e-3);
        }
        finally
        {
            File.Delete(savePath);
        }
    }

    [Fact]
    public void SaveFile_WhenPathAndCutOnYAreValid_CreateValidStlFileBottom()
    {
        string savePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".stl");

        var (piece, _, _) = CreatePiece(savePath, axis:Axis.Y, position:10, pieceIndex:0);
        try
        {
            piece.SaveFile();
            Assert.Null(piece.SaveErrorMessage);
            Assert.True(new FileInfo(savePath).Length > 0);

            var (result, mesh) = MeshLoader.GetMeshFromSTLFile(savePath);
            Assert.NotNull(mesh);

            Assert.Equal(0, mesh.GetBounds().Min.y, 1e-3);
            Assert.Equal(10, mesh.GetBounds().Max.y, 1e-3);
        }
        finally
        {
            File.Delete(savePath);
        }
    }

    [Fact]
    public void SaveFile_WhenPathAndCutOnXAreValid_CreateValidStlFileBottom()
    {
        string savePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".stl");

        var (piece, _, _) = CreatePiece(savePath, axis:Axis.X, position:10, pieceIndex:0);
        try
        {
            piece.SaveFile();
            Assert.Null(piece.SaveErrorMessage);
            Assert.True(new FileInfo(savePath).Length > 0);

            var (result, mesh) = MeshLoader.GetMeshFromSTLFile(savePath);
            Assert.NotNull(mesh);

            Assert.Equal(0, mesh.GetBounds().Min.x, 1e-3);
            Assert.Equal(10, mesh.GetBounds().Max.x, 1e-3);
        }
        finally
        {
            File.Delete(savePath);
        }
    }

    [Fact]
    public void SaveFile_WhenPathAndCutOnZAreValid_CreateValidStlFileTop()
    {
        string savePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".stl");

        var (piece, _, _) = CreatePiece(savePath, axis:Axis.Z, position:10, pieceIndex:1);
        try
        {
            piece.SaveFile();
            Assert.Null(piece.SaveErrorMessage);
            Assert.True(new FileInfo(savePath).Length > 0);

            var (result, mesh) = MeshLoader.GetMeshFromSTLFile(savePath);
            Assert.NotNull(mesh);

            Assert.Equal(10, mesh.GetBounds().Min.z, 1e-3);
            Assert.Equal(50, mesh.GetBounds().Max.z, 1e-3);
        }
        finally
        {
            File.Delete(savePath);
        }
    }

    [Fact]
    public void SaveFile_WhenPathAndCutOnYAreValid_CreateValidStlFileTop()
    {
        string savePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".stl");

        var (piece, _, _) = CreatePiece(savePath, axis:Axis.Y, position:10, pieceIndex:1);
        try
        {
            piece.SaveFile();
            Assert.Null(piece.SaveErrorMessage);
            Assert.True(new FileInfo(savePath).Length > 0);

            var (result, mesh) = MeshLoader.GetMeshFromSTLFile(savePath);
            Assert.NotNull(mesh);

            Assert.Equal(10, mesh.GetBounds().Min.y, 1e-3);
            Assert.Equal(50, mesh.GetBounds().Max.y, 1e-3);
        }
        finally
        {
            File.Delete(savePath);
        }
    }

    [Fact]
    public void SaveFile_WhenPathAndCutOnXAreValid_CreateValidStlFileTop()
    {
        string savePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".stl");

        var (piece, _, _) = CreatePiece(savePath, axis:Axis.X, position:10, pieceIndex:1);
        try
        {
            piece.SaveFile();
            Assert.Null(piece.SaveErrorMessage);
            Assert.True(new FileInfo(savePath).Length > 0);

            var (result, mesh) = MeshLoader.GetMeshFromSTLFile(savePath);
            Assert.NotNull(mesh);

            Assert.Equal(10, mesh.GetBounds().Min.x, 1e-3);
            Assert.Equal(50, mesh.GetBounds().Max.x, 1e-3);
        }
        finally
        {
            File.Delete(savePath);
        }
    }





    [Fact]
    public void SaveFile_WhenUserCancels_DoesNotSetError()
    {
        var (piece, _, dialog) = CreatePiece(null, Axis.Z, 25);

        piece.SaveFile();

        Assert.NotNull(dialog.StlName);         
        Assert.Null(piece.SaveErrorMessage);    
    }

    [Fact]
    public void SaveFile_WhenFormatIsUnknown_SetsErrorMessage()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".tmp");
        var (piece, _, _) = CreatePiece(path, Axis.Z, 25);
        try
        {
            piece.SaveFile();

            Assert.NotNull(piece.SaveErrorMessage);
            Assert.Contains("UnknownFormatError", piece.SaveErrorMessage);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void SaveFile_ProposesDefaultName()
    {
        var (piece, _, dialog) = CreatePiece(null, Axis.Z, 25);

        piece.SaveFile();

        Assert.Equal("New Model.STL", dialog.StlName);
    }

}