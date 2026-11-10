using MeshCutter.App.ViewModels.Interfaces;

namespace MeshCutter.Tests.Fakes;
public class FakeFileDialogService : IFileDialogService
{
    private string? _path;
    public string? StlName { get; private set; }
    private string? _savePath;
    public FakeFileDialogService(string? path, string? savePath)
    {
        _path = path;
        _savePath =  savePath;
    }

    public string? GetFilePath()
    {
        return _path;
    }

    public string? GetSavePath(string stlName)
    {
        StlName = stlName;
        return _savePath;
    }
}