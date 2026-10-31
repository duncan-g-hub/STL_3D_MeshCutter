using System.Windows;
using System.Windows.Media.Media3D;
using MeshCutter.App.ViewModels;
using System.ComponentModel;
using MeshCutter.App.Services;

using MeshCutter.Core;
using g3;
using System.Windows.Controls;

namespace MeshCutter.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel = new(new WpfFileDialogService());
    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;     
        explo();
    }
    
    
    public void explo()
    {
        // string path = @"C:\Users\Supa\Desktop\Apprentissage_DEV\C#\Projets\STL_3D_MeshCutter\samples\cube 50mm trou.STL";
        // var (result, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        // var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(0, 0, 30), new Vector3d(0, 0, 1));
        // var botInfo = MeshInfo.GetMeshInfo(meshBottom);
        // MessageBox.Show($"{meshBottom.TriangleCount} {meshBottom.VertexCount} {meshBottom.IsClosed()} {botInfo.Volume}"); // 12 12 false
        // var topInfo = MeshInfo.GetMeshInfo(meshTop);
        // MessageBox.Show($"{meshTop.TriangleCount} {meshTop.VertexCount} {meshTop.IsClosed()} {topInfo.Volume}"); // 13 12 false
        
        // string path = @"C:\Users\Supa\Desktop\Apprentissage_DEV\C#\Projets\STL_3D_MeshCutter\samples\cube 50mm.STL";
        // var (result, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        // var (meshBottom, meshTop, meshBase) = MeshSlicer.Cut(mesh, new Vector3d(0, 0, 100), new Vector3d(0, 0, 1));
        // MessageBox.Show($"{meshBase.TriangleCount} {meshBase.VertexCount} {meshBase.IsClosed()}"); // 12 8 true
        // MessageBox.Show($"{meshBottom.TriangleCount} {meshBottom.VertexCount} {meshBottom.IsClosed()}"); // 12 8 true
        // MessageBox.Show($"{meshTop.TriangleCount} {meshTop.VertexCount} {meshTop.IsClosed()}"); // 0 0 false
    
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

