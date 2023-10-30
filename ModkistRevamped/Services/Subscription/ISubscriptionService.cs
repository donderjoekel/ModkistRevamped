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
    Task Subscribe(Mod mod);
    Task Subscribe(uint modId);
    Task Unsubscribe(Mod mod);
    Task Unsubscribe(uint modId);

    IEnumerable<Mod> GetSubscribedMods();
}
