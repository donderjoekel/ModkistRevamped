using Microsoft.Extensions.Logging;
using Modio;
using Modio.Models;

namespace TNRD.Modkist.Services.Rating;

public class RemoteRatingService : IRatingService
{
    private readonly SettingsService settingsService;
    private readonly UserClient userClient;
    private readonly ModsClient modsClient;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly ILogger<RemoteRatingService> logger;

    private readonly List<Modio.Models.Rating> ratings = new();

    public RemoteRatingService(
        SettingsService settingsService,
        UserClient userClient,
        ModsClient modsClient,
        SnackbarQueueService snackbarQueueService,
        ILogger<RemoteRatingService> logger
    )
    {
        this.settingsService = settingsService;
        this.userClient = userClient;
        this.modsClient = modsClient;
        this.snackbarQueueService = snackbarQueueService;
        this.logger = logger;
    }

    public bool CanRate => true;

    public async Task Initialize()
    {
        try
        {
            if (!settingsService.HasValidAccessToken())
                return;

            ratings.Clear();
            ratings.AddRange(await userClient.GetRatings().ToList());
        }
        catch (RateLimitExceededException)
        {
            snackbarQueueService.EnqueueRateLimitMessage();
            logger.LogWarning("Being rate limited!");
        }
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

    public Task<bool> Upvote(Mod mod)
    {
        return Upvote(mod.Id);
    }

    public async Task<bool> Upvote(uint modId)
    {
        try
        {
            await modsClient[modId].Rate(NewRating.Positive);
            await Initialize();
            return true;
        }
        catch (RateLimitExceededException)
        {
            snackbarQueueService.EnqueueRateLimitMessage();
            logger.LogWarning("Being rate limited!");
            return false;
        }
    }

    public Task<bool> Downvote(Mod mod)
    {
        return Downvote(mod.Id);
    }

    public async Task<bool> Downvote(uint modId)
    {
        try
        {
            await modsClient[modId].Rate(NewRating.Negative);
            await Initialize();
            return true;
        }
        catch (RateLimitExceededException)
        {
            snackbarQueueService.EnqueueRateLimitMessage();
            logger.LogWarning("Being rate limited!");
            return false;
        }
    }

    public Task<bool> RemoveRating(Mod mod)
    {
        return RemoveRating(mod.Id);
    }

    public async Task<bool> RemoveRating(uint modId)
    {
        try
        {
            await modsClient[modId].Rate(NewRating.None);
            await Initialize();
            return true;
        }
        catch (RateLimitExceededException)
        {
            snackbarQueueService.EnqueueRateLimitMessage();
            logger.LogWarning("Being rate limited!");
            return false;
        }
    }
}
