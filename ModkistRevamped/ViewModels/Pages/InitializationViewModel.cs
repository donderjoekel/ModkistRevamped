using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using Octokit;
using TNRD.Modkist.Services;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Application = System.Windows.Application;

namespace TNRD.Modkist.ViewModels.Pages;

public partial class InitializationViewModel : ObservableObject, INavigationAware
{
    private readonly SteamService steamService;
    private readonly SettingsService settingsService;
    private readonly INavigationService navigationService;
    private readonly IGitHubClient gitHubClient;
    private readonly IContentDialogService contentDialogService;

    public InitializationViewModel(
        SteamService steamService,
        SettingsService settingsService,
        INavigationService navigationService,
        IGitHubClient gitHubClient,
        IContentDialogService contentDialogService
    )
    {
        this.steamService = steamService;
        this.settingsService = settingsService;
        this.navigationService = navigationService;
        this.gitHubClient = gitHubClient;
        this.contentDialogService = contentDialogService;
    }

    [ObservableProperty] private string? textContent;

    async void INavigationAware.OnNavigatedTo()
    {
        await CheckForUpdate();

        TextContent = "Checking for Zeepkist...";

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

    private async Task CheckForUpdate()
    {
        TextContent = "Checking for update...";

        Release? latestRelease = await gitHubClient.Repository.Release.GetLatest("tnrd-org", "ModkistRevamped");
        Version latestReleaseVersion = Version.Parse(latestRelease.TagName.TrimStart('v', 'V'));
        Version? version = Assembly.GetExecutingAssembly().GetName().Version;

        if (version < latestReleaseVersion)
        {
            ContentDialogResult result = await contentDialogService.ShowSimpleDialogAsync(
                new SimpleContentDialogCreateOptions()
                {
                    Title = "Update available",
                    Content = "A new version of Modkist is available. Do you want to download the latest version?",
                    CloseButtonText = "No",
                    PrimaryButtonText = "Yes"
                });

            if (result == ContentDialogResult.Primary)
            {
                Process.Start("explorer", latestRelease.HtmlUrl);
                Application.Current.Shutdown();
            }
        }
    }

    private async Task FinishInitialization(bool withArtificialDelay = true)
    {
        if (withArtificialDelay)
        {
            await Task.Delay(Random.Shared.Next(1000, 1500));
        }

        navigationService.Navigate(typeof(VerifyBepInExPage));
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
