using TNRD.Modkist.Models;
using TNRD.Modkist.Services;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace TNRD.Modkist.ViewModels.Controls;

public partial class ProfileListItemViewModel : ObservableObject
{
    private readonly ProfileModel profileModel;
    private readonly ProfileService profileService;
    private readonly SettingsService settingsService;

    public ProfileListItemViewModel(
        ProfileModel profileModel,
        ProfileService profileService,
        SettingsService settingsService
    )
    {
        this.profileModel = profileModel;
        this.profileService = profileService;
        this.settingsService = settingsService;
    }

    public ControlAppearance ButtonAppearance => profileService.IsSelectedProfile(profileModel)
        ? ControlAppearance.Primary
        : ControlAppearance.Secondary;

    public string ButtonContent => profileService.IsSelectedProfile(profileModel)
        ? "Currently Active"
        : "Activate";

    public string Name => profileModel.DisplayName;

    public bool IsActivateButtonEnabled
    {
        get
        {
            if (profileModel.Type == ProfileType.Remote)
                return !profileService.IsSelectedProfile(profileModel) && settingsService.HasValidAccessToken();

            return !profileService.IsSelectedProfile(profileModel);
        }
    }

    public bool IsDeleteEnabled =>
        !profileService.IsSelectedProfile(profileModel) && profileModel.Type == ProfileType.Local;

    [RelayCommand]
    private void SelectProfile()
    {
        profileService.SelectProfile(profileModel);
    }

    [RelayCommand]
    private async Task DeleteProfile()
    {
        MessageBox messageBox = new()
        {
            Title = "Caution!",
            Content = "Are you sure you want to delete this profile?",
            PrimaryButtonText = "Yes",
            IsPrimaryButtonEnabled = true,
            PrimaryButtonAppearance = ControlAppearance.Danger,
            CloseButtonAppearance = ControlAppearance.Secondary,
            CloseButtonText = "No"
        };

        MessageBoxResult messageBoxResult = await messageBox.ShowDialogAsync();
        if (messageBoxResult == MessageBoxResult.Primary)
        {
            profileService.DeleteProfile(profileModel);
        }
    }
}
