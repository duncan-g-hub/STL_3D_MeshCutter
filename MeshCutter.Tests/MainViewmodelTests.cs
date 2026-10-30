using MeshCutter.Tests.Fakes;
using MeshCutter.App.ViewModels;
using MeshCutter.Core;
using MeshCutter.App;

namespace MeshCutter.Tests;

public class MainViewModelUnitTest
{
    [Fact]
    public void OpenFile_WhenUserCancels_ChangesNothing()
    {
        MainViewModel viewModel = new(new FakeFileDialogService(null));
        viewModel.OpenFile();
        Assert.Null(viewModel.Mesh);
        Assert.Null(viewModel.FileErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }

    [Fact]
    public void OpenFile_WhenPathDoesNotExist_SetsErrorMessage()
    {
        MainViewModel viewModel = new(new FakeFileDialogService("Wrong Path"));
        viewModel.OpenFile();
        Assert.Null(viewModel.Mesh);
        Assert.NotNull(viewModel.FileErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }

    [Fact]
    public void OpenFile_WhenPathExist_GetMeshAndInfos()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        Assert.NotNull(viewModel.Mesh);
        Assert.Null(viewModel.FileErrorMessage);
        Assert.NotEqual("Informations : ", viewModel.Informations);
        Assert.Contains("Nombre de triangles : 12", viewModel.Informations);
        Assert.Contains("Nombre de sommets : 8", viewModel.Informations);
    }

    [Fact]
    public void OpenFile_WhenFormatIsUnsupported_SetsErrorMessage()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.SLDPRT");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        Assert.Null(viewModel.Mesh);
        Assert.NotNull(viewModel.FileErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }

    [Fact]
    public void OpenFile_WhenFileIsCorrupted_SetsErrorMessage()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm error.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        Assert.Null(viewModel.Mesh);
        Assert.NotNull(viewModel.FileErrorMessage);
        Assert.Equal("Informations : ", viewModel.Informations);
    }

    [Fact]
    public void CutPlanMesh_WhenMeshSmallerThanPrintableVolume_ReturnOnePiece()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        viewModel.PrintSizeX = 180;
        viewModel.MarginX = 40;
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(1, viewModel.PiecesX);
    }

    [Fact]
    public void CutPlanMesh_WhenMeshBiggerThanPrintableVolume_ReturnTwoPieces()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        viewModel.PrintSizeX = 80;
        viewModel.MarginX = 40;
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(2, viewModel.PiecesX);
    }

    [Fact]
    public void CutPlanMesh_WhenPrintableVolumeEqualZero_SetsError()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        viewModel.PrintSizeX = 0;
        viewModel.MarginX = 40;
        Assert.Equal("Axe X : l'espace de l'imprimante doit être supérieur à 0mm.",viewModel.PrintErrorMessage);
        Assert.Equal(0, viewModel.PiecesX);
        Assert.Equal(0, viewModel.PiecesY);
        Assert.Equal(0, viewModel.PiecesZ);
        viewModel.PrintSizeX = 200;
        Assert.Null(viewModel.PrintErrorMessage);
    }

    [Fact]
    public void UpdateMarginsAndPrintableVolume_WhenNoFileLoaded_ReturnZeroPiece()
    {
        MainViewModel viewModel = new(new FakeFileDialogService(null));
        viewModel.PrintSizeX = 0;
        viewModel.MarginX = -5;
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(0, viewModel.PiecesX);
    }

    [Fact]
    public void LoadFile_WhenSecondFileIsInvalid_KeepsFirstModel()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(1, viewModel.PiecesX);
        Assert.Equal(1, viewModel.PiecesY);
        Assert.Equal(1, viewModel.PiecesZ);

        string pathError = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm error.STL");
        viewModel.LoadFile(pathError);
        Assert.NotNull(viewModel.Mesh);
        Assert.NotNull(viewModel.FileErrorMessage);
        Assert.NotEqual("Informations : ", viewModel.Informations);
        Assert.Contains("Nombre de triangles : 12", viewModel.Informations);
        Assert.Contains("Nombre de sommets : 8", viewModel.Informations);
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(1, viewModel.PiecesX);
        Assert.Equal(1, viewModel.PiecesY);
        Assert.Equal(1, viewModel.PiecesZ);
        Assert.NotNull(viewModel.MeshBase);
    }

    [Fact]
    public void CutPlanMesh_WhenInvalidValueIsCorrected_ErrorDisappearsAndPiecesComeBack()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();

        viewModel.PrintSizeX = 0;
        Assert.NotNull(viewModel.PrintErrorMessage);
        Assert.Equal(0, viewModel.PiecesX);

        viewModel.PrintSizeX = 200;
        Assert.Null(viewModel.PrintErrorMessage);
        Assert.Equal(1, viewModel.PiecesX);
    }

    [Fact]
    public void CutPlanMesh_WhenSeveralValuesAreInvalid_ShowsAllErrors()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();

        viewModel.PrintSizeX = 0;
        viewModel.MarginZ = -5;

        Assert.Equal(
            "Axe X : l'espace de l'imprimante doit être supérieur à 0mm.\n" +
            "Axe Z : la marge doit être supérieure ou égale à 0mm.",
            viewModel.PrintErrorMessage);
    }

    [Fact]
    public void CutPlanMesh_WhenPrintSizeIsNotANumber_SetsError()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();

        viewModel.PrintSizeX = double.NaN;

        Assert.Equal("Axe X : l'espace de l'imprimante doit être un nombre valide.", viewModel.PrintErrorMessage);
        Assert.Equal(0, viewModel.PiecesX);
        Assert.Equal(0, viewModel.PiecesY);
        Assert.Equal(0, viewModel.PiecesZ);
    }

    [Fact]
    public void CutPlanMesh_WhenMarginIsInfinite_SetsError()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();

        viewModel.MarginY = double.PositiveInfinity;

        Assert.Equal("Axe Y : la marge doit être un nombre valide.", viewModel.PrintErrorMessage);
        Assert.Equal(0, viewModel.PiecesX);
    }



    [Fact]
    public void CutMesh_WhenNoFileLoaded_DoNothing()
    {
        MainViewModel viewModel = new(new FakeFileDialogService(null));
        viewModel.CutMesh();
        Assert.Null(viewModel.SlicerErrorMessage);
        Assert.Null(viewModel.MeshBottom);
        Assert.Null(viewModel.MeshTop);
        Assert.Null(viewModel.MeshBase);
    }

    [Fact]
    public void CutMesh_WhenConfigurationIsValid_ReturnsClosedTopAndBottomMeshes()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        
        Assert.NotNull(viewModel.MeshBase);
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
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        
        Assert.NotNull(viewModel.MeshBase);
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();
        Assert.Null(viewModel.SlicerErrorMessage);

        Assert.NotNull(viewModel.MeshBottom);
        Assert.Equal(0, viewModel.MeshBottom.GetBounds().Min.z, 1e-3);
        Assert.Equal(25, viewModel.MeshBottom.GetBounds().Max.z, 1e-3);

        Assert.NotNull(viewModel.MeshTop);
        Assert.Equal(25, viewModel.MeshTop.GetBounds().Min.z, 1e-3);
        Assert.Equal(50, viewModel.MeshTop.GetBounds().Max.z, 1e-3);
    }


    [Fact]
    public void CutMeshOnX_WhenConfigurationIsValid_ReturnsTopAndBottomBounds()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        
        Assert.NotNull(viewModel.MeshBase);
        viewModel.SlicerAxis = Axis.X;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();
        Assert.Null(viewModel.SlicerErrorMessage);

        Assert.NotNull(viewModel.MeshBottom);
        Assert.Equal(0, viewModel.MeshBottom.GetBounds().Min.x, 1e-3);
        Assert.Equal(25, viewModel.MeshBottom.GetBounds().Max.x, 1e-3);

        Assert.NotNull(viewModel.MeshTop);
        Assert.Equal(25, viewModel.MeshTop.GetBounds().Min.x, 1e-3);
        Assert.Equal(50, viewModel.MeshTop.GetBounds().Max.x, 1e-3);
    }

    [Fact]
    public void CutMeshOnY_WhenConfigurationIsValid_ReturnsTopAndBottomBounds()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        
        Assert.NotNull(viewModel.MeshBase);
        viewModel.SlicerAxis = Axis.Y;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();
        Assert.Null(viewModel.SlicerErrorMessage);

        Assert.NotNull(viewModel.MeshBottom);
        Assert.Equal(0, viewModel.MeshBottom.GetBounds().Min.y, 1e-3);
        Assert.Equal(25, viewModel.MeshBottom.GetBounds().Max.y, 1e-3);

        Assert.NotNull(viewModel.MeshTop);
        Assert.Equal(25, viewModel.MeshTop.GetBounds().Min.y, 1e-3);
        Assert.Equal(50, viewModel.MeshTop.GetBounds().Max.y, 1e-3);
    }


    [Fact]
    public void CutMesh_WhenConfigurationIsValid_ReturnsValidVolumes()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        
        Assert.NotNull(viewModel.MeshBase);
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

        var volumeInitial = MeshInfo.GetMeshInfo(viewModel.MeshBase).Volume;
        Assert.NotNull(volumeInitial);
        Assert.Equal(volumeInitial.Value, volumeBottom.Value + volumeTop.Value, 1e-3);
    }

    [Fact]
    public void CutMesh_WhenSlicerPositionTooHigh_ReturnsFormattedError()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "samples", "cube 50mm.STL");
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        
        Assert.NotNull(viewModel.MeshBase);
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
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        
        Assert.NotNull(viewModel.MeshBase);
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
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();

        Assert.NotNull(viewModel.MeshBase);
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
        MainViewModel viewModel = new(new FakeFileDialogService(path));
        viewModel.OpenFile();
        
        Assert.NotNull(viewModel.MeshBase);
        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 25;

        viewModel.CutMesh();
        Assert.Null(viewModel.SlicerErrorMessage);
        Assert.NotNull(viewModel.MeshBottom);
        Assert.Equal(0, viewModel.MeshBottom.GetBounds().Min.z, 1e-3);
        Assert.Equal(25, viewModel.MeshBottom.GetBounds().Max.z, 1e-3);
        Assert.NotNull(viewModel.MeshTop);
        Assert.Equal(25, viewModel.MeshTop.GetBounds().Min.z, 1e-3);
        Assert.Equal(50, viewModel.MeshTop.GetBounds().Max.z, 1e-3);

        viewModel.SlicerAxis = Axis.Z;
        viewModel.SlicerPosition = 100;
        viewModel.CutMesh();
        Assert.NotNull(viewModel.SlicerErrorMessage);
        Assert.NotNull(viewModel.MeshBottom);
        Assert.Equal(0, viewModel.MeshBottom.GetBounds().Min.z, 1e-3);
        Assert.Equal(25, viewModel.MeshBottom.GetBounds().Max.z, 1e-3);
        Assert.NotNull(viewModel.MeshTop);
        Assert.Equal(25, viewModel.MeshTop.GetBounds().Min.z, 1e-3);
        Assert.Equal(50, viewModel.MeshTop.GetBounds().Max.z, 1e-3);
    }

    [Fact]
    public void Choices_ReturnsThreeAxisChoicesWithFormattedNames()
    {
        MainViewModel viewModel = new(new FakeFileDialogService(null));

        Assert.Equal(3, viewModel.Choices.Count);
        Assert.Contains(new AxisChoice("Axe X", Axis.X), viewModel.Choices);
        Assert.Contains(new AxisChoice("Axe Y", Axis.Y), viewModel.Choices);
        Assert.Contains(new AxisChoice("Axe Z", Axis.Z), viewModel.Choices);
    }
}

