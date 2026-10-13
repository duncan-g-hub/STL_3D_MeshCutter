using Microsoft.Win32;
using MeshCutter.App.ViewModels.Interfaces;

namespace MeshCutter.App.Services;

public class WpfFileDialogService : IFileDialogService
{
    public string? GetFilePath()
    {
        var fileDialog = new OpenFileDialog();
        fileDialog.Filter = "STL files (*.STL)|*.STL|All files (*.*)|*.*";
        if(fileDialog.ShowDialog() == true)
        {
            return fileDialog.FileName;
        }
        return null;
    }
}