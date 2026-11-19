using MeshCutter.Tests.Fakes;
using MeshCutter.App.ViewModels;
using MeshCutter.Core;
using MeshCutter.App;

namespace MeshCutter.Tests;

public partial class FileViewModelTests
{
    [Fact]
    public void CutMesh_WhenNoFileLoaded_DoNothing()
    {
        FileViewModel viewModel = new(new FakeFileDialogService(null, null, null), new FakePieceWindowService());
        viewModel.CutMesh();
        Assert.Null(viewModel.SlicerErrorMessage);
        Assert.Null(viewModel.MeshBottom);
        Assert.Null(viewModel.MeshTop);
        Assert.Null(viewModel.CurrentMesh);
    }

    [Fact]
    public void CutMesh_WhenConfigurationIsValid_ReturnsClosedTopAndBottomMeshes()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();

        Assert.NotNull(viewModel.CurrentMesh);
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();
        Assert.Null(viewModel.SlicerErrorMessage);

        Assert.NotNull(viewModel.MeshBottom);
        Assert.True(viewModel.MeshBottom.IsClosed());

        Assert.NotNull(viewModel.MeshTop);
        Assert.True(viewModel.MeshTop.IsClosed());
    }

    [Fact]
    public void CutMeshOnZ_WhenConfigurationIsValid_ReturnsTopAndBottomBounds()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();

        Assert.NotNull(viewModel.CurrentMesh);
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();
        Assert.Null(viewModel.SlicerErrorMessage);

        Assert.NotNull(viewModel.MeshBottom);
        Assert.Equal(0, viewModel.MeshBottom.GetBounds().Min.z, 1e-3);
        Assert.Equal(25, viewModel.MeshBottom.GetBounds().Max.z, 1e-3);

        Assert.NotNull(viewModel.MeshTop);
        Assert.Equal(0, viewModel.MeshTop.GetBounds().Min.z, 1e-3);
        Assert.Equal(25, viewModel.MeshTop.GetBounds().Max.z, 1e-3);
    }

    [Fact]
    public void CutMeshOnX_WhenConfigurationIsValid_ReturnsTopAndBottomBounds()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();

        Assert.NotNull(viewModel.CurrentMesh);
        viewModel.SlicerAxis = Axis.X;
        viewModel.SlicerPosition = 0;

        viewModel.CutMesh();
        Assert.Null(viewModel.SlicerErrorMessage);

        Assert.NotNull(viewModel.MeshBottom);
        Assert.Equal(-12.5, viewModel.MeshBottom.GetBounds().Min.x, 1e-3);
        Assert.Equal(12.5, viewModel.MeshBottom.GetBounds().Max.x, 1e-3);

        Assert.NotNull(viewModel.MeshTop);
        Assert.Equal(-12.5, viewModel.MeshTop.GetBounds().Min.x, 1e-3);
        Assert.Equal(12.5, viewModel.MeshTop.GetBounds().Max.x, 1e-3);
    }

    [Fact]
    public void CutMeshOnY_WhenConfigurationIsValid_ReturnsTopAndBottomBounds()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();

        Assert.NotNull(viewModel.CurrentMesh);
        viewModel.SlicerAxis = Axis.Y;
        viewModel.SlicerPosition = 0;

        viewModel.CutMesh();
        Assert.Null(viewModel.SlicerErrorMessage);

        Assert.NotNull(viewModel.MeshBottom);
        Assert.Equal(-12.5, viewModel.MeshBottom.GetBounds().Min.y, 1e-3);
        Assert.Equal(12.5, viewModel.MeshBottom.GetBounds().Max.y, 1e-3);

        Assert.NotNull(viewModel.MeshTop);
        Assert.Equal(-12.5, viewModel.MeshTop.GetBounds().Min.y, 1e-3);
        Assert.Equal(12.5, viewModel.MeshTop.GetBounds().Max.y, 1e-3);
    }

    [Fact]
    public void CutMesh_WhenConfigurationIsValid_ReturnsValidVolumes()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();

        Assert.NotNull(viewModel.CurrentMesh);
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();
        Assert.Null(viewModel.SlicerErrorMessage);

        Assert.NotNull(viewModel.MeshBottom);
        var volumeBottom = MeshInfo.GetMeshInfo(viewModel.MeshBottom).Volume;
        Assert.NotNull(volumeBottom);
        Assert.Equal(62500, volumeBottom.Value, 1e-3);

        Assert.NotNull(viewModel.MeshTop);
        var volumeTop = MeshInfo.GetMeshInfo(viewModel.MeshTop).Volume;
        Assert.NotNull(volumeTop);
        Assert.Equal(62500, volumeTop.Value, 1e-3);

        var volumeInitial = MeshInfo.GetMeshInfo(viewModel.CurrentMesh).Volume;
        Assert.NotNull(volumeInitial);
        Assert.Equal(volumeInitial.Value, volumeBottom.Value + volumeTop.Value, 1e-3);
    }

    [Fact]
    public void CutMesh_WhenSlicerPositionTooHigh_ReturnsFormattedError()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();

        Assert.NotNull(viewModel.CurrentMesh);
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 50;

        viewModel.CutMesh();
        Assert.Equal("La position du plan de découpe doit être au sein du modèle, au maximum à une distance valide du bord.", viewModel.SlicerErrorMessage);
        Assert.Null(viewModel.MeshBottom);
        Assert.Null(viewModel.MeshTop);
    }

    private static readonly double[] NotFiniteValues =
    {
        double.NaN, double.PositiveInfinity, double.NegativeInfinity
    };

    [Fact]
    public void CutMesh_WhenPositionIsNotNumber_ReturnsFormattedError()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();

        Assert.NotNull(viewModel.CurrentMesh);
        viewModel.SlicerAxis = Axis.Z;
        foreach (double bad in NotFiniteValues)
        {
            viewModel.SlicerPosition = bad;
            viewModel.CutMesh();
            Assert.Equal("La position du plan de découpe doit être un nombre valide.", viewModel.SlicerErrorMessage);
            Assert.Null(viewModel.MeshBottom);
            Assert.Null(viewModel.MeshTop);
        }
    }

    [Fact]
    public void CutMesh_WhenMeshIsNotClosed_ReturnsFormattedError()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm trou.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();

        Assert.NotNull(viewModel.CurrentMesh);
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;
        viewModel.CutMesh();

        Assert.Equal("Le modèle doit être fermé pour procéder à une découpe.", viewModel.SlicerErrorMessage);
        Assert.Null(viewModel.MeshBottom);
        Assert.Null(viewModel.MeshTop);
    }

    [Fact]
    public void CutMesh_WhenSecondCutIsInvalid_KeepsFirstResults()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        FileViewModel viewModel = new(new FakeFileDialogService(path, null, null), new FakePieceWindowService());
        viewModel.OpenFile();

        Assert.NotNull(viewModel.CurrentMesh);
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();
        Assert.Null(viewModel.SlicerErrorMessage);
        Assert.NotNull(viewModel.MeshBottom);
        Assert.Equal(0, viewModel.MeshBottom.GetBounds().Min.z, 1e-3);
        Assert.Equal(25, viewModel.MeshBottom.GetBounds().Max.z, 1e-3);
        Assert.NotNull(viewModel.MeshTop);
        Assert.Equal(0, viewModel.MeshTop.GetBounds().Min.z, 1e-3);
        Assert.Equal(25, viewModel.MeshTop.GetBounds().Max.z, 1e-3);

        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 100;
        viewModel.CutMesh();
        Assert.Equal("La position du plan de découpe doit être au sein du modèle, au maximum à une distance valide du bord.", viewModel.SlicerErrorMessage);
        Assert.NotNull(viewModel.MeshBottom);
        Assert.Equal(0, viewModel.MeshBottom.GetBounds().Min.z, 1e-3);
        Assert.Equal(25, viewModel.MeshBottom.GetBounds().Max.z, 1e-3);
        Assert.NotNull(viewModel.MeshTop);
        Assert.Equal(0, viewModel.MeshTop.GetBounds().Min.z, 1e-3);
        Assert.Equal(25, viewModel.MeshTop.GetBounds().Max.z, 1e-3);
    }

    [Fact]
    public void Choices_ReturnsThreeAxisChoicesWithFormattedNames()
    {
        FileViewModel viewModel = new(new FakeFileDialogService(null, null, null), new FakePieceWindowService());
        Assert.Equal(3, viewModel.Choices.Count);
        Assert.Contains(new AxisChoice("Axe X", Axis.X), viewModel.Choices);
        Assert.Contains(new AxisChoice("Axe Y", Axis.Y), viewModel.Choices);
        Assert.Contains(new AxisChoice("Axe Z", Axis.Z), viewModel.Choices);
    }
}