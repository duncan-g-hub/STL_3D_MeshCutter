using CommunityToolkit.Mvvm.ComponentModel;
using g3;
using CommunityToolkit.Mvvm.Input;
using MeshCutter.App.ViewModels.Interfaces;
using MeshCutter.Core;
using System.IO;
using System.Diagnostics;

namespace MeshCutter.App.ViewModels;

public partial class FileViewModel : BaseViewModel
{
    [ObservableProperty]
    private string? _fileErrorMessage;

    private readonly IFileDialogService _fileDialogService;

   

    public FileViewModel(IFileDialogService fileDialogService, IPieceWindowService pieceWindowService)
        : base(pieceWindowService)
    {
        _fileDialogService = fileDialogService;
        
    }
    
    [RelayCommand]
    public void OpenFile()
    {
        var path = _fileDialogService.GetFilePath();
        if (path != null)
        {
            LoadFile(path);
        }
    }

    public void LoadFile(string path)
    {
        
        var stopwatch = Stopwatch.StartNew();
        var (result, mesh) = MeshLoader.GetMeshFromSTLFile(path);

        stopwatch.Stop();
        
        if (result.code == IOCode.Ok && mesh != null)
        {
            SharedHistory = new History();
            CurrentPieceHistory = SharedHistory.AddRoot(Path.GetFileNameWithoutExtension(path), mesh);
            LoadMesh(mesh);
            FileErrorMessage = $"Load : {stopwatch.ElapsedMilliseconds} ms";
        }
        else
        {
            FileErrorMessage = $"Erreur lors du chargement du fichier : \ncode : {result.code} \nmessage : {result.message}";
        }
    }

}


