using System.Diagnostics;
using Microsoft.Extensions.Options;
using Modio;
using TNRD.Modkist.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Settings;
using Wpf.Ui;

namespace TNRD.Modkist.ViewModels.Pages;

public partial class EnterLoginCodeViewModel : ObservableObject
{
    private readonly AuthClient authClient;
    private readonly LoginModel loginModel;
    private readonly SettingsService settingsService;
    private readonly IOptions<ModioSettings> modioSettings;

    [ObservableProperty] private string code = null!;
    [ObservableProperty] private bool loginButtonEnabled;

    public string Email => loginModel.Email;

    public EnterLoginCodeViewModel(
        AuthClient authClient,
        LoginModel loginModel,
        SettingsService settingsService,
        IOptions<ModioSettings> modioSettings
    )
    {
        this.authClient = authClient;
        this.loginModel = loginModel;
        this.settingsService = settingsService;
        this.modioSettings = modioSettings;
    }

    [RelayCommand]
    private async Task Login()
    {
        try
        {
            settingsService.AccessToken = await authClient.SecurityCode(modioSettings.Value.ApiKey, Code);
        }
        catch (Exception e)
        {
            // TODO: Handle properly
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
}
