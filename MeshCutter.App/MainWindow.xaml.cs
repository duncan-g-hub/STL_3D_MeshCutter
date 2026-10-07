using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using g3;
using HelixToolkit;
using HelixToolkit.Wpf;
using MeshCutter.Core;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using System.IO;

namespace MeshCutter.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
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

        // HelixViewport3D




        // MessageBox.Show($"Sommets : {mesh.VertexCount}, triangles : {mesh.TriangleCount}");
        // var bounds = mesh.GetBounds();

        // // coordonnés des bornes + volume au sein de la boite englobante 
        // MessageBox.Show($"Boite englobante : longueur (x) : {bounds.Width}mm; hauteur (y) : {bounds.Height}mm; profondeur (z) : {bounds.Depth}mm ; volume : {bounds.Volume}mm³");
        
        // if (mesh.IsClosed())
        // {
        //     MessageBox.Show("Le volume est fermé !");
        //     var measures = MeshMeasurements.VolumeArea(mesh, mesh.TriangleIndices(), i => mesh.GetVertex(i));
        //     MessageBox.Show($"Volume : {measures[0]}mm³ ; superficie : {measures[1]}mm²");
        // }
        // else
        // {
        //     MessageBox.Show("Le volume est ouvert, impossible de continuer...");
        // }
    }
  

    private void OpenFileButton_Click(object sender, RoutedEventArgs e)
    {
        var fileDialog = new OpenFileDialog();
        fileDialog.Filter = "STL files (*.STL)|*.STL|All files (*.*)|*.*";
        if(fileDialog.ShowDialog() == true)
        {
            string STLPath = fileDialog.FileName;
            ReadSTLFile(STLPath);
        }
    }

    private void ReadSTLFile(string path)
    {
        var (result, mesh) = Exploration.ReadFile(path);
        if (result.code == IOCode.Ok && mesh != null)
        {
            DisplayModel(mesh);
        }
        else
        {
            MessageBox.Show($"code : {result.code} ; message : {result.message}");
        }
    }


    private void DisplayModel(DMesh3 mesh)
    {
        var myModel = Converter.MeshConverter(mesh);
        DisplayedModel.MeshGeometry = myModel;
        ZoomToModel(mesh);
    }

        private void ZoomToModel(DMesh3 mesh)
    {
        var b = mesh.GetBounds();
        var rect3d = new Rect3D(b.Min.x, b.Min.y, b.Min.z, b.Width, b.Height, b.Depth);
        Visualizer3D.ZoomExtents(rect3d);
    }
}

class Converter
{
    public static MeshGeometry3D MeshConverter(DMesh3 mesh)
    {
        // gestion sommets
        IList<Point3D> positions = new List<Point3D>();
        IDictionary<int,int> indexMap = new Dictionary<int,int>();

        foreach (int i in mesh.VertexIndices())
        {
            indexMap.Add(i, positions.Count);
            var v = mesh.GetVertex(i);
            positions.Add(new Point3D(v.x, v.y, v.z));
        }

        // gestion triangles 
        IList<int> triangleIndices = new List<int>();
        foreach (int i in mesh.TriangleIndices())
        {            
            var t = mesh.GetTriangle(i);
            triangleIndices.Add(indexMap[t.a]); triangleIndices.Add(indexMap[t.b]); triangleIndices.Add(indexMap[t.c]);
        }

        // nouveau mesh convertit
        var convertedPositions = new Point3DCollection(positions);
        var convertedTriangles = new Int32Collection(triangleIndices);
        var convertedMesh = new MeshGeometry3D{Positions=convertedPositions, TriangleIndices=convertedTriangles};
        
        return convertedMesh;
    }
}