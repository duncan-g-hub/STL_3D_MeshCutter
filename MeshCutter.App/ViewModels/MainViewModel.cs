using CommunityToolkit.Mvvm.ComponentModel;
using MeshCutter.Core;
using System.Windows.Media.Media3D;
using g3;

namespace MeshCutter.App.ViewModels;
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _informations = "Informations : ";
    [ObservableProperty]
    private MeshGeometry3D? _mesh;
    [ObservableProperty]
    private string? _errorMessage;

    public void LoadFile(string path)
    {
        var (result, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        if (result.code == IOCode.Ok && mesh != null)
        {
            Informations = GetModelInfos(mesh);
            Mesh = Converter.MeshConverter(mesh);        
            ErrorMessage = null;    
        }
        else
        {
            ErrorMessage = $"Erreur lors du chargement du fichier : \ncode : {result.code} \nmessage : {result.message}";
        }
    }

    private string GetModelInfos(DMesh3 mesh)
    {
        var m = MeshInfo.GetMeshInfo(mesh);

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

