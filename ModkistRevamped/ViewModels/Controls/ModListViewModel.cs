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

namespace TNRD.Modkist.ViewModels.Controls;

public partial class ModListViewModel : ObservableObject
{
    private readonly ModCardFactory modCardFactory;
    private readonly ModCachingService modCachingService;
    private readonly ISubscriptionService subscriptionService;

    public ModListViewModel(
        ModCardFactory modCardFactory,
        ModCachingService modCachingService,
        ISubscriptionService subscriptionService
    )
    {
        this.modCardFactory = modCardFactory;
        this.modCachingService = modCachingService;
        this.subscriptionService = subscriptionService;

        this.subscriptionService.SubscriptionAdded += OnSubscriptionAdded;
        this.subscriptionService.SubscriptionRemoved += OnSubscriptionRemoved;

        collectionView = CollectionViewSource.GetDefaultView(ModCards);
        collectionView.Filter = FilterModCards;
        OnSortModeChanged(SortMode.MostPopular);
    }

    [ObservableProperty] private ModType modType;
    [ObservableProperty] private string? query;
    [ObservableProperty] private SortMode sortMode = SortMode.MostPopular;
    [ObservableProperty] private ObservableCollection<ModCard> modCards = new();
    [ObservableProperty] private ICollectionView collectionView;
    [ObservableProperty] private bool installedOnly;

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

    private void ReloadMods()
    {
        List<Mod> mods = GetOrderedMods();

        ModCards.Clear();

        foreach (Mod mod in mods)
        {
            ModCard modCard = modCardFactory.Create(mod);
            ModCards.Add(modCard);
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
                SetSort(nameof(ModCard.SortTitle), ListSortDirection.Ascending);
                break;
            case SortMode.MostPopular:
                SetSort(nameof(ModCard.SortPopularityRank), ListSortDirection.Ascending);
                break;
            case SortMode.MostDownloads:
                SetSort(nameof(ModCard.SortTotalDownloads), ListSortDirection.Ascending);
                break;
            case SortMode.MostSubscribers:
                SetSort(nameof(ModCard.SortTotalSubscribers), ListSortDirection.Ascending);
                break;
            case SortMode.RecentlyAdded:
                SetSort(nameof(ModCard.SortDateAdded), ListSortDirection.Descending);
                break;
            case SortMode.LastUpdated:
                SetSort(nameof(ModCard.SortDateUpdated), ListSortDirection.Descending);
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

    private bool FilterModCards(object obj)
    {
        if (obj is not ModCard modCard)
            return false;

        return string.IsNullOrEmpty(Query) || modCard.IsValidForFilter(Query);
    }

    public void OnNavigatedTo()
    {
        ReloadMods();
    }
}
