using Modio;
using Modio.Models;

namespace TNRD.Modkist.Services;

public class SubscriptionService
{
    private readonly SettingsService settingsService;
    private readonly UserClient userClient;
    private readonly ModsClient modsClient;

    private readonly List<Mod> subscriptions = new();

    public SubscriptionService(UserClient userClient, SettingsService settingsService, ModsClient modsClient)
    {
        this.userClient = userClient;
        this.settingsService = settingsService;
        this.modsClient = modsClient;
    }

    public async Task LoadAuthenticatedUserData()
    {
        if (!settingsService.HasValidAccessToken())
            return;

        subscriptions.Clear();
        subscriptions.AddRange(await userClient.GetSubscriptions().ToList());
    }

    public bool IsSubscribedToMod(Mod mod)
    {
        return IsSubscribedToMod(mod.Id);
    }

    public bool IsSubscribedToMod(uint modId)
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
        await LoadAuthenticatedUserData();
    }

    public Task Unsubscribe(Mod mod)
    {
        return Unsubscribe(mod.Id);
    }

    public async Task Unsubscribe(uint modId)
    {
        await modsClient[modId].Unsubscribe();
        await LoadAuthenticatedUserData();
    }
}
