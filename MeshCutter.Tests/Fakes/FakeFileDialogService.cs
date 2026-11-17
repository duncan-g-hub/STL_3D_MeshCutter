using MeshCutter.App.ViewModels.Interfaces;

namespace MeshCutter.Tests.Fakes;
public class FakeFileDialogService : IFileDialogService
{
    private string? _path;
    public string? StlName { get; private set; }
    private string? _savePath;
    public string? ExportName { get; private set; }
    private string? _exportPath;
    public FakeFileDialogService(string? path, string? savePath, string? exportPath)
    {
        _path = path;
        _savePath =  savePath;
        _exportPath =  exportPath;
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

    public string? GetExportPath(string exportName)
    {
        ExportName = exportName;
        return _exportPath;
    }
}