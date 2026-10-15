namespace MeshCutter.Core;

public class CutPlanner
{
    private const double tolerance = 0.01;

    public int PiecesX { get; }
    public int PiecesY { get; }
    public int PiecesZ { get; }
    public string? ErrorMessage { get; }

    public CutPlanner(MeshInfo meshInfo, PrintVolume printVolume, Margins margins)
    {
        var errors = new List<string>();

        bool validX = IsUtilSpace(printVolume.SizeX, margins.X);
        bool validY = IsUtilSpace(printVolume.SizeY, margins.Y);
        bool validZ = IsUtilSpace(printVolume.SizeZ, margins.Z);

        if (!validX)
            errors.Add("X : les marges dépassent l'espace d'impression.");

        if (!validY)
            errors.Add("Y : les marges dépassent l'espace d'impression.");

        if (!validZ)
            errors.Add("Z : les marges dépassent l'espace d'impression.");

        if (errors.Count > 0)
        {
            ErrorMessage = string.Join("\n", errors);
        }
        else
        {
            ErrorMessage = null;
        }

        PiecesX = CalculatePieces(meshInfo.SizeX, printVolume.SizeX, margins.X);
        PiecesY = CalculatePieces(meshInfo.SizeY, printVolume.SizeY, margins.Y);
        PiecesZ = CalculatePieces(meshInfo.SizeZ, printVolume.SizeZ, margins.Z);
    }


    private int CalculatePieces(double meshSize, double printVolumeSize, double margin)
    {
        if (!IsUtilSpace(printVolumeSize, margin))
            return 0;

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

    private bool IsUtilSpace(double sizePrintVolume, double sizeMargin)
    {
        if (sizePrintVolume <= sizeMargin)
        {
            return false;
        }
        return true;
    }  
}