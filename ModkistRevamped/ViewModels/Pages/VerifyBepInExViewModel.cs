using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Ionic.Zip;
using TNRD.Modkist.Services;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Pages;

public class VerifyBepInExViewModel : ObservableObject, INavigationAware
{
    private const string BEPINEX_URL =
        "https://github.com/BepInEx/BepInEx/releases/download/v5.4.22/BepInEx_x64_5.4.22.0.zip";

    private readonly SettingsService settingsService;
    private readonly INavigationService navigationService;
    private readonly HttpClient httpClient;
    private readonly IContentDialogService contentDialogService;

    public VerifyBepInExViewModel(
        SettingsService settingsService,
        INavigationService navigationService,
        HttpClient httpClient,
        IContentDialogService contentDialogService
    )
    {
        this.settingsService = settingsService;
        this.navigationService = navigationService;
        this.httpClient = httpClient;
        this.contentDialogService = contentDialogService;
    }

    async void INavigationAware.OnNavigatedTo()
    {
        if (HasBepInEx())
        {
            await Task.Delay(Random.Shared.Next(1000, 1500)); // Artificial delay to make it feel better
            navigationService.Navigate(typeof(VerifyLoginPage));
            return;
        }

        string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);
        string zipPath = Path.Combine(tempDirectory, "BepInEx.zip");
        await DownloadZip(zipPath);
        ExtractZip(zipPath);
        Directory.Delete(tempDirectory, true);

        if (!HasBepInEx()) // Validate
        {
            await contentDialogService.ShowAlertAsync("Uh-oh",
                "Failed to install/update BepInEx, please contact the developer of Modkist!",
                "OK");

            Application.Current.Shutdown(-1); // TODO: Does this need a custom code?
            return;
        }

        await Task.Delay(Random.Shared.Next(250, 500));
        navigationService.Navigate(typeof(VerifyLoginPage));
    }

    private bool HasBepInEx()
    {
        string dllPath = Path.Combine(settingsService.ZeepkistDirectory, "BepInEx", "core", "BepInEx.dll");

        if (File.Exists(dllPath))
        {
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(dllPath);
            return fileVersionInfo.FileVersion == "5.4.22.0";
        }

        return false;
    }

    private async Task DownloadZip(string zipPath)
    {
        HttpResponseMessage response = await httpClient.GetAsync(BEPINEX_URL);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            await contentDialogService.ShowAlertAsync("Uh-oh",
                "Failed to download BepInEx, please try again later!",
                "OK");

            // TODO: Log exception to file

            Application.Current.Shutdown(-1); // TODO: Does this need a custom code?
            return;
        }

        byte[] data;

        try
        {
            data = await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception e)
        {
            await contentDialogService.ShowAlertAsync("Uh-oh",
                "Failed to download BepInEx, please try again later!",
                "OK");

            // TODO: Log exception to file

            Application.Current.Shutdown(-1); // TODO: Does this need a custom code?
            return;
        }

        await File.WriteAllBytesAsync(zipPath, data);
    }

    private void ExtractZip(string zipPath)
    {
        using (FileStream stream = new(zipPath, FileMode.Open))
        {
            using (ZipFile? zip = ZipFile.Read(stream))
            {
                zip.ExtractAll(settingsService.ZeepkistDirectory, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }

    void INavigationAware.OnNavigatedFrom()
    {
        // Left empty on purpose
    }
}
