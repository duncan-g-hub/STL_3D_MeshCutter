using g3;
using MeshCutter.App.ViewModels.Interfaces;
using MeshCutter.Core;

namespace MeshCutter.Tests.Fakes;

public record PieceWindowCall(
    DMesh3 Mesh,
    double SizeX, double SizeY, double SizeZ,
    double MarginX, double MarginY, double MarginZ, 
    PieceHistory CurrentPieceHistory, History History);

public class FakePieceWindowService : IPieceWindowService
{
    public List<PieceWindowCall> Calls { get; } = new();

    public void OpenSlicedPiece(DMesh3 mesh, double sizeX, double sizeY, double sizeZ,
        double marginX, double marginY, double marginZ, PieceHistory currentPieceHistory, History history)
    {
        Calls.Add(new PieceWindowCall(mesh, sizeX, sizeY, sizeZ, marginX, marginY, marginZ, currentPieceHistory, history));
    }
}