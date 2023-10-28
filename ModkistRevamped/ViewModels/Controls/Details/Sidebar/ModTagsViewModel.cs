using Modio.Models;
using TNRD.Modkist.Services;

namespace TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

public class ModTagsViewModel : ObservableObject
{
    private readonly SelectedModService selectedModService;

    public ModTagsViewModel(SelectedModService selectedModService)
    {
        this.selectedModService = selectedModService;
    }

    public List<Tag> Tags => selectedModService.SelectedMod!.Tags;
}
