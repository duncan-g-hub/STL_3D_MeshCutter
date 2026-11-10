using CommunityToolkit.Mvvm.ComponentModel;
using g3;
using CommunityToolkit.Mvvm.Input;
using MeshCutter.App.ViewModels.Interfaces;

namespace MeshCutter.App.ViewModels;

public partial class PieceViewModel : BaseViewModel
{
    [ObservableProperty]
    private string? _saveErrorMessage;

    private readonly IFileDialogService _fileDialogService;

    public PieceViewModel(DMesh3 mesh, double sizeX, double sizeY, double sizeZ, 
    double marginX, double marginY, double marginZ,
        IFileDialogService fileDialogService, IPieceWindowService pieceWindowService)
        : base(pieceWindowService)
    {
        // Printer settings first, mesh last: _meshInfo is still null while these are assigned,
        // so Recalculate() does nothing. LoadMesh then runs a single real calculation.
        PrintSizeX = sizeX;
        PrintSizeY = sizeY;
        PrintSizeZ = sizeZ;
        MarginX = marginX;
        MarginY = marginY;
        MarginZ = marginZ;
        _fileDialogService = fileDialogService;
        LoadMesh(mesh);
    }

    [RelayCommand]
    public void SaveFile()
    {
        if (CurrentMesh != null)
        {
            string stlName = "New Model.STL";
            var fileName = _fileDialogService.GetSavePath(stlName);
            if (fileName != null)
            {
                List<DMesh3> meshes = new List<DMesh3>{CurrentMesh};
                var options = WriteOptions.Defaults;
                // options.bWriteBinary = true;
                var result = StandardMeshWriter.WriteMeshes(fileName, meshes, options);

                if (result.code == IOCode.Ok)
                {
                    SaveErrorMessage = null;
                }
                else
                {
                    SaveErrorMessage = $"Erreur lors de la sauvegarde du fichier : \ncode : {result.code} \nmessage : {result.message}";
                }
            }
        }
    }
}

