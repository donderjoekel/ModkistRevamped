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

    public Task Upvote(Mod mod)
    {
        return Task.CompletedTask;
    }

    public Task Upvote(uint modId)
    {
        return Task.CompletedTask;
    }

    public Task Downvote(Mod mod)
    {
        return Task.CompletedTask;
    }

    public Task Downvote(uint modId)
    {
        return Task.CompletedTask;
    }

    public Task RemoveRating(Mod mod)
    {
        return Task.CompletedTask;
    }

    public Task RemoveRating(uint modId)
    {
        return Task.CompletedTask;
    }
}
