using MeshCutter.App;
using MeshCutter.Core;

namespace MeshCutter.Tests;

public class SpaceErrorFormatterTests
{
    [Fact]
    public void Format_WhenNoError_ReturnsNull()
    {
        var errors = new List<SpaceError>();
        Assert.Null(SpaceErrorFormatter.Format(errors));
    }

    [Fact]
    public void Format_WhenOneError_ReturnsFormattedError()
    {
        var errors = new List<SpaceError>();

        errors.Add(new SpaceError(Axis.X, KindOfError.SizeNotPositive));
        Assert.Equal("Axe X : l'espace de l'imprimante doit être supérieur à 0mm.", SpaceErrorFormatter.Format(errors));
    }

    [Fact]
    public void Format_WhenThreeErrors_ReturnsFormattedErrors()
    {
        var errors = new List<SpaceError>();

        errors.Add(new SpaceError(Axis.X, KindOfError.MarginTooBig));
        errors.Add(new SpaceError(Axis.Y, KindOfError.SizeNotPositive));
        errors.Add(new SpaceError(Axis.Z, KindOfError.MarginNegative));

        Assert.Equal("Axe X : la marge doit être inférieure à l'espace de l'imprimante.\nAxe Y : l'espace de l'imprimante doit être supérieur à 0mm.\nAxe Z : la marge doit être supérieure ou égale à 0mm.", 
            SpaceErrorFormatter.Format(errors));
    }

    [Fact]
    public void Format_ForEveryCombination_DoesNotThrow()
    {
        foreach (Axis axis in Enum.GetValues<Axis>())
        {
            foreach (KindOfError kind in Enum.GetValues<KindOfError>())
            {
                var error = new SpaceError(axis, kind);
                var exception = Record.Exception(() => SpaceErrorFormatter.Format(new[] { error }));
                Assert.Null(exception);
            }
        }
    }

    [Fact]
    public void Format_WhenAxisIsUnknown_Throws()
    {
        var error = new SpaceError((Axis)99, KindOfError.SizeNotPositive);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => SpaceErrorFormatter.Format(new[] { error })
        );
    }

    [Fact]
    public void Format_WhenKindOfErrorIsUnknown_Throws()
    {
        var error = new SpaceError(Axis.X, (KindOfError)99);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => SpaceErrorFormatter.Format(new[] { error })
        );
    }
}