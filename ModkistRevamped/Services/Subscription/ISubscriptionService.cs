using Modio.Models;

namespace TNRD.Modkist.Services.Subscription;

public interface ISubscriptionService
{
    bool CanSubscribe { get; }

    Task Initialize();
    bool IsSubscribed(Mod mod);
    bool IsSubscribed(uint modId);
    Task Subscribe(Mod mod);
    Task Subscribe(uint modId);
    Task Unsubscribe(Mod mod);
    Task Unsubscribe(uint modId);
}
