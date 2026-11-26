using MeshCutter.App;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public class AngleErrorFormatterTests
{
    [Fact]
    public void Format_WhenNoError_ReturnsNull()
    {
        Assert.Null(AngleErrorFormatter.Format(null));
    }

    [Fact]
    public void Format_AngleNotANumberError_ReturnsFormattedError()
    {
        Assert.Equal(
            "L'angle de rotation doit être un nombre valide.",
            AngleErrorFormatter.Format(AngleError.AngleNotANumber)
            );
    }

    [Fact]
    public void Format_MeshIsNotClosedError_ReturnsFormattedError()
    {
        Assert.Equal(
            "Le modèle doit être fermé pour procéder à une rotation.",
            AngleErrorFormatter.Format(AngleError.MeshIsNotClosed)
            );
    }

    [Fact]
    public void Format_ForEveryError_DoesNotThrow()
    {
        foreach (AngleError angleError in Enum.GetValues<AngleError>())
        {
            var exception = Record.Exception(() => AngleErrorFormatter.Format(angleError));
            Assert.Null(exception);
        }
    }

    [Fact]
    public void Format_WhenInvalidValue_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => AngleErrorFormatter.Format((AngleError)999));
    }
}