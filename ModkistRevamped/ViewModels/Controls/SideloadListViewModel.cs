using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Forms;
using TNRD.Modkist.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Views.Controls;

namespace TNRD.Modkist.ViewModels.Controls;

public partial class SideloadListViewModel : ObservableObject
{
    private readonly SideloadService sideloadService;

    public SideloadListViewModel(SideloadService sideloadService)
    {
        this.sideloadService = sideloadService;
        this.sideloadService.ModSideloaded += OnModSideloaded;
        this.sideloadService.SideloadedModRemoved += OnSideloadedModRemoved;

        collectionView = CollectionViewSource.GetDefaultView(collection);

        LoadSideloadedMods();
    }

    [ObservableProperty] private ObservableCollection<FrameworkElement> collection = new();
    [ObservableProperty] private ICollectionView collectionView;

    private void LoadSideloadedMods()
    {
        Collection.Clear();

        List<SideloadedModModel> sideloadedMods = sideloadService.GetSideloadedMods();

        for (int i = 0; i < sideloadedMods.Count; i++)
        {
            SideloadedModModel sideloadedMod = sideloadedMods[i];

            if (i > 0)
            {
                Collection.Add(new Separator());
            }

            SideloadListItem sideloadListItem = new(sideloadedMod);
            Collection.Add(sideloadListItem);
        }
    }

    private void OnModSideloaded()
    {
        LoadSideloadedMods();
    }

    private void OnSideloadedModRemoved()
    {
        LoadSideloadedMods();
    }

    [RelayCommand]
    private void Sideload()
    {
        OpenFileDialog ofd = new()
        {
            Multiselect = false,
            Filter =
                "dll files (*.dll)|*.dll|zip files (*.zip)|*.zip|zeeplevel blueprint files (*.zeeplevel)|*.zeeplevel|All files (*.*)|*.*",
            Title = "Select mod to side-load"
        };

        DialogResult dialogResult = ofd.ShowDialog();

        if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
        {
            sideloadService.SideloadMod(ofd.FileName);
        }
    }
}
