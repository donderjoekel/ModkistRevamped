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
    private readonly ModCachingService modCachingService;

    public VerifyModsJob(
        InstallationService installationService,
        ModManagerHostedService modManagerHostedService,
        ISubscriptionService subscriptionService,
        DependenciesService dependenciesService,
        ModCachingService modCachingService
    )
    {
        this.installationService = installationService;
        this.modManagerHostedService = modManagerHostedService;
        this.subscriptionService = subscriptionService;
        this.dependenciesService = dependenciesService;
        this.modCachingService = modCachingService;
    }

    public override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        List<Mod> subscribedMods = subscriptionService.GetSubscribedMods().ToList();
        List<InstalledModModel> installedMods = installationService.GetInstalledMods();

        VerifySubscribedMods(subscribedMods, installedMods);
        VerifyInstalledMods(installedMods, subscribedMods);
        VerifyDependencies(subscribedMods);

        return Task.CompletedTask;
    }

    private void VerifySubscribedMods(List<Mod> subscribedMods, IReadOnlyCollection<InstalledModModel> installedMods)
    {
        List<Mod> modsToAdd = new();
        List<Mod> modsToRemove = new();
        List<Mod> modsToUpdate = new();

        foreach (Mod mod in subscribedMods)
        {
            InstalledModModel? installedMod = installedMods.FirstOrDefault(x => x.ModId == mod.Id);

            if (installedMod == null)
            {
                modsToAdd.Add(mod);
            }
            else if (mod.Modfile == null)
            {
                modsToRemove.Add(mod);
            }
            else if (mod.Modfile.Id != installedMod.ModVersion)
            {
                modsToUpdate.Add(mod);
            }
        }

        if (modsToAdd.Count > 0)
        {
            modManagerHostedService.EnqueueAddModsJob(modsToAdd.ToArray());
        }

        if (modsToRemove.Count > 0)
        {
            modManagerHostedService.EnqueueRemoveModsJob(modsToRemove.ToArray());
        }

        if (modsToUpdate.Count > 0)
        {
            modManagerHostedService.EnqueueUpdateModsJob(modsToUpdate.ToArray());
        }
    }

    private void VerifyInstalledMods(List<InstalledModModel> installedMods, IReadOnlyCollection<Mod> subscribedMods)
    {
        List<Mod> modsToRemove = new();

        foreach (InstalledModModel installedMod in installedMods)
        {
            bool isAvailableOnline = subscribedMods.Any(x => x.Id == installedMod.ModId);

            if (!isAvailableOnline)
            {
                modsToRemove.Add(modCachingService[installedMod.ModId]);
            }
        }

        if (modsToRemove.Count > 0)
        {
            modManagerHostedService.EnqueueRemoveModsJob(modsToRemove.ToArray());
        }
    }

    private void VerifyDependencies(IEnumerable<Mod> subscribedMods)
    {
        List<Mod> modsToAdd = new();

        foreach (Mod mod in subscribedMods)
        {
            List<Mod> modDependencies = dependenciesService.GetDependencies(mod);
            modsToAdd.AddRange(modDependencies.Where(modDependency =>
                !subscriptionService.IsSubscribed(modDependency)));
        }

        if (modsToAdd.Count > 0)
        {
            modManagerHostedService.EnqueueAddModsJob(modsToAdd.ToArray());
        }
    }
}
