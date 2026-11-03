using g3;
using MeshCutter.App.ViewModels.Interfaces;

namespace MeshCutter.Tests.Fakes;

public record PieceWindowCall(
    DMesh3 Mesh,
    double SizeX, double SizeY, double SizeZ,
    double MarginX, double MarginY, double MarginZ);

public class FakePieceWindowService : IPieceWindowService
{
    public List<PieceWindowCall> Calls { get; } = new();

    public void OpenSlicedPiece(DMesh3 mesh, double sizeX, double sizeY, double sizeZ,
        double marginX, double marginY, double marginZ)
    {
        Calls.Add(new PieceWindowCall(mesh, sizeX, sizeY, sizeZ, marginX, marginY, marginZ));
    }
}