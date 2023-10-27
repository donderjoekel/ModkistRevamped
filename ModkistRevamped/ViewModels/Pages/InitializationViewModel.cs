using System.IO;
using Microsoft.Win32;
using TNRD.Modkist.Services;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Pages;

public class InitializationViewModel : ObservableObject, INavigationAware
{
    private readonly SteamService steamService;
    private readonly SettingsService settingsService;
    private readonly INavigationService navigationService;

    public InitializationViewModel(
        SteamService steamService,
        SettingsService settingsService,
        INavigationService navigationService
    )
    {
        this.steamService = steamService;
        this.settingsService = settingsService;
        this.navigationService = navigationService;
    }

    async void INavigationAware.OnNavigatedTo()
    {
        if (HasValidZeepkistPath())
        {
            await FinishInitialization();
            return;
        }

        string? steamPath = steamService.GetSteamPath();
        if (!string.IsNullOrEmpty(steamPath) && steamService.TryGetGameFolder(steamPath, out string? gameFolder))
        {
            settingsService.ZeepkistDirectory = gameFolder!;
            await FinishInitialization();
            return;
        }

        string zeepkistPath = GetZeepkistPathThroughFileDialog();
        settingsService.ZeepkistDirectory = Path.GetDirectoryName(zeepkistPath)!;
        await FinishInitialization(false);
    }

    private async Task FinishInitialization(bool withArtificialDelay = true)
    {
        if (withArtificialDelay)
        {
            await Task.Delay(Random.Shared.Next(1000, 1500));
        }

        navigationService.Navigate(typeof(VerifyLoginPage));
    }

    void INavigationAware.OnNavigatedFrom()
    {
        // Left empty on purpose
    }

    private bool HasValidZeepkistPath()
    {
        if (string.IsNullOrEmpty(settingsService.ZeepkistDirectory))
            return false;

        if (!Directory.Exists(settingsService.ZeepkistDirectory))
            return false;

        string executablePath = Path.Combine(settingsService.ZeepkistDirectory, "Zeepkist.exe");
        return File.Exists(executablePath);
    }

    private string GetZeepkistPathThroughFileDialog()
    {
        OpenFileDialog ofd = new()
        {
            Multiselect = false,
            DefaultExt = ".exe",
            Filter = "Zeepkist|Zeepkist.exe",
            Title = "Select Zeepkist.exe"
        };

        bool? showDialog = ofd.ShowDialog();

        if (!showDialog.HasValue || !showDialog.Value)
            return GetZeepkistPathThroughFileDialog();

        return ofd.FileName;
    }
}
