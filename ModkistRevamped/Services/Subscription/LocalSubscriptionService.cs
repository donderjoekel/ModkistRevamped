using Modio.Models;

namespace TNRD.Modkist.Services.Subscription;

public class LocalSubscriptionService : ISubscriptionService
{
    public LocalSubscriptionService()
    {
    }

    public bool CanSubscribe => true;

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public bool IsSubscribed(Mod mod)
    {
        return IsSubscribed(mod.Id);
    }

    public bool IsSubscribed(uint modId)
    {
        return false;
    }

    public Task Subscribe(Mod mod)
    {
        return Subscribe(mod.Id);
    }

    public Task Subscribe(uint modId)
    {
        return Task.CompletedTask;
    }

    public Task Unsubscribe(Mod mod)
    {
        return Unsubscribe(mod.Id);
    }

    public Task Unsubscribe(uint modId)
    {
        return Task.CompletedTask;
    }
}
