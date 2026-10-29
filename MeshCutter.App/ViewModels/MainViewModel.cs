using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Media3D;
using g3;
using CommunityToolkit.Mvvm.Input;
using MeshCutter.App.ViewModels.Interfaces;
using MeshCutter.Core;

namespace MeshCutter.App.ViewModels;


public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _informations = "Informations : ";
    [ObservableProperty]
    private MeshGeometry3D? _mesh;
    [ObservableProperty]
    private string? _fileErrorMessage;

    private readonly IFileDialogService _fileDialogService;

    private MeshInfo? _meshInfo; 

    [ObservableProperty]
    private double _printSizeX = 200;
    [ObservableProperty]
    private double _printSizeY = 200;
    [ObservableProperty]
    private double _printSizeZ = 200;

    [ObservableProperty]
    private double _marginX = 40;
    [ObservableProperty]
    private double _marginY = 60;
    [ObservableProperty]
    private double _marginZ = 20;

    [ObservableProperty]
    private int _piecesX;
    [ObservableProperty]
    private int _piecesY;
    [ObservableProperty]
    private int _piecesZ;
    [ObservableProperty]
    private string? _printErrorMessage;


    [ObservableProperty]
    private Axis _slicerAxis = Axis.X;
    [ObservableProperty]
    private double _slicerPosition = 5;
    [ObservableProperty]
    private DMesh3? _meshBase;
    [ObservableProperty]
    private DMesh3? _meshBottom;
    [ObservableProperty]
    private DMesh3? _meshTop;
    [ObservableProperty]
    private string? _slicerErrorMessage;






    public MainViewModel(IFileDialogService fileDialogService)
    {
        _fileDialogService = fileDialogService;
    }

    partial void OnPrintSizeXChanged(double value) => Recalculate();
    partial void OnPrintSizeYChanged(double value) => Recalculate();
    partial void OnPrintSizeZChanged(double value) => Recalculate();
    partial void OnMarginXChanged(double value) => Recalculate();
    partial void OnMarginYChanged(double value) => Recalculate();
    partial void OnMarginZChanged(double value) => Recalculate();

    private void Recalculate()
    {
        if (_meshInfo != null)
        {
            var errors = SpaceValidator.Validate(PrintSizeX, PrintSizeY, PrintSizeZ, MarginX, MarginY, MarginZ);
            if (errors.Count > 0)
            {
                PrintErrorMessage = SpaceErrorFormatter.Format(errors);
                PiecesX = 0;
                PiecesY = 0;
                PiecesZ = 0;
            }
            else
            {
                var margins = new Margins(MarginX, MarginY, MarginZ);
                var printVolume = new PrintVolume(PrintSizeX, PrintSizeY, PrintSizeZ);
                var cutPlanner = new CutPlanner(_meshInfo, printVolume, margins);
                PiecesX = cutPlanner.PiecesX;
                PiecesY = cutPlanner.PiecesY;
                PiecesZ = cutPlanner.PiecesZ;
                PrintErrorMessage = null;
            }
        }
    }
    
    [RelayCommand]
    public void OpenFile()
    {
        var path = _fileDialogService.GetFilePath();
        if (path != null)
        {
            LoadFile(path);
        }
    }

    public void LoadFile(string path)
    {
        var (result, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        if (result.code == IOCode.Ok && mesh != null)
        {
            _meshInfo = MeshInfo.GetMeshInfo(mesh);
            Informations = GetModelInfos();
            Mesh = Converter.MeshConverter(mesh);  
            FileErrorMessage = null;    
            Recalculate();
        }
        else
        {
            FileErrorMessage = $"Erreur lors du chargement du fichier : \ncode : {result.code} \nmessage : {result.message}";
        }
    }

    private string GetModelInfos()
    {
        var m = _meshInfo;
        if (m != null)
        {
            string modelInfos = @$"Informations : 

Nombre de sommets : {m.VertexCount}
Nombre de triangles : {m.TriangleCount}

Longueur (x) : {RoundedValue(m.SizeX)}mm
Hauteur (y) : {RoundedValue(m.SizeY)}mm
Profondeur (z) : {RoundedValue(m.SizeZ)}mm

Superficie : {RoundedValue(m.Area)}mm²
Volume : {DisplayVolume(m)}
";
            return modelInfos;
        }
        return "Informations : ";
    }

    private string DisplayVolume(MeshInfo meshInfo)
    {
        if (meshInfo.IsClosed && meshInfo.Volume != null)
        {
            return $"{RoundedValue(meshInfo.Volume.Value)}mm³";
        }
        else
        {
            return "Volume non disponible, le modèle est ouvert.";
        }
    }

    private double RoundedValue(double value)
    {
        return Math.Round(value, 2);
    }
}









