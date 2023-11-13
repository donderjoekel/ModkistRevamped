using Modio.Models;
using TNRD.Modkist.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Services.Hosted;
using TNRD.Modkist.Services.Subscription;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Jobs;

public class VerifyModsJob : JobBase
{
    private readonly InstallationService installationService;
    private readonly ModManagerHostedService modManagerHostedService;
    private readonly ISubscriptionService subscriptionService;
    private readonly DependenciesService dependenciesService;
    private readonly ModCachingService modCachingService;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly ISnackbarService snackbarService;

    public VerifyModsJob(
        InstallationService installationService,
        ModManagerHostedService modManagerHostedService,
        ISubscriptionService subscriptionService,
        DependenciesService dependenciesService,
        ModCachingService modCachingService,
        SnackbarQueueService snackbarQueueService,
        ISnackbarService snackbarService
    )
    {
        this.installationService = installationService;
        this.modManagerHostedService = modManagerHostedService;
        this.subscriptionService = subscriptionService;
        this.dependenciesService = dependenciesService;
        this.modCachingService = modCachingService;
        this.snackbarQueueService = snackbarQueueService;
        this.snackbarService = snackbarService;
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

        if (modsToAdd.Count > 1)
        {
            modManagerHostedService.EnqueueAddModsJob(modsToAdd.ToArray());
        }
        else if (modsToAdd.Count == 1)
        {
            modManagerHostedService.EnqueueAddModJob(modsToAdd.First());
        }

        if (modsToRemove.Count > 1)
        {
            modManagerHostedService.EnqueueRemoveModsJob(modsToRemove.ToArray());
        }
        else if (modsToRemove.Count == 1)
        {
            modManagerHostedService.EnqueueRemoveModJob(modsToRemove.First());
        }

        if (modsToUpdate.Count > 1)
        {
            modManagerHostedService.EnqueueUpdateModsJob(modsToUpdate.ToArray());
        }
        else if (modsToUpdate.Count == 1)
        {
            modManagerHostedService.EnqueueUpdateModJob(modsToUpdate.First());
        }

        if (modsToAdd.Count == 0 && modsToRemove.Count == 0 && modsToUpdate.Count == 0)
        {
            snackbarQueueService.Enqueue("Hurray!",
                "All mods are up to date",
                ControlAppearance.Secondary,
                null,
                snackbarService.DefaultTimeOut * 2);
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
