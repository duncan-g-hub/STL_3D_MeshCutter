
using System.Windows;
using MeshCutter.App.ViewModels;
using MeshCutter.App.Services;

namespace MeshCutter.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);                          

        var fileDialogService = new WpfFileDialogService();
        var pieceWindowService = new WpfPieceWindowService();
        var fileViewModel = new FileViewModel(fileDialogService, pieceWindowService);  
        var window = new MainWindow(fileViewModel);   
        window.Show();
    }   
}

