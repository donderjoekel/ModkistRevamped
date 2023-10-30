using Modio.Models;

namespace TNRD.Modkist.Services.Subscription;

public class StubSubscriptionService : ISubscriptionService
{
    public bool CanSubscribe => false;

    public Task Initialize()
    {
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
}
