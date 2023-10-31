using Microsoft.Extensions.Logging;
using Modio;
using Modio.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Services.Subscription;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Jobs;

public class AddModJob : JobBase
{
    private readonly InstallationService installationService;
    private readonly DownloadService downloadService;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly ILogger<AddModJob> logger;
    private readonly ISubscriptionService subscriptionService;
    private readonly Mod mod;

    public AddModJob(
        InstallationService installationService,
        DownloadService downloadService,
        SnackbarQueueService snackbarQueueService,
        ILogger<AddModJob> logger,
        ISubscriptionService subscriptionService,
        Mod mod
    )
    {
        this.installationService = installationService;
        this.downloadService = downloadService;
        this.snackbarQueueService = snackbarQueueService;
        this.logger = logger;
        this.subscriptionService = subscriptionService;
        this.mod = mod;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (subscriptionService.IsSubscribed(mod) && installationService.IsInstalled(mod))
            return;

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
