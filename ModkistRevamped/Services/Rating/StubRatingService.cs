using Modio.Models;

namespace TNRD.Modkist.Services.Rating;

public class StubRatingService : IRatingService
{
    public bool CanRate => false;

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public bool HasUpvoted(Mod mod)
    {
        return false;
    }

    public bool HasUpvoted(uint modId)
    {
        return false;
    }

    public bool HasDownvoted(Mod mod)
    {
        return false;
    }

    public bool HasDownvoted(uint modId)
    {
        return false;
    }

    public Task<bool> Upvote(Mod mod)
    {
        return Task.FromResult(false);
    }

    public Task<bool> Upvote(uint modId)
    {
        return Task.FromResult(false);
    }

    public Task<bool> Downvote(Mod mod)
    {
        return Task.FromResult(false);
    }

    public Task<bool> Downvote(uint modId)
    {
        return Task.FromResult(false);
    }

    public Task<bool> RemoveRating(Mod mod)
    {
        return Task.FromResult(false);
    }

    public Task<bool> RemoveRating(uint modId)
    {
        return Task.FromResult(false);
    }
}
