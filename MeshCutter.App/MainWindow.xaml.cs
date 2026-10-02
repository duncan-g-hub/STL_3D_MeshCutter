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
        
        var newVolume = new PrintVolume(220, 220, 250);
        MessageBox.Show($"Volume créé : {newVolume.Volume}mm³");
        try
        {
            var errorVolume = new PrintVolume(-5, 220, 250);
        }
        catch (ArgumentOutOfRangeException e)
        {
            MessageBox.Show($"Erreur : {e.Message}");
        }
    }
}