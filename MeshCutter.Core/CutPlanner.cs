namespace MeshCutter.Core;

public class CutPlanner
{
    private const double tolerance = 0.01;

    public int PiecesX { get; }
    public int PiecesY { get; }
    public int PiecesZ { get; }


    public CutPlanner(MeshInfo meshInfo, PrintVolume printVolume, Margins margins)
    {

        var errors = SpaceValidator.Validate(printVolume.SizeX, printVolume.SizeY, printVolume.SizeZ, margins.X, margins.Y, margins.Z);
        if (errors.Count > 0)
        {
            throw new ArgumentException($"Invalid configuration: {string.Join(", ", errors)}");
        }

        PiecesX = CalculatePieces(meshInfo.SizeX, printVolume.SizeX, margins.X);
        PiecesY = CalculatePieces(meshInfo.SizeY, printVolume.SizeY, margins.Y);
        PiecesZ = CalculatePieces(meshInfo.SizeZ, printVolume.SizeZ, margins.Z);
    }


    private int CalculatePieces(double meshSize, double printVolumeSize, double margin)
    {
        double utilSpace = printVolumeSize - margin;

        return GetPieceNumber(meshSize, utilSpace);
    }

    private int GetPieceNumber(double pieceSize, double utilSpace)
    {
        int n = 1;
        while (Math.Round(pieceSize - n * utilSpace, 2) > tolerance)
            n += 1;

        return n;
    }   
}