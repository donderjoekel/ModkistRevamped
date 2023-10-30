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
    private readonly DependenciesService dependenciesService;

    public VerifyModsJob(
        InstallationService installationService,
        ModManagerHostedService modManagerHostedService,
        ISubscriptionService subscriptionService,
        DependenciesService dependenciesService
    )
    {
        this.installationService = installationService;
        this.modManagerHostedService = modManagerHostedService;
        this.subscriptionService = subscriptionService;
        this.dependenciesService = dependenciesService;
    }

    public override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        List<Mod> subscribedMods = subscriptionService.GetSubscribedMods().ToList();
        List<InstalledModModel> installedMods = installationService.GetInstalledMods();
        IReadOnlyList<uint> dependencies = dependenciesService.GetAllDependencies();

        VerifySubscribedMods(subscribedMods, installedMods);
        VerifyInstalledMods(installedMods, subscribedMods);
        VerifyDependencies(dependencies, subscribedMods);

        return Task.CompletedTask;
    }

    private void VerifySubscribedMods(List<Mod> subscribedMods, IReadOnlyCollection<InstalledModModel> installedMods)
    {
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
    }

    private void VerifyInstalledMods(List<InstalledModModel> installedMods, IReadOnlyCollection<Mod> subscribedMods)
    {
        foreach (InstalledModModel installedMod in installedMods)
        {
            bool isAvailableOnline = subscribedMods.Any(x => x.Id == installedMod.ModId);

            if (!isAvailableOnline)
            {
                modManagerHostedService.EnqueueRemoveModJob(installedMod.ModId);
            }
        }
    }

    private void VerifyDependencies(IEnumerable<uint> dependencies, IReadOnlyCollection<Mod> subscribedMods)
    {
        foreach (uint dependency in dependencies)
        {
            if (subscribedMods.All(x => x.Id != dependency))
            {
                modManagerHostedService.EnqueueAddModJob(dependency);
            }
        }
    }
}
