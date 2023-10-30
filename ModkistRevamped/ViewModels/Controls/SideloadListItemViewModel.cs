using TNRD.Modkist.Models;
using TNRD.Modkist.Services;

namespace TNRD.Modkist.ViewModels.Controls;

public partial class SideloadListItemViewModel : ObservableObject
{
    private readonly SideloadedModModel sideloadedMod;
    private readonly SideloadService sideloadService;

    public SideloadListItemViewModel(SideloadedModModel sideloadedMod, SideloadService sideloadService)
    {
        this.sideloadedMod = sideloadedMod;
        this.sideloadService = sideloadService;

        Name = this.sideloadedMod.Filename;
    }

    [ObservableProperty] private string name;

    [RelayCommand]
    private void Delete()
    {
        sideloadService.Remove(sideloadedMod);
    }
}
