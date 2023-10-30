using Modio;
using Modio.Models;

namespace TNRD.Modkist.Services.Rating;

public class RemoteRatingService : IRatingService
{
    private readonly SettingsService settingsService;
    private readonly UserClient userClient;
    private readonly ModsClient modsClient;

    private readonly List<Modio.Models.Rating> ratings = new();

    public RemoteRatingService(SettingsService settingsService, UserClient userClient, ModsClient modsClient)
    {
        this.settingsService = settingsService;
        this.userClient = userClient;
        this.modsClient = modsClient;
    }

    public bool CanRate => true;

    public async Task Initialize()
    {
        if (!settingsService.HasValidAccessToken())
            return;

        ratings.Clear();
        ratings.AddRange(await userClient.GetRatings().ToList());
    }

    public bool HasUpvoted(Mod mod)
    {
        return HasUpvoted(mod.Id);
    }

    public bool HasUpvoted(uint modId)
    {
        return ratings.Any(x => x.ModId == modId && x.Value == 1);
    }

    public bool HasDownvoted(Mod mod)
    {
        return HasDownvoted(mod.Id);
    }

    public bool HasDownvoted(uint modId)
    {
        return ratings.Any(x => x.ModId == modId && x.Value == -1);
    }

    public Task Upvote(Mod mod)
    {
        return Upvote(mod.Id);
    }

    public async Task Upvote(uint modId)
    {
        await modsClient[modId].Rate(NewRating.Positive);
        await Initialize();
    }

    public Task Downvote(Mod mod)
    {
        return Downvote(mod.Id);
    }

    public async Task Downvote(uint modId)
    {
        await modsClient[modId].Rate(NewRating.Negative);
        await Initialize();
    }

    public Task RemoveRating(Mod mod)
    {
        return RemoveRating(mod.Id);
    }

    public async Task RemoveRating(uint modId)
    {
        await modsClient[modId].Rate(NewRating.None);
        await Initialize();
    }
}
