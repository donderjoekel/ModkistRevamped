using Humanizer;
using TNRD.Modkist.Services;

namespace TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

public class ModStatisticsViewModel : ObservableObject
{
    private readonly SelectedModService selectedModService;

    public ModStatisticsViewModel(SelectedModService selectedModService)
    {
        this.selectedModService = selectedModService;
    }

    public string VersionNumber => selectedModService.SelectedMod!.Modfile?.Version ?? "Unknown";

    public string LastUpdated =>
        DateTimeOffset.FromUnixTimeSeconds(selectedModService.SelectedMod!.DateUpdated).Humanize();

    public string TotalDownloads => selectedModService.SelectedMod!.Stats?.TotalDownloads.ToString() ?? string.Empty;

    public string TotalSubscribers =>
        selectedModService.SelectedMod!.Stats?.TotalSubscribers.ToString() ?? string.Empty;

    public string ResourceId => selectedModService.SelectedMod!.Id.ToString();
}
