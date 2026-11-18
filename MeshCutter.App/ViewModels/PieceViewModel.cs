using CommunityToolkit.Mvvm.ComponentModel;
using g3;
using CommunityToolkit.Mvvm.Input;
using MeshCutter.App.ViewModels.Interfaces;
using MeshCutter.Core;
using System.IO;

namespace MeshCutter.App.ViewModels;

public partial class PieceViewModel : BaseViewModel
{
    [ObservableProperty]
    private string? _saveErrorMessage;

    [ObservableProperty]
    private string? _exportErrorMessage;

    private readonly IFileDialogService _fileDialogService;

    public PieceViewModel(DMesh3 mesh, double sizeX, double sizeY, double sizeZ, 
    double marginX, double marginY, double marginZ,
        IFileDialogService fileDialogService, IPieceWindowService pieceWindowService, PieceHistory currentPieceHistory, History sharedHistory)
        : base(pieceWindowService)
    {
        PrintSizeX = sizeX;
        PrintSizeY = sizeY;
        PrintSizeZ = sizeZ;
        MarginX = marginX;
        MarginY = marginY;
        MarginZ = marginZ;
        _fileDialogService = fileDialogService;
        LoadMesh(mesh);
        CurrentPieceHistory = currentPieceHistory;
        SharedHistory = sharedHistory;
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


    [RelayCommand]
    public void ExportHistory()
    {
        if (SharedHistory != null)
        {
            string exportName = "New Export.CSV";
            var fileName = _fileDialogService.GetExportPath(exportName);
            if (fileName != null)
            {
                try
                {
                    // gestion ecriture CSV
                    var content = CsvHistoryFormatter.Format(SharedHistory.Pieces);
                    File.WriteAllText(fileName, content);
                    ExportErrorMessage = null;
                }
                catch (IOException ex)
                {
                    ExportErrorMessage = $"Impossible d'écrire le fichier CSV : {ex.Message}";
                }
                catch (UnauthorizedAccessException ex)
                {
                    ExportErrorMessage = $"Accès refusé : {ex.Message}";
                }
            }
        }
    }
}

