using MeshCutter.Core;

namespace MeshCutter.App;

public static class AngleErrorFormatter
{
    public static string? Format(AngleError? error)
    {
        if (error == null)
            return null;

        return error.Value switch
        {
            AngleError.AngleNotADegree => "L'angle de rotation doit être un nombre valide.",
            AngleError.MeshIsNotClosed => "Le modèle doit être fermé pour procéder à une rotation.",
           
            _ => throw new ArgumentOutOfRangeException(nameof(error))
        };
    }
}