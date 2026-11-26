using MeshCutter.App;

namespace MeshCutter.Tests;

public partial class PieceViewModelTests
{
    [Fact]
    public void ExportHistory_WhenPathIsValid_WritesTheCsvOfTheWholeHistory()
    {
        string exportPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".csv");
        var (piece, _, _) = CreatePiece(exportPath: exportPath);
        try
        {
            piece.ExportHistory();

            Assert.Null(piece.ExportErrorMessage);
            Assert.True(File.Exists(exportPath));

            string content = File.ReadAllText(exportPath);
            Assert.NotNull(piece.SharedHistory);
            Assert.Equal(CsvHistoryFormatter.Format(piece.SharedHistory.Pieces), content);

            // le test ci-dessus ne détecterait pas un Format cassé : on vérifie aussi un contenu précis
            var lines = content.ReplaceLineEndings("\n").TrimEnd('\n').Split('\n');
            Assert.Equal(4, lines.Length); // en-tête + P1, P2, P3
            Assert.StartsWith("P1;;cube 50mm origin;", lines[1]);
            Assert.StartsWith("P2;P1;cube 50mm origin > slice Z 25 bottom;", lines[2]);
        }
        finally
        {
            File.Delete(exportPath);
        }
    }

    [Fact]
    public void ExportHistory_WhenUserCancels_DoesNotSetError()
    {
        var (piece, _, dialog) = CreatePiece(exportPath: null);

        piece.ExportHistory();

        Assert.Equal("New Export.CSV", dialog.ExportName);
        Assert.Null(piece.ExportErrorMessage);
    }

    [Fact]
    public void ExportHistory_WhenDirectoryDoesNotExist_SetsErrorMessage()
    {
        string dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string exportPath = Path.Combine(dir, "export.csv");
        var (piece, _, _) = CreatePiece(exportPath: exportPath);

        piece.ExportHistory();

        Assert.NotNull(piece.ExportErrorMessage);
        Assert.False(File.Exists(exportPath));
    }

    [Fact]
    public void ExportHistory_WhenSucceedsAfterAFailure_ClearsErrorMessage()
    {
        string dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string exportPath = Path.Combine(dir, "export.csv");
        var (piece, _, _) = CreatePiece(exportPath: exportPath);
        try
        {
            piece.ExportHistory();                 // échec : le dossier n'existe pas
            Assert.NotNull(piece.ExportErrorMessage);

            Directory.CreateDirectory(dir);        // même chemin, mais il est maintenant valide
            piece.ExportHistory();

            Assert.Null(piece.ExportErrorMessage);
            Assert.True(File.Exists(exportPath));
        }
        finally
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
    }
}