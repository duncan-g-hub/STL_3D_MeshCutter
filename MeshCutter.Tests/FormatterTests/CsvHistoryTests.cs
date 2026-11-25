using System.Globalization;
using g3;
using MeshCutter.App;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public class CsvHistoryFormatterTests
{
    private const string Header = "ID;Parent ID;Name;SizeX;SizeY;SizeZ;Triangles;Volume";

    private static DMesh3 LoadCube()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        var (_, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        Assert.NotNull(mesh);
        return mesh;
    }

    private static History CutOnce(string rootName, Axis axis, double position)
    {
        var mesh = LoadCube();
        var normal = axis switch
        {
            Axis.X => new Vector3d(1, 0, 0),
            Axis.Y => new Vector3d(0, 1, 0),
            _ => new Vector3d(0, 0, 1),
        };
        var origin = new Vector3d(normal.x * position, normal.y * position, normal.z * position);
        var (bottom, top, _) = MeshSlicer.Cut(mesh, origin, normal);

        var history = new History();
        var root = history.AddRoot(rootName, mesh);
        history.AddSlice(root, axis, position, bottom, top);
        return history;
    }

    private static string[] Lines(string csv) =>
        csv.ReplaceLineEndings("\n").TrimEnd('\n').Split('\n');

    [Fact]
    public void Format_RootAndTwoChildren_ProducesExpectedText()
    {
        var history = CutOnce("cube 50mm", Axis.Z, 25);
        var lines = Lines(CsvHistoryFormatter.Format(history.Pieces));
        Assert.Equal(4, lines.Length);
        Assert.Equal(Header, lines[0]);
        Assert.Equal("P1;;cube 50mm origin;50;50;50;12;125000", lines[1]);
        Assert.Equal("P2;P1;cube 50mm origin > Z 25 bottom;50;50;25;20;62500", lines[2]);
        Assert.Equal("P3;P1;cube 50mm origin > Z 25 top;50;50;25;20;62500", lines[3]);
    }

    [Fact]
    public void Format_NameContainingSeparator_IsQuoted()
    {
        var history = CutOnce("cube; v2", Axis.Z, 25);
        var lines = Lines(CsvHistoryFormatter.Format(history.Pieces));
        Assert.Equal("P1;;\"cube; v2 origin\";50;50;50;12;125000", lines[1]);
    }

    [Fact]
    public void Format_NameContainingQuote_DoublesTheQuote()
    {
        var history = CutOnce("cube \"v2\"", Axis.Z, 25);
        var lines = Lines(CsvHistoryFormatter.Format(history.Pieces));
        Assert.Equal("P1;;\"cube \"\"v2\"\" origin\";50;50;50;12;125000", lines[1]);
    }

    [Fact]
    public void Format_NonIntegerCut_UsesInvariantDecimalPoint()
    {
        var history = CutOnce("cube", Axis.X, 5.5);
        var lines = Lines(CsvHistoryFormatter.Format(history.Pieces));
        Assert.Equal("P2;P1;cube origin > X 5.5 bottom;5.5;50;50;20;13750", lines[2]);
        var cellsTop = lines[3].Split(';');
        Assert.Equal("cube origin > X 5.5 top", cellsTop[2]);
        Assert.Equal("44.5", cellsTop[3]);
        Assert.Equal(8, cellsTop.Length);
    }

    [Fact]
    public void Format_IsIndependentOfThreadCulture()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            var history = CutOnce("cube", Axis.X, 5.5);
            var lines = Lines(CsvHistoryFormatter.Format(history.Pieces));
            Assert.Contains("X 5.5 bottom", lines[2]);
            Assert.Equal("5.5", lines[2].Split(';')[3]);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void Format_EmptyList_ReturnsOnlyHeader()
    {
        var lines = Lines(CsvHistoryFormatter.Format(new History().Pieces));
        Assert.Single(lines);
        Assert.Equal(Header, lines[0]);
    }
}