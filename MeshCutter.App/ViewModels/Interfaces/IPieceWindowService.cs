using g3;
using MeshCutter.Core;

namespace MeshCutter.App.ViewModels.Interfaces;
public interface IPieceWindowService
{
    void OpenSlicedPiece(DMesh3 mesh, double sizeX, double sizeY, double sizeZ, 
        double marginX, double marginY, double marginZ, PieceHistory currentPieceHistory, History history); 
}