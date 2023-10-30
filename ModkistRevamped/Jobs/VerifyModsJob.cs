using Modio.Models;
using TNRD.Modkist.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Services.Hosted;
using TNRD.Modkist.Services.Subscription;

namespace TNRD.Modkist.Jobs;

public class VerifyModsJob : JobBase
{
    private readonly InstallationService installationService;
    private readonly ModManagerHostedService modManagerHostedService;
    private readonly ISubscriptionService subscriptionService;

    public VerifyModsJob(
        InstallationService installationService,
        ModManagerHostedService modManagerHostedService,
        ISubscriptionService subscriptionService
    )
    {
        this.installationService = installationService;
        this.modManagerHostedService = modManagerHostedService;
        this.subscriptionService = subscriptionService;
    }

    public override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        IEnumerable<Mod> subscribedMods = subscriptionService.GetSubscribedMods().ToList();
        List<InstalledModModel> installedMods = installationService.GetInstalledMods();

        foreach (Mod mod in subscribedMods)
        {
            InstalledModModel? installedMod = installedMods.FirstOrDefault(x => x.ModId == mod.Id);

            if (installedMod == null)
            {
                modManagerHostedService.EnqueueAddModJob(mod.Id);
            }
            else if (mod.Modfile == null)
            {
                modManagerHostedService.EnqueueRemoveModJob(mod.Id);
            }
            else if (mod.Modfile.Id != installedMod.ModVersion)
            {
                modManagerHostedService.EnqueueUpdateModJob(mod.Id);
            }
        }

        foreach (InstalledModModel installedMod in installedMods)
        {
            bool isAvailableOnline = subscribedMods.Any(x => x.Id == installedMod.ModId);

            if (!isAvailableOnline)
            {
                modManagerHostedService.EnqueueRemoveModJob(installedMod.ModId);
            }
        }

        return Task.CompletedTask;
    }
}
