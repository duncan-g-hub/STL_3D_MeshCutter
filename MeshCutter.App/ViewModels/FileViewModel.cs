using CommunityToolkit.Mvvm.ComponentModel;
using g3;
using CommunityToolkit.Mvvm.Input;
using MeshCutter.App.ViewModels.Interfaces;
using MeshCutter.Core;

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
        var (result, mesh) = MeshLoader.GetMeshFromSTLFile(path);
        if (result.code == IOCode.Ok && mesh != null)
        {
            LoadMesh(mesh);
            FileErrorMessage = null;
        }
        else
        {
            FileErrorMessage = $"Erreur lors du chargement du fichier : \ncode : {result.code} \nmessage : {result.message}";
        }
    }

}


