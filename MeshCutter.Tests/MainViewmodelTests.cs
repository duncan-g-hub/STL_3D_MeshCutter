using MeshCutter.Tests.Fakes;
using MeshCutter.App.ViewModels;

namespace MeshCutter.Tests;

public class MainViewModelUnitTest
{
    [Fact]
    public void OpenFile_WhenUserCancels_ChangesNothing()
    {
        MainViewModel viewModel = new(new FakeFileDialogService(null));
        viewModel.OpenFile();
        Assert.Null(viewModel.Mesh);
        Assert.Null(viewModel.FileErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }

    [Fact]
    public void OpenFile_WhenPathDoesNotExist_SetsErrorMessage()
    {
        MainViewModel viewModel = new(new FakeFileDialogService("Wrong Path"));
        viewModel.OpenFile();
        Assert.Null(viewModel.Mesh);
        Assert.NotNull(viewModel.FileErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }

    [Fact]
    public void OpenFile_WhenPathExist_GetMeshAndInfos()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
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
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        Assert.Null(viewModel.Mesh);
        Assert.NotNull(viewModel.FileErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }

    [Fact]
    public void OpenFile_WhenFileIsCorrupted_SetsErrorMessage()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm error.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        Assert.Null(viewModel.Mesh);
        Assert.NotNull(viewModel.FileErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }


    [Fact]
    public void CutMesh_WhenMeshSmallerThanPrintableVolume_ReturnOnePiece()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        viewModel.PrintSizeX = 180;
        viewModel.MarginX = 40;
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(1, viewModel.PiecesX);
    }

    [Fact]
    public void CutMesh_WhenMeshBiggerThanPrintableVolume_ReturnTwoPieces()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        viewModel.PrintSizeX = 80;
        viewModel.MarginX = 40;
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(2, viewModel.PiecesX);
    }

    [Fact]
    public void CutMesh_WhenPrintableVolumeEqualZero_SetsError()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        viewModel.PrintSizeX = 0;
        viewModel.MarginX = 40;
        Assert.NotNull(viewModel.PrintErrorMessage);
    }

    [Fact]
    public void UpdateMarginsAndPrintableVolume_WhenNoFileLoaded_ReturnZeroPiece()
    {
        MainViewModel viewModel = new(new FakeFileDialogService(null));
        viewModel.PrintSizeX = 0;
        viewModel.MarginX = -5;
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(0, viewModel.PiecesX);
    }

    [Fact]
    public void LoadErrorFile_WhenValidFileLoaded_PreviousStateRetained()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(1, viewModel.PiecesX);
        Assert.Equal(1, viewModel.PiecesY);
        Assert.Equal(1, viewModel.PiecesZ);

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
    }

}

