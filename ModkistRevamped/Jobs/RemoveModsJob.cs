using Modio.Models;
using TNRD.Modkist.Services;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Jobs;

public class RemoveModsJob : JobBase
{
    private readonly Mod[] mods;
    private readonly InstallationService installationService;
    private readonly SnackbarQueueService snackbarQueueService;

    public RemoveModsJob(
        InstallationService installationService,
        SnackbarQueueService snackbarQueueService,
        Mod[] mods
    )
    {
        this.installationService = installationService;
        this.snackbarQueueService = snackbarQueueService;
        this.mods = mods;
    }

    public override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        snackbarQueueService.Enqueue("Remove", $"Removing {mods.Length} mods...");

        foreach (Mod mod in mods)
        {
            installationService.UninstallMod(mod);
        }

        snackbarQueueService.Enqueue("Remove",
            $"Removed {mods.Length} mods!",
            ControlAppearance.Secondary,
            new SymbolIcon(SymbolRegular.Delete24));

        return Task.CompletedTask;
    }
}
