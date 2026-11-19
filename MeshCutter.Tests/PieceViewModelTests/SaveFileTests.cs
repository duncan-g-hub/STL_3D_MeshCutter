using MeshCutter.Core;

namespace MeshCutter.Tests;

public partial class PieceViewModelTests
{
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

        var (piece, _, _) = CreatePiece(savePath, axis:Axis.Y, position:0, pieceIndex:0);
        try
        {
            piece.SaveFile();
            Assert.Null(piece.SaveErrorMessage);
            Assert.True(new FileInfo(savePath).Length > 0);

            var (result, mesh) = MeshLoader.GetMeshFromSTLFile(savePath);
            Assert.NotNull(mesh);

            Assert.Equal(-12.5, mesh.GetBounds().Min.y, 1e-3);
            Assert.Equal(12.5, mesh.GetBounds().Max.y, 1e-3);
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

        var (piece, _, _) = CreatePiece(savePath, axis:Axis.X, position:0, pieceIndex:0);
        try
        {
            piece.SaveFile();
            Assert.Null(piece.SaveErrorMessage);
            Assert.True(new FileInfo(savePath).Length > 0);

            var (result, mesh) = MeshLoader.GetMeshFromSTLFile(savePath);
            Assert.NotNull(mesh);

            Assert.Equal(-12.5, mesh.GetBounds().Min.x, 1e-3);
            Assert.Equal(12.5, mesh.GetBounds().Max.x, 1e-3);
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

            Assert.Equal(0, mesh.GetBounds().Min.z, 1e-3);
            Assert.Equal(40, mesh.GetBounds().Max.z, 1e-3);
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

        var (piece, _, _) = CreatePiece(savePath, axis:Axis.Y, position:0, pieceIndex:1);
        try
        {
            piece.SaveFile();
            Assert.Null(piece.SaveErrorMessage);
            Assert.True(new FileInfo(savePath).Length > 0);

            var (result, mesh) = MeshLoader.GetMeshFromSTLFile(savePath);
            Assert.NotNull(mesh);

            Assert.Equal(-12.5, mesh.GetBounds().Min.y, 1e-3);
            Assert.Equal(12.5, mesh.GetBounds().Max.y, 1e-3);
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

        var (piece, _, _) = CreatePiece(savePath, axis:Axis.X, position:0, pieceIndex:1);
        try
        {
            piece.SaveFile();
            Assert.Null(piece.SaveErrorMessage);
            Assert.True(new FileInfo(savePath).Length > 0);

            var (result, mesh) = MeshLoader.GetMeshFromSTLFile(savePath);
            Assert.NotNull(mesh);

            Assert.Equal(-12.5, mesh.GetBounds().Min.x, 1e-3);
            Assert.Equal(12.5, mesh.GetBounds().Max.x, 1e-3);
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
