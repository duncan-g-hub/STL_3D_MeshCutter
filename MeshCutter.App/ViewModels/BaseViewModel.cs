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
    private DMesh3? _currentMesh;
    [ObservableProperty]
    private DMesh3? _meshBottom;
    [ObservableProperty]
    private DMesh3? _meshTop;
    [ObservableProperty]
    private string? _slicerErrorMessage;
    
    private readonly IPieceWindowService _pieceWindowService;

    [ObservableProperty]
    private History? _sharedHistory;
    [ObservableProperty]
    private PieceHistory? _currentPieceHistory;

    [ObservableProperty]
    private AxisAlignedBox3d _printVolumeBox;
    [ObservableProperty]
    private AxisAlignedBox3d _utilBox;



    protected BaseViewModel(IPieceWindowService pieceWindowService)
    {
        _pieceWindowService = pieceWindowService;
        GetBoxes();
    }

    private void GetBoxes()
    {
        var errors = SpaceValidator.Validate(PrintSizeX, PrintSizeY, PrintSizeZ, MarginX, MarginY, MarginZ);
        if (errors.Count > 0)
        {
            PrintErrorMessage = SpaceErrorFormatter.Format(errors);
        }
        else
        {
            var margins = new Margins(MarginX, MarginY, MarginZ);
            var printVolume = new PrintVolume(PrintSizeX, PrintSizeY, PrintSizeZ);
            var (printVolumeBox, utilBox) = UtilVolumeBox.CreatePrintVolumeAndUtilBoxes(printVolume, margins);
            PrintVolumeBox = printVolumeBox;
            UtilBox = utilBox;
            PrintErrorMessage = null;
        }
    }

    protected void LoadMesh(DMesh3 mesh)
    {
        CurrentMesh = mesh;
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
        if (CurrentMesh != null)
        {
            var (origin, normal) = GetOriginAndNormal();
            var error = SlicerValidator.Validate(origin, normal, CurrentMesh.GetBounds(), Settings.MinThickness, CurrentMesh.IsClosed());
            if (error != null)
            {
                SlicerErrorMessage = SlicerErrorFormatter.Format(error);
            }
            else
            {
                (MeshBottom, MeshTop, CurrentMesh) = MeshSlicer.Cut(CurrentMesh, origin, normal);
                MoveMesh.CenterMesh(MeshBottom);
                MoveMesh.CenterMesh(MeshTop);
                SlicerErrorMessage = null;
                if(SharedHistory != null && CurrentPieceHistory != null)
                {
                    var (historyBottom, historyTop) = SharedHistory.AddSlice(CurrentPieceHistory, SlicerAxis, SlicerPosition, MeshBottom, MeshTop);
                    _pieceWindowService.OpenSlicedPiece(MeshBottom, PrintSizeX, PrintSizeY, PrintSizeZ, MarginX, MarginY, MarginZ, historyBottom, SharedHistory);
                    _pieceWindowService.OpenSlicedPiece(MeshTop, PrintSizeX, PrintSizeY, PrintSizeZ, MarginX, MarginY, MarginZ, historyTop, SharedHistory);
                }
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

    partial void OnPrintSizeXChanged(double value) => UpdatePrinterSpace();   
    partial void OnPrintSizeYChanged(double value) => UpdatePrinterSpace();
    partial void OnPrintSizeZChanged(double value) => UpdatePrinterSpace();
    partial void OnMarginXChanged(double value) => UpdatePrinterSpace();
    partial void OnMarginYChanged(double value) => UpdatePrinterSpace();
    partial void OnMarginZChanged(double value) => UpdatePrinterSpace();


    private void UpdatePrinterSpace()
    {
        Recalculate(); 
        GetBoxes();
    }

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
