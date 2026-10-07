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
        var (result, mesh) = MeshLoader.GetMeshFromSTLFile(path);
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
        DisplayModelInfos(mesh);
        ZoomToModel(mesh);
    }

    private void ZoomToModel(DMesh3 mesh)
    {
        var b = mesh.GetBounds();
        var rect3d = new Rect3D(b.Min.x, b.Min.y, b.Min.z, b.Width, b.Height, b.Depth);
        Visualizer3D.ZoomExtents(rect3d);
    }

    private void DisplayModelInfos(DMesh3 mesh)
    {
        var m = MeshInfo.GetMeshInfo(mesh);

        ModelInfos.Text = @$"Informations : 

Nombre de sommets : {m.VertexCount}
Nombre de triangles : {m.TriangleCount}

Longueur (x) : {RoundedValue(m.SizeX)}mm
Hauteur (y) : {RoundedValue(m.SizeY)}mm
Profondeur (z) : {RoundedValue(m.SizeZ)}mm

Superficie : {RoundedValue(m.Area)}mm²
Volume : {DisplayVolume(m)}
";
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