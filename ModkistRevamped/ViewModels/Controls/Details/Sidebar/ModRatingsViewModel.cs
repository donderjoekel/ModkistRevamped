using TNRD.Modkist.Services;

namespace TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

public class ModRatingsViewModel : ObservableObject
{
    private readonly SelectedModService selectedModService;

    public ModRatingsViewModel(SelectedModService selectedModService)
    {
        this.selectedModService = selectedModService;
    }

    public uint PositiveRatings => selectedModService.SelectedMod?.Stats?.PositiveRatings ?? 0;
    public uint NegativeRatings => selectedModService.SelectedMod?.Stats?.NegativeRatings ?? 0;

    // TODO: Add commands for processing up and down vote
}
