using TNRD.Modkist.Services;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

public partial class ModRatingsViewModel : ObservableObject
{
    private readonly SelectedModService selectedModService;
    private readonly SettingsService settingsService;
    private readonly RatingService ratingService;
    private readonly ISnackbarService snackbarService;

    public ModRatingsViewModel(
        SelectedModService selectedModService,
        SettingsService settingsService,
        RatingService ratingService,
        ISnackbarService snackbarService
    )
    {
        this.selectedModService = selectedModService;
        this.settingsService = settingsService;
        this.ratingService = ratingService;
        this.snackbarService = snackbarService;

        UpdateButtonAppearance();
    }

    [ObservableProperty] private ControlAppearance upvoteAppearance;
    [ObservableProperty] private ControlAppearance downvoteAppearance;

    public bool CanVote => settingsService.HasValidAccessToken();

    public uint Upvotes => selectedModService.SelectedMod?.Stats?.PositiveRatings ?? 0;
    public uint Downvotes => selectedModService.SelectedMod?.Stats?.NegativeRatings ?? 0;

    [RelayCommand]
    private async Task Downvote()
    {
        if (ratingService.HasDownvoted(selectedModService.SelectedMod!))
        {
            await ratingService.RemoveRating(selectedModService.SelectedMod!);

            snackbarService.Show("Downvote",
                "Your downvote has been removed!",
                ControlAppearance.Secondary,
                null,
                TimeSpan.FromSeconds(2.5d));
        }
        else
        {
            await ratingService.Downvote(selectedModService.SelectedMod!);

            snackbarService.Show("Downvote",
                "Your downvote has been submitted!",
                ControlAppearance.Secondary,
                null,
                TimeSpan.FromSeconds(2.5d));
        }

        UpdateButtonAppearance();
    }

    [RelayCommand]
    private async Task Upvote()
    {
        if (ratingService.HasUpvoted(selectedModService.SelectedMod!))
        {
            await ratingService.RemoveRating(selectedModService.SelectedMod!);

            snackbarService.Show("Upvote",
                "Your upvote has been removed!",
                ControlAppearance.Secondary,
                null,
                TimeSpan.FromSeconds(2.5d));
        }
        else
        {
            await ratingService.Upvote(selectedModService.SelectedMod!);

            snackbarService.Show("Upvote",
                "Your upvote has been submitted!",
                ControlAppearance.Secondary,
                null,
                TimeSpan.FromSeconds(2.5d));
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
