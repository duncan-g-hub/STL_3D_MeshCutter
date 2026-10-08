using System.Windows;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using MeshCutter.App.ViewModels;
using System.ComponentModel;

namespace MeshCutter.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel = new();
    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged; 

        // var newVolume = new PrintVolume(220, 220, 250);
        // MessageBox.Show($"Volume créé : {newVolume.Volume}mm³");
        // try
        // {
        //     var errorVolume = new PrintVolume(-5, 220, 250);
        // }
        // catch (ArgumentOutOfRangeException e)
        // {
        //     MessageBox.Show($"Erreur : {e.Message}");
        // }
    }
    
    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.Mesh))       
        {
            ZoomToModel(_viewModel.Mesh);
        }
    }
  
    private void OpenFileButton_Click(object sender, RoutedEventArgs e)
    {
        var fileDialog = new OpenFileDialog();
        fileDialog.Filter = "STL files (*.STL)|*.STL|All files (*.*)|*.*";
        if(fileDialog.ShowDialog() == true)
        {
            string STLPath = fileDialog.FileName;
            _viewModel.LoadFile(STLPath);
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

