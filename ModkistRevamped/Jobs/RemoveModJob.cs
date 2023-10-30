using Microsoft.Extensions.Logging;
using Modio;
using Modio.Models;
using TNRD.Modkist.Services;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Jobs;

public class RemoveModJob : JobBase
{
    private readonly uint modId;
    private readonly ModsClient modsClient;
    private readonly InstallationService installationService;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly ILogger<RemoveModJob> logger;

    public RemoveModJob(
        uint modId,
        ModsClient modsClient,
        InstallationService installationService,
        SnackbarQueueService snackbarQueueService,
        ILogger<RemoveModJob> logger
    )
    {
        this.modId = modId;
        this.modsClient = modsClient;
        this.installationService = installationService;
        this.snackbarQueueService = snackbarQueueService;
        this.logger = logger;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (await RegularUninstall())
            return;

        installationService.UninstallUnknownMod(modId);

        snackbarQueueService.Enqueue("Remove",
            "Unlisted mod has been removed!",
            ControlAppearance.Secondary,
            new SymbolIcon(SymbolRegular.Delete24));
    }

    private async Task<bool> RegularUninstall()
    {
        try
        {
            Mod mod = await modsClient[modId].Get();
            installationService.UninstallMod(mod);

            snackbarQueueService.Enqueue("Remove",
                $"'{mod.Name}' has been removed!",
                ControlAppearance.Secondary,
                new SymbolIcon(SymbolRegular.Delete24));

            return true;
        }
        catch (RateLimitExceededException)
        {
            snackbarQueueService.EnqueueRateLimitMessage();
            logger.LogWarning("Being rate limited!");
            return true; // Returning true here because we simply want to abort
        }
        catch (NotFoundException e)
        {
            return false;
        }
    }
}
