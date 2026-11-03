using CommunityToolkit.Mvvm.ComponentModel;
using g3;
using CommunityToolkit.Mvvm.Input;
using MeshCutter.App.ViewModels.Interfaces;

namespace MeshCutter.App.ViewModels;

public partial class PieceViewModel : BaseViewModel
{
    [ObservableProperty]
    private string? _saveErrorMessage;

    public PieceViewModel(DMesh3 mesh, double sizeX, double sizeY, double sizeZ, 
    double marginX, double marginY, double marginZ, IPieceWindowService pieceWindowService)
        : base(pieceWindowService)
    {
        PrintSizeX = sizeX;
        PrintSizeY = sizeY;
        PrintSizeZ = sizeZ;
        MarginX = marginX;
        MarginY = marginY;
        MarginZ = marginZ;
        LoadMesh(mesh);
    }

    [RelayCommand]
    public void SaveFile()
    {
        // a venir
    }
}

