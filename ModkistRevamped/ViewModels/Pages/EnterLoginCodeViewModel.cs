using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modio;
using TNRD.Modkist.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Settings;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Pages;

public partial class EnterLoginCodeViewModel : ObservableObject
{
    private readonly AuthClient authClient;
    private readonly LoginModel loginModel;
    private readonly SettingsService settingsService;
    private readonly IOptions<ModioSettings> modioSettings;
    private readonly INavigationService navigationService;
    private readonly IContentDialogService contentDialogService;
    private readonly ILogger<EnterLoginCodeViewModel> logger;
    private readonly SnackbarQueueService snackbarQueueService;

    [ObservableProperty] private string code = null!;
    [ObservableProperty] private bool loginButtonEnabled;

    public string Email => loginModel.Email;

    public EnterLoginCodeViewModel(
        AuthClient authClient,
        LoginModel loginModel,
        SettingsService settingsService,
        IOptions<ModioSettings> modioSettings,
        INavigationService navigationService,
        IContentDialogService contentDialogService,
        ILogger<EnterLoginCodeViewModel> logger,
        SnackbarQueueService snackbarQueueService
    )
    {
        this.authClient = authClient;
        this.loginModel = loginModel;
        this.settingsService = settingsService;
        this.modioSettings = modioSettings;
        this.navigationService = navigationService;
        this.contentDialogService = contentDialogService;
        this.logger = logger;
        this.snackbarQueueService = snackbarQueueService;
    }

    [RelayCommand]
    private async Task Login()
    {
        try
        {
            settingsService.AccessToken = await authClient.SecurityCode(modioSettings.Value.ApiKey, Code);
            settingsService.SelectedProfile = null!;
        }
        catch (UnauthorizedException)
        {
            snackbarQueueService.Enqueue("Invalid code",
                "That code doesn't seem to work!",
                ControlAppearance.Caution,
                new SymbolIcon(SymbolRegular.Warning24));

            return;
        }
        catch (Exception e)
        {
            await contentDialogService.ShowAlertAsync("Uh oh",
                "Seems like something went wrong while trying to log in, please try again later!",
                "OK");

            logger.LogError(e, "Failed to sign in");
            return;
        }

        // Restart application
        Process.Start(Environment.ProcessPath!);
        Application.Current.Shutdown();
    }

    partial void OnCodeChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        if (value.Length > 5)
            Code = value[..5];

        LoginButtonEnabled = Code.Length == 5;
    }

    [RelayCommand]
    private void SkipLogin()
    {
        settingsService.SkippedLogin = true;
        navigationService.Navigate(typeof(BrowsePluginsPage));
    }
}
