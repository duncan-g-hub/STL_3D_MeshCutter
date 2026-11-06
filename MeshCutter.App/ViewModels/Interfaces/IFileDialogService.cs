namespace MeshCutter.App.ViewModels.Interfaces;
public interface IFileDialogService
{
    string? GetFilePath(); 

    string? GetSavePath(string stlName);
}