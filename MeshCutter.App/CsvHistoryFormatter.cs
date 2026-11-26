
using MeshCutter.Core;
using System.Globalization;
using System.Text;

namespace MeshCutter.App;

public static class CsvHistoryFormatter
{
    public static string Format(IReadOnlyList<PieceHistory> pieces)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Piece ID;Parent ID;Name;SizeX;SizeY;SizeZ;Triangles;Volume");
        for (int i = 0; i < pieces.Count; i++)
        {
            var meshInfo = pieces[i].GetInfo();
            sb.AppendLine($"{pieces[i].Id};{pieces[i].ParentId};{Escape(pieces[i].Name)};{meshInfo.SizeX.ToString(CultureInfo.InvariantCulture)};{meshInfo.SizeY.ToString(CultureInfo.InvariantCulture)};{meshInfo.SizeZ.ToString(CultureInfo.InvariantCulture)};{meshInfo.TriangleCount};{meshInfo.Volume?.ToString(CultureInfo.InvariantCulture)}"); 
        }
        return sb.ToString(); 
    }

    private static string Escape(string field)
    {
        if (field.Contains(';') || field.Contains('"') || field.Contains('\n'))
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        return field;
    }
}