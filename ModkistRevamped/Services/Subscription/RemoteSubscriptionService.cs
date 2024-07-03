using Microsoft.Extensions.Logging;
using Modio;
using Modio.Filters;
using Modio.Models;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Services.Subscription;

public class RemoteSubscriptionService : ISubscriptionService
{
    private readonly SettingsService settingsService;
    private readonly UserClient userClient;
    private readonly ModsClient modsClient;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly ILogger<RemoteSubscriptionService> logger;
    private readonly ModCachingService modCachingService;

    private readonly List<Mod> subscriptions = new();

    public RemoteSubscriptionService(
        UserClient userClient,
        SettingsService settingsService,
        ModsClient modsClient,
        SnackbarQueueService snackbarQueueService,
        ILogger<RemoteSubscriptionService> logger,
        ModCachingService modCachingService
    )
    {
        this.userClient = userClient;
        this.settingsService = settingsService;
        this.modsClient = modsClient;
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
        try
        {
            if (!settingsService.HasValidAccessToken())
                throw new InvalidOperationException(
                    "Cannot initialize subscription service without a valid access token!");

            subscriptions.Clear();
            subscriptions.AddRange(await userClient
                .GetSubscriptions(Filter.Custom("game_id", Operator.Equal, "3213"))
                .ToList());

            if (notify) SubscriptionsLoaded?.Invoke();
        }
        catch (RateLimitExceededException)
        {
            snackbarQueueService.EnqueueRateLimitMessage();
            logger.LogWarning("Being rate limited!");
        }
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
            await modsClient[mod.Id].Subscribe();
            await Initialize(false);
            SubscriptionAdded?.Invoke(mod);

            IReadOnlyList<Dependency> dependencies = await modsClient[mod.Id].Dependencies.Get();
            foreach (Dependency dependency in dependencies)
            {
                if (subscriptions.Any(x => x.Id == dependency.ModId))
                    continue;

                if (modCachingService.TryGetMod(dependency.ModId, out Mod? dependencyMod))
                {
                    await Subscribe(dependencyMod);
                }
                else
                {
                    logger.LogError("Mod with id '{Id}' not found in cache!", dependency.ModId);
                    snackbarQueueService.Enqueue(
                        "Uh oh",
                        $"Dependency with id '{dependency.ModId}' cannot be found!",
                        ControlAppearance.Danger,
                        new SymbolIcon(SymbolRegular.ErrorCircle24));
                }
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
        try
        {
            await modsClient[mod.Id].Unsubscribe();
            await Initialize(false);
            SubscriptionRemoved?.Invoke(mod);
            return true;
        }
        catch (RateLimitExceededException)
        {
            snackbarQueueService.EnqueueRateLimitMessage();
            logger.LogWarning("Being rate limited!");
            return false;
        }
    }

    public IEnumerable<Mod> GetSubscribedMods()
    {
        return subscriptions;
    }
}
