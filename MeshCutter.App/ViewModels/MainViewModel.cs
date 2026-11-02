using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Media3D;
using g3;
using CommunityToolkit.Mvvm.Input;
using MeshCutter.App.ViewModels.Interfaces;
using MeshCutter.Core;

namespace MeshCutter.App.ViewModels;


public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _informations = "Informations : ";
    [ObservableProperty]
    private MeshGeometry3D? _mesh;
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

    private readonly IPieceWindowService _pieceWindowService;


    protected BaseViewModel(IPieceWindowService pieceWindowService)
    {
        _pieceWindowService = pieceWindowService;
    }

    protected void LoadMesh(DMesh3 mesh)
    {
        MeshBase = mesh;
        _meshInfo = MeshInfo.GetMeshInfo(mesh);
        Informations = GetModelInfos();
        Mesh = Converter.MeshConverter(mesh);
        Recalculate();
    }

    public List<AxisChoice> Choices { get; } = new()
    {
        new(AxisFormatter.FormatAxis(Axis.X), Axis.X),
        new(AxisFormatter.FormatAxis(Axis.Y), Axis.Y),
        new(AxisFormatter.FormatAxis(Axis.Z), Axis.Z),
    };

    [RelayCommand]
    public void CutMesh()
    {
        if (MeshBase != null)
        {
            var (origin, normal) = GetOriginAndNormal();
            var error = SlicerValidator.Validate(origin, normal, MeshBase.GetBounds(), Settings.MinThickness, MeshBase.IsClosed());
            if (error != null)
            {
                SlicerErrorMessage = SlicerErrorFormatter.Format(error);
            }
            else
            {
                (MeshBottom, MeshTop, MeshBase) = MeshSlicer.Cut(MeshBase, origin, normal);
                SlicerErrorMessage = null;

                // appel de la gestion pour ouvrir les deux nouvelles fenetre en leur passant les nouvelles mesh 
                _pieceWindowService.OpenSlicedPiece(MeshBottom, PrintSizeX, PrintSizeY, PrintSizeZ, MarginX, MarginY, MarginZ);
                _pieceWindowService.OpenSlicedPiece(MeshTop, PrintSizeX, PrintSizeY, PrintSizeZ, MarginX, MarginY, MarginZ);
            }
        }
    }

    private (Vector3d, Vector3d) GetOriginAndNormal()
    {
        Vector3d origin = new Vector3d (0, 0, 0);
        Vector3d normal = new Vector3d (0, 0, 0);
        if (SlicerAxis == Axis.X)
        {
            origin.x = SlicerPosition;
            normal.x = 1;
        }
        else if (SlicerAxis == Axis.Y)
        {
            origin.y = SlicerPosition;
            normal.y = 1;
        }
        else if (SlicerAxis == Axis.Z)
        {
            origin.z = SlicerPosition;
            normal.z = 1;
        }
        return (origin, normal);
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




public partial class MainViewModel : BaseViewModel
{
    [ObservableProperty]
    private string? _fileErrorMessage;

    private readonly IFileDialogService _fileDialogService;

   

    public MainViewModel(IFileDialogService fileDialogService, IPieceWindowService pieceWindowService)
        : base(pieceWindowService)
    {
        _fileDialogService = fileDialogService;
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
            LoadMesh(mesh);
            FileErrorMessage = null;
        }
        else
        {
            FileErrorMessage = $"Erreur lors du chargement du fichier : \ncode : {result.code} \nmessage : {result.message}";
        }
    }

}


public partial class PieceViewModel : BaseViewModel
{
    public PieceViewModel(DMesh3 mesh, double sizeX, double sizeY, double sizeZ, 
    double marginX, double marginY, double marginZ, IPieceWindowService pieceWindowService)
        : base(pieceWindowService)
    {
        PrintSizeX = sizeX;
        PrintSizeY = sizeY;
        PrintSizeZ = sizeZ;
        MarginX = marginX;
        MarginY = marginY;
        MarginZ = marginZ;
        LoadMesh(mesh);
    }
}

