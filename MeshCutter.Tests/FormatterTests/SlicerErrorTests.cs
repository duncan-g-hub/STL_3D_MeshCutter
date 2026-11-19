using MeshCutter.App;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public class SlicerErrorFormatterTests
{
    [Fact]
    public void Format_WhenNoError_ReturnsNull()
    {
        var error = SlicerErrorFormatter.Format(null);
        Assert.Null(error);
    }

    [Fact]
    public void Format_TooLowSliceCoordinateError_ReturnsFormattedError()
    {
        var error = SlicerError.TooLowSliceCoordinate;
        Assert.Equal(
            "La position du plan de découpe doit être au sein du modèle, au minimum à une distance valide du bord.", 
            SlicerErrorFormatter.Format(error)
            );
    }

    [Fact]
    public void Format_TooHighSliceCoordinateError_ReturnsFormattedError()
    {
        var error = SlicerError.TooHighSliceCoordinate;
        Assert.Equal(
            "La position du plan de découpe doit être au sein du modèle, au maximum à une distance valide du bord.", 
            SlicerErrorFormatter.Format(error)
            );
    }

    [Fact]
    public void Format_NormalNotAVectorError_ReturnsFormattedError()
    {
        var error = SlicerError.NormalNotAVector;
        Assert.Equal(
            "L'inclinaison du plan de découpe doit être un vecteur valide.", 
            SlicerErrorFormatter.Format(error)
            );
    }

    [Fact]
    public void Format_OriginNotAVectorError_ReturnsFormattedError()
    {
        var error = SlicerError.OriginNotAVector;
        Assert.Equal(
            "La position du plan de découpe doit être un nombre valide.", 
            SlicerErrorFormatter.Format(error)
            );
    }

    [Fact]
    public void Format_MinThicknessNotANumberError_ReturnsFormattedError()
    {
        var error = SlicerError.MinThicknessNotANumber;
        Assert.Equal(
            "L'épaisseur minimum doit être un nombre valide.", 
            SlicerErrorFormatter.Format(error)
            );
    }

    [Fact]
    public void Format_NormalNotAlignedError_ReturnsFormattedError()
    {
        var error = SlicerError.NormalNotAligned;
        Assert.Equal(
            "Le plan de découpe doit être aligné à l'un des axes (X, Y ou Z).", 
            SlicerErrorFormatter.Format(error)
            );
    }

    [Fact]
    public void Format_NormalIsZeroError_ReturnsFormattedError()
    {
        var error = SlicerError.NormalIsZero;
        Assert.Equal(
            "L'inclinaison du plan de découpe ne peut pas être égale à 0.", 
            SlicerErrorFormatter.Format(error)
            );
    }
    
    [Fact]
    public void Format_MeshIsNotClosedError_ReturnsFormattedError()
    {
        var error = SlicerError.MeshIsNotClosed;
        Assert.Equal(
            "Le modèle doit être fermé pour procéder à une découpe.", 
            SlicerErrorFormatter.Format(error)
            );
    }

    [Fact]
    public void Format_ForEveryCombination_DoesNotThrow()
    {

        foreach (SlicerError slicerError in Enum.GetValues<SlicerError>())
        {
            var exception = Record.Exception(() => SlicerErrorFormatter.Format(slicerError));
            Assert.Null(exception);
        }
        
    }

    [Fact]
    public void Format_WhenInvalidValue_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => SlicerErrorFormatter.Format((SlicerError)999));
    }


}