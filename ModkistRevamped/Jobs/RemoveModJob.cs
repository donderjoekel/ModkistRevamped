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

    public RemoveModJob(
        uint modId,
        ModsClient modsClient,
        InstallationService installationService,
        SnackbarQueueService snackbarQueueService
    )
    {
        this.modId = modId;
        this.modsClient = modsClient;
        this.installationService = installationService;
        this.snackbarQueueService = snackbarQueueService;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (await RegularUninstall())
            return;

        installationService.UninstallUnknownMod(modId);

        snackbarQueueService.Enqueue("Remove",
            "Unlisted mod has been removed!",
            ControlAppearance.Secondary,
            new SymbolIcon(SymbolRegular.Delete24),
            TimeSpan.FromSeconds(2.5d));
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
                new SymbolIcon(SymbolRegular.Delete24),
                TimeSpan.FromSeconds(2.5d));

            return true;
        }
        catch (NotFoundException e)
        {
            return false;
        }
    }
}
