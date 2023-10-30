using Modio;
using Modio.Models;

namespace TNRD.Modkist.Services.Subscription;

public class RemoteSubscriptionService : ISubscriptionService
{
    private readonly SettingsService settingsService;
    private readonly UserClient userClient;
    private readonly ModsClient modsClient;

    private readonly List<Mod> subscriptions = new();

    public RemoteSubscriptionService(UserClient userClient, SettingsService settingsService, ModsClient modsClient)
    {
        this.userClient = userClient;
        this.settingsService = settingsService;
        this.modsClient = modsClient;
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
        if (!settingsService.HasValidAccessToken())
            throw new InvalidOperationException("Cannot initialize subscription service without a valid access token!");

        subscriptions.Clear();
        subscriptions.AddRange(await userClient.GetSubscriptions().ToList());

        if (notify) SubscriptionsLoaded?.Invoke();
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
    }

    public Task Unsubscribe(Mod mod)
    {
        return Unsubscribe(mod.Id);
    }

    public async Task Unsubscribe(uint modId)
    {
        await modsClient[modId].Unsubscribe();
        await Initialize(false);
        SubscriptionRemoved?.Invoke(modId);
    }

    public IEnumerable<Mod> GetSubscribedMods()
    {
        return subscriptions;
    }
}
