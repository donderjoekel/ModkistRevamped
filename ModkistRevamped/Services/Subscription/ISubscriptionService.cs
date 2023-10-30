using Modio.Models;

namespace TNRD.Modkist.Services.Subscription;

public delegate void SubscriptionsLoadedDelegate();

public delegate void SubscriptionAddedDelegate(uint modId);

public delegate void SubscriptionRemovedDelegate(uint modId);

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
    Task<bool> Subscribe(uint modId);
    Task<bool> Unsubscribe(Mod mod);
    Task<bool> Unsubscribe(uint modId);

    IEnumerable<Mod> GetSubscribedMods();
}
