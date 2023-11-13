using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Threading;
using TNRD.Modkist.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Views.Controls;

namespace TNRD.Modkist.ViewModels.Controls;

public partial class SideloadListViewModel : ObservableObject
{
    private readonly SideloadService sideloadService;
    private readonly Dispatcher dispatcher;

    public SideloadListViewModel(SideloadService sideloadService)
    {
        this.sideloadService = sideloadService;
        this.sideloadService.ModSideloaded += OnModSideloaded;
        this.sideloadService.SideloadedModRemoved += OnSideloadedModRemoved;

        dispatcher = Dispatcher.CurrentDispatcher;

        new FileSystemWatcher();

        collectionView = CollectionViewSource.GetDefaultView(collection);

        LoadSideloadedMods();
    }

    private FileSystemWatcher? fileSystemWatcher;

    [ObservableProperty] private ObservableCollection<FrameworkElement> collection = new();
    [ObservableProperty] private ICollectionView collectionView;
    [ObservableProperty] private ModType modType;
    [ObservableProperty] private string title = null!;

    partial void OnModTypeChanged(ModType value)
    {
        switch (value)
        {
            case ModType.Plugin:
                Title = "Plugins";
                break;
            case ModType.Blueprint:
                Title = "Blueprints";
                break;
            case ModType.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }

        fileSystemWatcher?.Dispose();

        fileSystemWatcher = new FileSystemWatcher(sideloadService.GetFullPath(value));
        fileSystemWatcher.IncludeSubdirectories = false;
        fileSystemWatcher.Changed += (_, _) => LoadSideloadedMods();
        fileSystemWatcher.Created += (_, _) => LoadSideloadedMods();
        fileSystemWatcher.Deleted += (_, _) => LoadSideloadedMods();
        fileSystemWatcher.Renamed += (_, _) => LoadSideloadedMods();
        fileSystemWatcher.EnableRaisingEvents = true;

        LoadSideloadedMods();
    }

    private void LoadSideloadedMods()
    {
        if (ModType == ModType.None)
            return;

        dispatcher.Invoke(() =>
        {
            Collection.Clear();

            List<SideloadedModModel> sideloadedMods = ModType == ModType.Plugin
                ? sideloadService.GetSideloadedPlugins()
                : sideloadService.GetSideloadedBlueprints();

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
        });
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
            Filter = GetFilter(),
            Title = "Select mod to side-load"
        };

        DialogResult dialogResult = ofd.ShowDialog();

        if (dialogResult != DialogResult.OK || string.IsNullOrWhiteSpace(ofd.FileName))
            return;

        switch (ModType)
        {
            case ModType.Plugin:
                sideloadService.SideloadPlugin(ofd.FileName);
                break;
            case ModType.Blueprint:
                sideloadService.SideloadBlueprint(ofd.FileName);
                break;
            case ModType.None:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetFilter()
    {
        return ModType switch
        {
            ModType.Plugin => "dll files (*.dll)|*.dll|zip files (*.zip)|*.zip|All files (*.*)|*.*",
            ModType.Blueprint =>
                "zeeplevel files (*.zeeplevel)|*.zeeplevel|zip files (*.zip)|*.zip|All files (*.*)|*.*",
            ModType.None => throw new ArgumentOutOfRangeException(nameof(ModType), ModType, null),
            _ => throw new InvalidOperationException()
        };
    }
}
