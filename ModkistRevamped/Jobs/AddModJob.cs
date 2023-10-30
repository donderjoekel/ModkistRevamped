using Modio;
using Modio.Models;
using TNRD.Modkist.Services;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Jobs;

public class AddModJob : JobBase
{
    private readonly uint modId;
    private readonly ModsClient modsClient;
    private readonly InstallationService installationService;
    private readonly DownloadService downloadService;
    private readonly SnackbarQueueService snackbarQueueService;

    public AddModJob(
        uint modId,
        ModsClient modsClient,
        InstallationService installationService,
        DownloadService downloadService,
        SnackbarQueueService snackbarQueueService
    )
    {
        this.modId = modId;
        this.modsClient = modsClient;
        this.installationService = installationService;
        this.downloadService = downloadService;
        this.snackbarQueueService = snackbarQueueService;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Mod mod = await modsClient[modId].Get();

        string? downloadedFilePath = await downloadService.DownloadMod(mod);

        if (string.IsNullOrEmpty(downloadedFilePath))
        {
            snackbarQueueService.Enqueue("Install",
                $"Unable to install '{mod.Name}'",
                ControlAppearance.Caution,
                new SymbolIcon(SymbolRegular.Warning24));

            return;
        }

        installationService.InstallMod(mod, downloadedFilePath);

        snackbarQueueService.Enqueue("Install",
            $"'{mod.Name}' has been installed!",
            ControlAppearance.Secondary,
            new SymbolIcon(SymbolRegular.Checkmark24));
    }
}
