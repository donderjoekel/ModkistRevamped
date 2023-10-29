using Modio;
using Modio.Models;

namespace TNRD.Modkist.Services;

public class RatingService
{
    private readonly SettingsService settingsService;
    private readonly UserClient userClient;
    private readonly ModsClient modsClient;

    private readonly List<Rating> ratings = new();

    public RatingService(SettingsService settingsService, UserClient userClient, ModsClient modsClient)
    {
        this.settingsService = settingsService;
        this.userClient = userClient;
        this.modsClient = modsClient;
    }

    public async Task LoadAuthenticatedUserData()
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
        await LoadAuthenticatedUserData();
    }

    public Task Downvote(Mod mod)
    {
        return Downvote(mod.Id);
    }

    public async Task Downvote(uint modId)
    {
        await modsClient[modId].Rate(NewRating.Negative);
        await LoadAuthenticatedUserData();
    }

    public Task RemoveRating(Mod mod)
    {
        return RemoveRating(mod.Id);
    }

    public async Task RemoveRating(uint modId)
    {
        await modsClient[modId].Rate(NewRating.None);
        await LoadAuthenticatedUserData();
    }
}
