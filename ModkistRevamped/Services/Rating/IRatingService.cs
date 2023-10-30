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
    Task<bool> Upvote(Mod mod);
    Task<bool> Upvote(uint modId);
    Task<bool> Downvote(Mod mod);
    Task<bool> Downvote(uint modId);
    Task<bool> RemoveRating(Mod mod);
    Task<bool> RemoveRating(uint modId);
}
