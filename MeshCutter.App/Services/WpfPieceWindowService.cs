using g3;
using MeshCutter.App.ViewModels.Interfaces;
using MeshCutter.App.ViewModels;

namespace MeshCutter.App.Services;

public class WpfPieceWindowService : IPieceWindowService
{
    private readonly IFileDialogService _fileDialogService;

    public WpfPieceWindowService(IFileDialogService fileDialogService)
    {
        _fileDialogService = fileDialogService;
    }
    public void OpenSlicedPiece(DMesh3 mesh, double sizeX, double sizeY, double sizeZ, 
    double marginX, double marginY, double marginZ)
    {
        var pieceViewModel = new PieceViewModel(mesh, sizeX, sizeY, sizeZ, marginX, marginY, marginZ, _fileDialogService, this);
        var pieceWindow = new MainWindow(pieceViewModel);
        pieceWindow.Show();
    }
}