using TNRD.Modkist.Services;

namespace TNRD.Modkist.ViewModels.Pages;

public class ModDetailsViewModel : ObservableObject
{
    private readonly SelectedModService selectedModService;

    public ModDetailsViewModel(SelectedModService selectedModService)
    {
        this.selectedModService = selectedModService;
    }

    public string Name => selectedModService.SelectedMod?.Name ?? "[UNKNOWN]";
}
