using Microsoft.Extensions.Logging;
using Modio;
using Modio.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Services.Subscription;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Jobs;

public class AddModJob : JobBase
{
    private readonly uint modId;
    private readonly ModsClient modsClient;
    private readonly InstallationService installationService;
    private readonly DownloadService downloadService;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly ILogger<AddModJob> logger;
    private readonly ISubscriptionService subscriptionService;

    public AddModJob(
        uint modId,
        ModsClient modsClient,
        InstallationService installationService,
        DownloadService downloadService,
        SnackbarQueueService snackbarQueueService,
        ILogger<AddModJob> logger,
        ISubscriptionService subscriptionService
    )
    {
        this.modId = modId;
        this.modsClient = modsClient;
        this.installationService = installationService;
        this.downloadService = downloadService;
        this.snackbarQueueService = snackbarQueueService;
        this.logger = logger;
        this.subscriptionService = subscriptionService;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (subscriptionService.IsSubscribed(modId))
                return; // Early out if already subscribed

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
        catch (RateLimitExceededException)
        {
            snackbarQueueService.EnqueueRateLimitMessage();
            logger.LogWarning("Being rate limited!");
        }
    }
}
