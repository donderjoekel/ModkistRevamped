using Modio.Models;

namespace TNRD.Modkist.Services;

public class SelectedModService
{
    private readonly List<Mod> selectedMods = new();

    public Mod? SelectedMod => selectedMods.LastOrDefault();

    public void SetSelectedMod(Mod? selectedMod, bool addOnTop)
    {
        if (selectedMod == null && !addOnTop)
        {
            selectedMods.Clear();
            return;
        }

        if (!addOnTop)
            selectedMods.Clear();

        selectedMods.Add(selectedMod!);
    }
}
