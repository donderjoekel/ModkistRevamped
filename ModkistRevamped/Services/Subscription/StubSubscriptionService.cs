using Modio.Models;

namespace TNRD.Modkist.Services.Subscription;

public class StubSubscriptionService : ISubscriptionService
{
    public bool CanSubscribe => false;
    public event SubscriptionsLoadedDelegate? SubscriptionsLoaded;
    public event SubscriptionAddedDelegate? SubscriptionAdded;
    public event SubscriptionRemovedDelegate? SubscriptionRemoved;

    public Task Initialize()
    {
        // Invoke only this event here so that other services can respond to it as it is the event
        // that makes sure the initial mod install/update check is done
        SubscriptionsLoaded?.Invoke();
        return Task.CompletedTask;
    }

    public bool IsSubscribed(Mod mod)
    {
        return false;
    }

    public bool IsSubscribed(uint modId)
    {
        return false;
    }

    public Task Subscribe(Mod mod)
    {
        return Task.CompletedTask;
    }

    public Task Subscribe(uint modId)
    {
        return Task.CompletedTask;
    }

    public Task Unsubscribe(Mod mod)
    {
        return Task.CompletedTask;
    }

    public Task Unsubscribe(uint modId)
    {
        return Task.CompletedTask;
    }

    public IEnumerable<Mod> GetSubscribedMods()
    {
        return Array.Empty<Mod>();
    }
}
