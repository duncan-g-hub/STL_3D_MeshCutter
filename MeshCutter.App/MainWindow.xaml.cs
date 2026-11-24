using System.Windows;
using System.Windows.Media.Media3D;
using MeshCutter.App.ViewModels;
using System.ComponentModel;
using HelixToolkit.Wpf;

namespace MeshCutter.App;

public partial class MainWindow : Window
{
    private readonly BaseViewModel _viewModel;
    private readonly BoundingBoxWireFrameVisual3D _printVolumeWireframe;
    private readonly BoundingBoxWireFrameVisual3D _utilVolumeWireframe;

    public MainWindow(BaseViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();

        var referencePoint = new CoordinateSystemVisual3D();
        referencePoint.ArrowLengths = 20;
        Visualizer3D.Children.Add(referencePoint);

        _printVolumeWireframe = new BoundingBoxWireFrameVisual3D();
        _printVolumeWireframe.Color = System.Windows.Media.Color.FromArgb(250, 222, 222, 164);
        _printVolumeWireframe.Thickness = 1;
        Visualizer3D.Children.Add(_printVolumeWireframe);
        
        _utilVolumeWireframe = new BoundingBoxWireFrameVisual3D();
        _utilVolumeWireframe.Color = System.Windows.Media.Color.FromArgb(250, 180, 222, 164);
        _utilVolumeWireframe.Thickness = 2;
        Visualizer3D.Children.Add(_utilVolumeWireframe);

        // The mesh is added back last so that it is drawn after the elements it is intended to reveal.
        Visualizer3D.Children.Remove(MeshVisual);
        Visualizer3D.Children.Add(MeshVisual);

        DataContext = _viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;   
        UpdateVolumeBox();
        CameraOnFront();
        ZoomToPrintVolume();
    }  

    private void UpdateVolumeBox()
    {
        _printVolumeWireframe.BoundingBox = new Rect3D(_viewModel.PrintVolumeBox.Min.x, _viewModel.PrintVolumeBox.Min.y, _viewModel.PrintVolumeBox.Min.z, _viewModel.PrintVolumeBox.Width, _viewModel.PrintVolumeBox.Height, _viewModel.PrintVolumeBox.Depth);
        _utilVolumeWireframe.BoundingBox  = new Rect3D(_viewModel.UtilBox.Min.x, _viewModel.UtilBox.Min.y, _viewModel.UtilBox.Min.z, _viewModel.UtilBox.Width, _viewModel.UtilBox.Height, _viewModel.UtilBox.Depth);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.Mesh))       
        {
            ZoomToPrintVolume();
        }
        if (e.PropertyName == nameof(_viewModel.PrintVolumeBox) || e.PropertyName == nameof(_viewModel.UtilBox))
        {
            UpdateVolumeBox();
            ZoomToPrintVolume();
        } 
    }
  
    private void ZoomToPrintVolume()
    {
        const double scale = 1.1;
        const int divingCorrection = 5;
        var bBox = _printVolumeWireframe.BoundingBox;
        bBox.Size = new Size3D(bBox.SizeX * scale, bBox.SizeY * scale, bBox.SizeZ * scale);
        bBox.Location = new Point3D(bBox.X * scale, bBox.Y * scale, -bBox.SizeZ/divingCorrection);
        Visualizer3D.ZoomExtents(bBox);
    }

    private void CameraOnFront()
    {
        Visualizer3D.SetView(new Point3D(0,0,1), new Vector3D(0,1,-0.3), new Vector3D(0,1,0));
    }
}

