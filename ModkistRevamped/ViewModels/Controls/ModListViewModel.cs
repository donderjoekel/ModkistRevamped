using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Modio.Models;
using TNRD.Modkist.Extensions;
using TNRD.Modkist.Factories.Controls;
using TNRD.Modkist.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Services.Subscription;
using TNRD.Modkist.Views.Controls;
using TNRD.Modkist.Views.Controls.List;
using Visibility = System.Windows.Visibility;

namespace TNRD.Modkist.ViewModels.Controls;

public partial class ModListViewModel : ObservableObject
{
    private readonly ModListCardItemFactory modListCardItemFactory;
    private readonly ModListListItemFactory modListListItemFactory;
    private readonly ModCachingService modCachingService;
    private readonly ISubscriptionService subscriptionService;

    public ModListViewModel(
        ModListCardItemFactory modListCardItemFactory,
        ModCachingService modCachingService,
        ISubscriptionService subscriptionService,
        ModListListItemFactory modListListItemFactory
    )
    {
        this.modListCardItemFactory = modListCardItemFactory;
        this.modCachingService = modCachingService;
        this.subscriptionService = subscriptionService;
        this.modListListItemFactory = modListListItemFactory;

        this.subscriptionService.SubscriptionAdded += OnSubscriptionAdded;
        this.subscriptionService.SubscriptionRemoved += OnSubscriptionRemoved;

        collectionView = CollectionViewSource.GetDefaultView(collection);
        collectionView.Filter = FilterItems;

        OnSortModeChanged(SortMode.MostPopular);
    }

    [ObservableProperty] private ModType modType;
    [ObservableProperty] private string? query;
    [ObservableProperty] private SortMode sortMode = SortMode.MostPopular;
    [ObservableProperty] private ObservableCollection<FrameworkElement> collection = new();
    [ObservableProperty] private ICollectionView collectionView;
    [ObservableProperty] private bool installedOnly;
    [ObservableProperty] private ModListMode listMode;
    [ObservableProperty] private Visibility cardViewVisibility;
    [ObservableProperty] private Visibility listViewVisibility;

    private void OnSubscriptionAdded(Mod mod)
    {
        if (!InstalledOnly)
            return;

        ReloadMods();
    }

    private void OnSubscriptionRemoved(Mod mod)
    {
        if (!InstalledOnly)
            return;

        ReloadMods();
    }

    partial void OnModTypeChanged(ModType value)
    {
        ReloadMods();
    }

    partial void OnInstalledOnlyChanged(bool value)
    {
        ReloadMods();
    }

    partial void OnListModeChanged(ModListMode value)
    {
        CardViewVisibility = value == ModListMode.Card ? Visibility.Visible : Visibility.Collapsed;
        ListViewVisibility = value == ModListMode.List ? Visibility.Visible : Visibility.Collapsed;

        ReloadMods();
    }

    private void ReloadMods()
    {
        if (ListMode == ModListMode.None)
            return;

        List<Mod> mods = GetOrderedMods();

        Collection.Clear();

        for (int i = 0; i < mods.Count; i++)
        {
            Mod mod = mods[i];

            if (ListMode == ModListMode.Card)
            {
                Collection.Add(modListCardItemFactory.Create(mod));
            }
            else if (ListMode == ModListMode.List)
            {
                Collection.Add(modListListItemFactory.Create(mod));
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    partial void OnQueryChanged(string? value)
    {
        CollectionView.Refresh();
    }

    partial void OnSortModeChanged(SortMode value)
    {
        switch (value)
        {
            case SortMode.Alphabetical:
                SetSort(nameof(ModListCardItem.SortTitle), ListSortDirection.Ascending);
                break;
            case SortMode.MostPopular:
                SetSort(nameof(ModListCardItem.SortPopularityRank), ListSortDirection.Ascending);
                break;
            case SortMode.MostDownloads:
                SetSort(nameof(ModListCardItem.SortTotalDownloads), ListSortDirection.Ascending);
                break;
            case SortMode.MostSubscribers:
                SetSort(nameof(ModListCardItem.SortTotalSubscribers), ListSortDirection.Ascending);
                break;
            case SortMode.RecentlyAdded:
                SetSort(nameof(ModListCardItem.SortDateAdded), ListSortDirection.Descending);
                break;
            case SortMode.LastUpdated:
                SetSort(nameof(ModListCardItem.SortDateUpdated), ListSortDirection.Descending);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
    }

    private void SetSort(string key, ListSortDirection direction)
    {
        CollectionView.SortDescriptions.Clear();
        CollectionView.SortDescriptions.Add(new SortDescription(key, direction));
        CollectionView.Refresh();
    }

    private List<Mod> GetOrderedMods()
    {
        string tag = ModType switch
        {
            ModType.None => string.Empty,
            ModType.Plugin => "plugin",
            ModType.Blueprint => "blueprint",
            _ => throw new ArgumentOutOfRangeException()
        };

        List<Mod> mods = modCachingService
            .Where(x => x.MatchesTag(tag))
            .Where(x => !InstalledOnly || subscriptionService.IsSubscribed(x))
            .ToList();

        return mods.OrderByDescending(x => Math.Max(x.DateUpdated, x.DateAdded)).ToList();
    }

    private bool FilterItems(object obj)
    {
        if (obj is ModListCardItem cardItem)
            return string.IsNullOrEmpty(Query) || cardItem.IsValidForFilter(Query);
        if (obj is ModListListItem listItem)
            return string.IsNullOrEmpty(Query) || listItem.IsValidForFilter(Query);

        return false;
    }

    public void OnNavigatedTo()
    {
        ReloadMods();
    }
}
