using MeshCutter.Tests.Fakes;
using MeshCutter.App.ViewModels;

namespace MeshCutter.Tests;

public partial class FileViewModelTests
{
    [Fact]
    public void OpenFile_WhenUserCancels_ChangesNothing()
    {
        FileViewModel viewModel = new(new FakeFileDialogService(null, null, null), new FakePieceWindowService());
        viewModel.OpenFile();
        Assert.Null(viewModel.Mesh);
        Assert.Null(viewModel.FileErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }

    [Fact]
    public void OpenFile_WhenPathDoesNotExist_SetsErrorMessage()
    {
        FileViewModel viewModel = new(new FakeFileDialogService("Wrong Path", null, null), new FakePieceWindowService());
        viewModel.OpenFile();
        Assert.Null(viewModel.Mesh);
        Assert.NotNull(viewModel.FileErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }

    [Fact]
    public void OpenFile_WhenPathExist_GetMeshAndInfos()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();
        Assert.NotNull(viewModel.Mesh);
        Assert.Null(viewModel.FileErrorMessage);
        Assert.NotEqual("Informations : ", viewModel.Informations);
        Assert.Contains("Nombre de triangles : 12", viewModel.Informations);
        Assert.Contains("Nombre de sommets : 8", viewModel.Informations);
    }

    [Fact]
    public void OpenFile_WhenFormatIsUnsupported_SetsErrorMessage()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.SLDPRT");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();
        Assert.Null(viewModel.Mesh);
        Assert.NotNull(viewModel.FileErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }

    [Fact]
    public void OpenFile_WhenFileIsCorrupted_SetsErrorMessage()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm error.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();
        Assert.Null(viewModel.Mesh);
        Assert.NotNull(viewModel.FileErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }

    [Fact]
    public void LoadFile_WhenSecondFileIsInvalid_KeepsFirstModel()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(1, viewModel.PiecesX);
        Assert.Equal(1, viewModel.PiecesY);
        Assert.Equal(1, viewModel.PiecesZ);

        var firstMeshBase = viewModel.CurrentMesh;
        var firstMesh = viewModel.Mesh;
        Assert.NotNull(firstMeshBase);
        Assert.NotNull(firstMesh);

        string pathError = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm error.STL");
        viewModel.LoadFile(pathError);
        Assert.NotNull(viewModel.Mesh);
        Assert.NotNull(viewModel.FileErrorMessage);
        Assert.NotEqual("Informations : ", viewModel.Informations);
        Assert.Contains("Nombre de triangles : 12", viewModel.Informations);
        Assert.Contains("Nombre de sommets : 8", viewModel.Informations);
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(1, viewModel.PiecesX);
        Assert.Equal(1, viewModel.PiecesY);
        Assert.Equal(1, viewModel.PiecesZ);

        Assert.Same(firstMeshBase, viewModel.CurrentMesh);
        Assert.Same(firstMesh, viewModel.Mesh);
    }
}