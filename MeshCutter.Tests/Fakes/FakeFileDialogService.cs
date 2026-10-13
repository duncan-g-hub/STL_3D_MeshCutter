using MeshCutter.App.ViewModels.Interfaces;

namespace MeshCutter.Tests.Fakes;
public class FakeFileDialogService : IFileDialogService
{
    private string? _path;
    public FakeFileDialogService(string? path)
    {
        _path = path;
    }

    public string? GetFilePath()
    {
        return _path;
    }
}