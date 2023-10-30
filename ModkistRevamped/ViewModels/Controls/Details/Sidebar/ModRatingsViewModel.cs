using TNRD.Modkist.Services;
using TNRD.Modkist.Services.Rating;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

public partial class ModRatingsViewModel : ObservableObject
{
    private readonly SelectedModService selectedModService;
    private readonly IRatingService ratingService;
    private readonly SnackbarQueueService snackbarQueueService;

    public ModRatingsViewModel(
        SelectedModService selectedModService,
        IRatingService ratingService,
        SnackbarQueueService snackbarQueueService
    )
    {
        this.selectedModService = selectedModService;
        this.ratingService = ratingService;
        this.snackbarQueueService = snackbarQueueService;

        UpdateButtonAppearance();
    }

    [ObservableProperty] private ControlAppearance upvoteAppearance;
    [ObservableProperty] private ControlAppearance downvoteAppearance;

    public bool CanVote => ratingService.CanRate;

    public uint Upvotes => selectedModService.SelectedMod?.Stats?.PositiveRatings ?? 0;
    public uint Downvotes => selectedModService.SelectedMod?.Stats?.NegativeRatings ?? 0;

    [RelayCommand]
    private async Task Downvote()
    {
        if (ratingService.HasDownvoted(selectedModService.SelectedMod!))
        {
            if (!await ratingService.RemoveRating(selectedModService.SelectedMod!))
                return;

            snackbarQueueService.Enqueue("Downvote",
                "Your downvote has been removed!",
                ControlAppearance.Secondary,
                new SymbolIcon(SymbolRegular.ThumbLikeDislike24));
        }
        else
        {
            if (!await ratingService.Downvote(selectedModService.SelectedMod!))
                return;

            snackbarQueueService.Enqueue("Downvote",
                "Your downvote has been submitted!",
                ControlAppearance.Secondary,
                new SymbolIcon(SymbolRegular.ThumbDislike24));
        }

        UpdateButtonAppearance();
    }

    [RelayCommand]
    private async Task Upvote()
    {
        if (ratingService.HasUpvoted(selectedModService.SelectedMod!))
        {
            if (!await ratingService.RemoveRating(selectedModService.SelectedMod!))
                return;

            snackbarQueueService.Enqueue("Upvote",
                "Your upvote has been removed!",
                ControlAppearance.Secondary,
                new SymbolIcon(SymbolRegular.ThumbLikeDislike24));
        }
        else
        {
            if (!await ratingService.Upvote(selectedModService.SelectedMod!))
                return;

            snackbarQueueService.Enqueue("Upvote",
                "Your upvote has been submitted!",
                ControlAppearance.Secondary,
                new SymbolIcon(SymbolRegular.ThumbLike24));
        }

        UpdateButtonAppearance();
    }

    private void UpdateButtonAppearance()
    {
        UpvoteAppearance = ratingService.HasUpvoted(selectedModService.SelectedMod!)
            ? ControlAppearance.Primary
            : ControlAppearance.Secondary;

        DownvoteAppearance = ratingService.HasDownvoted(selectedModService.SelectedMod!)
            ? ControlAppearance.Primary
            : ControlAppearance.Secondary;
    }
}
