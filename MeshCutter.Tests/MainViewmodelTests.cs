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
        Assert.Null(viewModel.ErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }

    [Fact]
    public void OpenFile_WhenPathDoesNotExist_SetsErrorMessage()
    {
        MainViewModel viewModel = new(new FakeFileDialogService("Wrong Path"));
        viewModel.OpenFile();
        Assert.Null(viewModel.Mesh);
        Assert.NotNull(viewModel.ErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }

    [Fact]
    public void OpenFile_WhenPathExist_GetMeshAndInfos()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        Assert.NotNull(viewModel.Mesh);
        Assert.Null(viewModel.ErrorMessage);
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
        Assert.NotNull(viewModel.ErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }
}

