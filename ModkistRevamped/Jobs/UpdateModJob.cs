using Modio.Models;
using TNRD.Modkist.Services;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Jobs;

public class UpdateModJob : JobBase
{
    private readonly InstallationService installationService;
    private readonly DownloadService downloadService;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly Mod mod;

    public UpdateModJob(
        InstallationService installationService,
        DownloadService downloadService,
        SnackbarQueueService snackbarQueueService,
        Mod mod
    )
    {
        this.installationService = installationService;
        this.downloadService = downloadService;
        this.snackbarQueueService = snackbarQueueService;
        this.mod = mod;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        string? downloadedFilePath = await downloadService.DownloadMod(mod);

        if (string.IsNullOrEmpty(downloadedFilePath))
        {
            snackbarQueueService.Enqueue("Update",
                $"Unable to update '{mod.Name}'",
                ControlAppearance.Caution,
                new SymbolIcon(SymbolRegular.Warning24));

            return;
        }

        installationService.UninstallMod(mod);
        installationService.InstallMod(mod, downloadedFilePath);

        snackbarQueueService.Enqueue("Update",
            $"'{mod.Name}' has been updated!",
            ControlAppearance.Secondary,
            new SymbolIcon(SymbolRegular.Checkmark24));
    }
}
