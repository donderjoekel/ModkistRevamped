using Modio.Models;
using TNRD.Modkist.Services;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Jobs;

public class UpdateModsJob : JobBase
{
    private readonly InstallationService installationService;
    private readonly DownloadService downloadService;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly Mod[] mods;

    public UpdateModsJob(
        InstallationService installationService,
        DownloadService downloadService,
        SnackbarQueueService snackbarQueueService,
        Mod[] mods
    )
    {
        this.installationService = installationService;
        this.downloadService = downloadService;
        this.snackbarQueueService = snackbarQueueService;
        this.mods = mods;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        snackbarQueueService.Enqueue("Update", $"Updating {mods.Length} mods...");

        int amountFailed = 0;
        int amountUpdated = 0;

        foreach (Mod mod in mods)
        {
            string? downloadedFilePath = await downloadService.DownloadMod(mod);

            if (string.IsNullOrEmpty(downloadedFilePath))
            {
                amountFailed++;
                continue;
            }

            installationService.UninstallMod(mod);
            installationService.InstallMod(mod, downloadedFilePath);

            amountUpdated++;
        }

        if (amountFailed > 0)
        {
            snackbarQueueService.Enqueue("Update",
                $"Updated {amountUpdated} mods but failed to update {amountFailed} mods!",
                ControlAppearance.Caution,
                new SymbolIcon(SymbolRegular.Warning24));
        }
        else
        {
            snackbarQueueService.Enqueue("Update",
                $"'Updated {amountUpdated} mods!",
                ControlAppearance.Secondary,
                new SymbolIcon(SymbolRegular.Checkmark24));
        }
    }
}
