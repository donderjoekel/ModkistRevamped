using ReverseMarkdown;
using TNRD.Modkist.Services;

namespace TNRD.Modkist.ViewModels.Controls.Details;

public class ModDescriptionViewModel : ObservableObject
{
    private readonly SelectedModService selectedModService;

    public ModDescriptionViewModel(SelectedModService selectedModService)
    {
        this.selectedModService = selectedModService;

        Markdown = selectedModService.SelectedMod!.Description ?? string.Empty;

        if (!string.IsNullOrEmpty(Markdown))
        {
            Converter converter = new();
            Markdown = converter.Convert(Markdown);
        }
    }

    public Visibility SectionVisibility => string.IsNullOrEmpty(selectedModService.SelectedMod!.Description)
        ? Visibility.Collapsed
        : Visibility.Visible;

    public string Markdown { get; }
}
