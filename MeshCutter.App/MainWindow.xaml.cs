using System.Windows;
using System.Windows.Media.Media3D;
using MeshCutter.App.ViewModels;
using System.ComponentModel;
using MeshCutter.App.Services;

namespace MeshCutter.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel = new(new WpfFileDialogService());
    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged; 
    }
    
    

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.Mesh))       
        {
            ZoomToModel(_viewModel.Mesh);
        }
    }
  
    private void ZoomToModel(MeshGeometry3D? mesh)
    {
        if (mesh != null)
        {
            Visualizer3D.ZoomExtents(mesh.Bounds);
        }
    }

}

