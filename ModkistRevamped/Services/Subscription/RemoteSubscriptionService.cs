using Microsoft.Extensions.Logging;
using Modio;
using Modio.Models;

namespace TNRD.Modkist.Services.Subscription;

public class RemoteSubscriptionService : ISubscriptionService
{
    private readonly SettingsService settingsService;
    private readonly UserClient userClient;
    private readonly ModsClient modsClient;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly ILogger<RemoteSubscriptionService> logger;

    private readonly List<Mod> subscriptions = new();

    public RemoteSubscriptionService(
        UserClient userClient,
        SettingsService settingsService,
        ModsClient modsClient,
        SnackbarQueueService snackbarQueueService,
        ILogger<RemoteSubscriptionService> logger
    )
    {
        this.userClient = userClient;
        this.settingsService = settingsService;
        this.modsClient = modsClient;
        this.snackbarQueueService = snackbarQueueService;
        this.logger = logger;
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
            subscriptions.AddRange(await userClient.GetSubscriptions().ToList());

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

    public Task<bool> Subscribe(Mod mod)
    {
        return Subscribe(mod.Id);
    }

    public async Task<bool> Subscribe(uint modId)
    {
        try
        {
            await modsClient[modId].Subscribe();
            await Initialize(false);
            SubscriptionAdded?.Invoke(modId);

            IReadOnlyList<Dependency> dependencies = await modsClient[modId].Dependencies.Get();
            foreach (Dependency dependency in dependencies)
            {
                if (subscriptions.Any(x => x.Id == dependency.ModId))
                    continue;

                await Subscribe(dependency.ModId);
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

    public Task<bool> Unsubscribe(Mod mod)
    {
        return Unsubscribe(mod.Id);
    }

    public async Task<bool> Unsubscribe(uint modId)
    {
        try
        {
            await modsClient[modId].Unsubscribe();
            await Initialize(false);
            SubscriptionRemoved?.Invoke(modId);
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
