using Modio.Models;

namespace TNRD.Modkist.Services.Subscription;

public delegate void SubscriptionsLoadedDelegate();

public delegate void SubscriptionAddedDelegate(Mod mod);

public delegate void SubscriptionRemovedDelegate(Mod mod);

public interface ISubscriptionService
{
    bool CanSubscribe { get; }

    event SubscriptionsLoadedDelegate? SubscriptionsLoaded;
    event SubscriptionAddedDelegate? SubscriptionAdded;
    event SubscriptionRemovedDelegate? SubscriptionRemoved;

    Task Initialize();

    bool IsSubscribed(Mod mod);
    bool IsSubscribed(uint modId);
    Task<bool> Subscribe(Mod mod);
    Task<bool> Unsubscribe(Mod mod);

    IEnumerable<Mod> GetSubscribedMods();
}
