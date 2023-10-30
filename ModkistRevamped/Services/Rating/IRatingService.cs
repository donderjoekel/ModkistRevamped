using Modio.Models;

namespace TNRD.Modkist.Services.Rating;

public interface IRatingService
{
    bool CanRate { get; }

    Task Initialize();
    bool HasUpvoted(Mod mod);
    bool HasUpvoted(uint modId);
    bool HasDownvoted(Mod mod);
    bool HasDownvoted(uint modId);
    Task Upvote(Mod mod);
    Task Upvote(uint modId);
    Task Downvote(Mod mod);
    Task Downvote(uint modId);
    Task RemoveRating(Mod mod);
    Task RemoveRating(uint modId);
}
