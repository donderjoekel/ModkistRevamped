using Modio.Models;
using TNRD.Modkist.Services;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Jobs;

public class RemoveModJob : JobBase
{
    private readonly InstallationService installationService;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly Mod mod;

    public RemoveModJob(
        InstallationService installationService,
        SnackbarQueueService snackbarQueueService,
        Mod mod
    )
    {
        this.installationService = installationService;
        this.snackbarQueueService = snackbarQueueService;
        this.mod = mod;
    }

    public override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        installationService.UninstallMod(mod);

        snackbarQueueService.Enqueue("Remove",
            $"'{mod.Name}' has been removed!",
            ControlAppearance.Secondary,
            new SymbolIcon(SymbolRegular.Delete24));

        return Task.CompletedTask;
    }
}
