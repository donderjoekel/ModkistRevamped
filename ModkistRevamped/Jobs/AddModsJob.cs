using Modio.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Services.Subscription;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Jobs;

public class AddModsJob : JobBase
{
    private readonly InstallationService installationService;
    private readonly DownloadService downloadService;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly ISubscriptionService subscriptionService;
    private readonly Mod[] mods;

    public AddModsJob(
        InstallationService installationService,
        DownloadService downloadService,
        SnackbarQueueService snackbarQueueService,
        ISubscriptionService subscriptionService,
        Mod[] mods
    )
    {
        this.installationService = installationService;
        this.downloadService = downloadService;
        this.snackbarQueueService = snackbarQueueService;
        this.subscriptionService = subscriptionService;
        this.mods = mods;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        snackbarQueueService.Enqueue("Install", $"Installing {mods.Length} mods...");

        int failedMods = 0;
        int installedMods = 0;

        foreach (Mod mod in mods)
        {
            if (subscriptionService.IsSubscribed(mod) && installationService.IsInstalled(mod))
                continue;

            string? downloadedFilePath = await downloadService.DownloadMod(mod);

            if (string.IsNullOrEmpty(downloadedFilePath))
            {
                failedMods++;
            }
            else
            {
                installationService.InstallMod(mod, downloadedFilePath);
                installedMods++;
            }
        }

        if (failedMods == 0)
        {
            snackbarQueueService.Enqueue("Install",
                $"Installed {installedMods} mods!",
                ControlAppearance.Secondary,
                new SymbolIcon(SymbolRegular.Checkmark24));
        }
        else
        {
            snackbarQueueService.Enqueue("Install",
                $"Installed {installedMods} mods, but failed to install {failedMods} mods!",
                ControlAppearance.Caution,
                new SymbolIcon(SymbolRegular.Warning24));
        }
    }
}
