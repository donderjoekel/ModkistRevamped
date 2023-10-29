using Microsoft.Extensions.Options;
using Modio;
using TNRD.Modkist.Helpers;
using TNRD.Modkist.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Settings;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui;

namespace TNRD.Modkist.ViewModels.Pages;

public partial class RequestLoginCodeViewModel : ObservableObject
{
    private readonly INavigationService navigationService;
    private readonly AuthClient authClient;
    private readonly LoginModel loginModel;
    private readonly IOptions<ModioSettings> modioSettings;
    private readonly SettingsService settingsService;

    [ObservableProperty] private string email = null!;
    [ObservableProperty] private bool enterCodeButtonEnabled;

    public RequestLoginCodeViewModel(
        INavigationService navigationService,
        AuthClient authClient,
        LoginModel loginModel,
        IOptions<ModioSettings> modioSettings,
        SettingsService settingsService
    )
    {
        this.navigationService = navigationService;
        this.authClient = authClient;
        this.loginModel = loginModel;
        this.modioSettings = modioSettings;
        this.settingsService = settingsService;
    }

    [RelayCommand]
    private async Task EmailCode()
    {
        await authClient.RequestCode(modioSettings.Value.ApiKey, Email);
        navigationService.Navigate(typeof(EnterLoginCodePage));
    }

    partial void OnEmailChanging(string value)
    {
        EnterCodeButtonEnabled = RegexHelper.IsValidEmail(value);
        loginModel.Email = value;
    }

    [RelayCommand]
    private void SkipLogin()
    {
        settingsService.SkippedLogin = true;
        navigationService.Navigate(typeof(BrowsePluginsPage));
    }
}
