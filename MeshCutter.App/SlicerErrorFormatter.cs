using MeshCutter.Core;

namespace MeshCutter.App;


public static class SlicerErrorFormatter
{
    public static string? Format(SlicerError? error)
    {
        if (error == null)
            return null;

        return error.Value switch
        {
            SlicerError.TooLowSliceCoordinate => "La position du plan de découpe doit être au sein du modèle, au minimum à une distance valide du bord.",
            SlicerError.TooHighSliceCoordinate => "La position du plan de découpe doit être au sein du modèle, au maximum à une distance valide du bord.",
            SlicerError.NormalNotAVector => "L'inclinaison du plan de découpe doit être un vecteur valide.",
            SlicerError.OriginNotAVector => "La position du plan de découpe doit être un nombre valide.",
            SlicerError.MinThicknessNotANumber => "L'épaisseur minimum doit être un nombre valide.",
            SlicerError.NormalNotAligned => "Le plan de découpe doit être aligné à l'un des axes (X, Y ou Z).",
            SlicerError.NormalIsZero => "L'inclinaison du plan de découpe ne peut pas être égale à 0.",
            SlicerError.MeshIsNotClosed => "Le modèle doit être fermé pour procéder à une découpe.",
            
            _ => throw new ArgumentOutOfRangeException(nameof(error))
        };
    }
}