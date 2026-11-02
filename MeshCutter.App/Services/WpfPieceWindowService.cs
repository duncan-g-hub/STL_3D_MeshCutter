using g3;
using MeshCutter.App.ViewModels.Interfaces;

namespace MeshCutter.App.Services;

public class WpfPieceWindowService : IPieceWindowService
{

    public void OpenSlicedPiece(DMesh3 mesh, double sizeX, double sizeY, double sizeZ, 
    double marginX, double marginY, double marginZ)
    {
        var pieceViewModel = new PieceViewModel(mesh, sizeX, sizeY, sizeZ, marginX, marginY, marginZ, this );
        var pieceWindow = new MainWindow(pieceViewModel);
    }
}