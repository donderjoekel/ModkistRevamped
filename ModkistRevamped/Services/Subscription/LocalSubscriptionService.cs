using System.IO;
using Modio;
using Modio.Models;
using Newtonsoft.Json;
using TNRD.Modkist.Models;
using File = System.IO.File;

namespace TNRD.Modkist.Services.Subscription;

public class LocalSubscriptionService : ISubscriptionService
{
    private readonly ModsClient modsClient;
    private readonly ProfileService profileService;

    private readonly List<Mod> subscriptions = new();

    public LocalSubscriptionService(ModsClient modsClient, ProfileService profileService)
    {
        this.modsClient = modsClient;
        this.profileService = profileService;
    }

    public bool CanSubscribe => true;
    public event SubscriptionsLoadedDelegate? SubscriptionsLoaded;
    public event SubscriptionAddedDelegate? SubscriptionAdded;
    public event SubscriptionRemovedDelegate? SubscriptionRemoved;

    Task ISubscriptionService.Initialize()
    {
        return Initialize(true);
    }

    private async Task Initialize(bool notify)
    {
        subscriptions.Clear();

        string modsPath = Path.Combine(GetModsDirectory(), profileService.SelectedProfileId + ".mods");

        if (!File.Exists(modsPath))
        {
            string json = JsonConvert.SerializeObject(new LocalSubscriptionsModel());
            await File.WriteAllTextAsync(modsPath, json);
        }

        string jsonContent = await File.ReadAllTextAsync(modsPath);
        LocalSubscriptionsModel? localSubscriptions =
            JsonConvert.DeserializeObject<LocalSubscriptionsModel>(jsonContent);

        if (localSubscriptions == null || localSubscriptions.SubscribedModIds.Count == 0)
        {
            if (notify)
                SubscriptionsLoaded?.Invoke();
            return;
        }

        IReadOnlyList<Mod> mods = await modsClient.Search().ToList();
        subscriptions.AddRange(mods.Where(x => localSubscriptions.SubscribedModIds.Contains(x.Id)));

        if (notify)
            SubscriptionsLoaded?.Invoke();
    }

    private static string GetModsDirectory()
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Mods");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    public bool IsSubscribed(Mod mod)
    {
        return IsSubscribed(mod.Id);
    }

    public bool IsSubscribed(uint modId)
    {
        return subscriptions.Any(x => x.Id == modId);
    }

    public Task Subscribe(Mod mod)
    {
        return Subscribe(mod.Id);
    }

    public async Task Subscribe(uint modId)
    {
        LocalSubscriptionsModel localSubscriptions = GetLocalSubscriptions();
        if (localSubscriptions.SubscribedModIds.Contains(modId))
            return;

        localSubscriptions.SubscribedModIds.Add(modId);
        SaveLocalSubscriptions(localSubscriptions);
        await Initialize(false);
        SubscriptionAdded?.Invoke(modId);

        IReadOnlyList<Dependency> dependencies = await modsClient[modId].Dependencies.Get();
        foreach (Dependency dependency in dependencies)
        {
            await Subscribe(dependency.ModId);
        }
    }

    public Task Unsubscribe(Mod mod)
    {
        return Unsubscribe(mod.Id);
    }

    public async Task Unsubscribe(uint modId)
    {
        LocalSubscriptionsModel localSubscriptions = GetLocalSubscriptions();
        if (localSubscriptions.SubscribedModIds.Remove(modId))
        {
            SaveLocalSubscriptions(localSubscriptions);
            await Initialize(false);
            SubscriptionRemoved?.Invoke(modId);
        }
    }

    public IEnumerable<Mod> GetSubscribedMods()
    {
        return subscriptions;
    }

    private LocalSubscriptionsModel GetLocalSubscriptions()
    {
        string modsPath = Path.Combine(GetModsDirectory(), profileService.SelectedProfileId + ".mods");
        string jsonContent = File.ReadAllText(modsPath);
        return JsonConvert.DeserializeObject<LocalSubscriptionsModel>(jsonContent)!;
    }

    private void SaveLocalSubscriptions(LocalSubscriptionsModel model)
    {
        string modsPath = Path.Combine(GetModsDirectory(), profileService.SelectedProfileId + ".mods");
        string json = JsonConvert.SerializeObject(model);
        File.WriteAllText(modsPath, json);
    }
}
