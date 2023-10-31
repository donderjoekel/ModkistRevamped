using System.IO;
using Microsoft.Extensions.Logging;
using Modio;
using Modio.Models;
using Newtonsoft.Json;
using TNRD.Modkist.Models;
using Wpf.Ui.Controls;
using File = System.IO.File;

namespace TNRD.Modkist.Services.Subscription;

public class LocalSubscriptionService : ISubscriptionService
{
    private readonly ModsClient modsClient;
    private readonly ProfileService profileService;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly ILogger<LocalSubscriptionService> logger;
    private readonly ModCachingService modCachingService;

    private readonly List<Mod> subscriptions = new();

    public LocalSubscriptionService(
        ModsClient modsClient,
        ProfileService profileService,
        SnackbarQueueService snackbarQueueService,
        ILogger<LocalSubscriptionService> logger,
        ModCachingService modCachingService
    )
    {
        this.modsClient = modsClient;
        this.profileService = profileService;
        this.snackbarQueueService = snackbarQueueService;
        this.logger = logger;
        this.modCachingService = modCachingService;
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

        try
        {
            subscriptions.AddRange(localSubscriptions.SubscribedModIds.Select(x => modCachingService[x]));

            if (notify)
                SubscriptionsLoaded?.Invoke();
        }
        catch (RateLimitExceededException)
        {
            snackbarQueueService.EnqueueRateLimitMessage();
            logger.LogWarning("Being rate limited!");
        }
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

    public async Task<bool> Subscribe(Mod mod)
    {
        try
        {
            LocalSubscriptionsModel localSubscriptions = GetLocalSubscriptions();
            if (localSubscriptions.SubscribedModIds.Contains(mod.Id))
                return true;

            localSubscriptions.SubscribedModIds.Add(mod.Id);

            SaveLocalSubscriptions(localSubscriptions);
            await Initialize(false);

            SubscriptionAdded?.Invoke(mod);

            IReadOnlyList<Dependency> dependencies = await modsClient[mod.Id].Dependencies.Get();
            foreach (Dependency dependency in dependencies)
            {
                await Subscribe(modCachingService[dependency.ModId]);
            }

            return true;
        }
        catch (RateLimitExceededException)
        {
            snackbarQueueService.EnqueueRateLimitMessage();
            logger.LogWarning("Being rate limited!");
            return false;
        }
    }

    public async Task<bool> Unsubscribe(Mod mod)
    {
        LocalSubscriptionsModel localSubscriptions = GetLocalSubscriptions();

        if (localSubscriptions.SubscribedModIds.Remove(mod.Id))
        {
            SaveLocalSubscriptions(localSubscriptions);
            await Initialize(false);
            SubscriptionRemoved?.Invoke(mod);
        }

        return true;
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
