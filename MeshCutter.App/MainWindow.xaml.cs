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
using MeshCutter.Core;

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

        var (result, mesh) = Exploration.ReadFile();
        MessageBox.Show($"code : {result.code} ; message : {result.message}");

        if (result.code == IOCode.Ok && mesh != null)
            {
                MessageBox.Show($"Sommets : {mesh.VertexCount}, triangles : {mesh.TriangleCount}");

                var bounds = mesh.GetBounds();

                // coordonnés des bornes + volume au sein de la boite englobante 
                MessageBox.Show($"Boite englobante : longueur (x) : {bounds.Width}mm; hauteur (y) : {bounds.Height}mm; profondeur (z) : {bounds.Depth}mm ; volume : {bounds.Volume}mm³");
                
                if (mesh.IsClosed())
                {
                    MessageBox.Show("Le volume est fermé !");
                    var measures = MeshMeasurements.VolumeArea(mesh, mesh.TriangleIndices(), i => mesh.GetVertex(i));
                    MessageBox.Show($"Volume : {measures[0]}mm³ ; superficie : {measures[1]}mm²");
                }
                else
                {
                    MessageBox.Show("Le volume est ouvert, impossible de continuer...");
                }
            }
    }
}