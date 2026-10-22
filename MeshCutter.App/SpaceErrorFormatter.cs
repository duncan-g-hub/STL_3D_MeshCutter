using MeshCutter.Core;

namespace MeshCutter.App;

public static class SpaceErrorFormatter
{
    public static string? Format(IReadOnlyList<SpaceError> errors)
    {
        if (errors.Count == 0)
            return null;

        return string.Join("\n", errors.Select(FormatOne));
    }

    private static string FormatOne(SpaceError error)
    {
        string name = error.Axis switch
        {
            Axis.X => "Axe X",
            Axis.Y => "Axe Y",
            Axis.Z => "Axe Z",
            _ => throw new ArgumentOutOfRangeException(nameof(error))
        };

        return error.KindOfError switch
        {
            KindOfError.SizeNotPositive => $"{name} : l'espace de l'imprimante doit être supérieur à 0mm.",
            KindOfError.MarginNegative => $"{name} : la marge doit être supérieure ou égale à 0mm.",
            KindOfError.MarginTooBig => $"{name} : la marge doit être inférieure à l'espace de l'imprimante.",
            KindOfError.SizeNotANumber => $"{name} : l'espace de l'imprimante doit être un nombre valide.",
            KindOfError.MarginNotANumber => $"{name} : la marge doit être un nombre valide.",
            _ => throw new ArgumentOutOfRangeException(nameof(error))
        };
    }
}