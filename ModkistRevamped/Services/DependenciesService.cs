using Microsoft.Extensions.Logging;
using Modio;
using Modio.Models;
using TNRD.Modkist.Services.Subscription;

namespace TNRD.Modkist.Services;

public class DependenciesService
{
    public delegate void DependencyAddedDelegate(uint dependencyId);

    public delegate void DependencyRemovedDelegate(uint dependencyId);

    private readonly ISubscriptionService subscriptionService;
    private readonly ModsClient modsClient;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly ILogger<DependenciesService> logger;
    private readonly Dictionary<uint, HashSet<uint>> dependencyIdToModIds = new();

    public DependenciesService(
        ISubscriptionService subscriptionService,
        ModsClient modsClient,
        SnackbarQueueService snackbarQueueService,
        ILogger<DependenciesService> logger
    )
    {
        this.subscriptionService = subscriptionService;
        this.modsClient = modsClient;
        this.snackbarQueueService = snackbarQueueService;
        this.logger = logger;

        this.subscriptionService.SubscriptionAdded += OnSubscriptionAdded;
        this.subscriptionService.SubscriptionRemoved += OnSubscriptionRemoved;
    }

    public event DependencyAddedDelegate? DependencyAdded;
    public event DependencyRemovedDelegate? DependencyRemoved;

    public async Task Initialize()
    {
        IEnumerable<Mod> subscribedMods = subscriptionService.GetSubscribedMods();

        foreach (Mod subscribedMod in subscribedMods)
        {
            try
            {
                IReadOnlyList<Dependency> dependencies = await modsClient[subscribedMod.Id].Dependencies.Get();
                foreach (Dependency dependency in dependencies)
                {
                    dependencyIdToModIds.TryAdd(dependency.ModId, new HashSet<uint>());
                    if (dependencyIdToModIds[dependency.ModId].Add(subscribedMod.Id))
                    {
                        DependencyAdded?.Invoke(dependency.ModId);
                    }
                }
            }
            catch (RateLimitExceededException)
            {
                snackbarQueueService.EnqueueRateLimitMessage();
                logger.LogWarning("Being rate limited!");

                // Early out here because of the rate limiting
                return;
            }
        }
    }

    private async void OnSubscriptionAdded(uint modId)
    {
        IReadOnlyList<Dependency> dependencies = await modsClient[modId].Dependencies.Get();
        foreach (Dependency dependency in dependencies)
        {
            dependencyIdToModIds.TryAdd(dependency.ModId, new HashSet<uint>());
            if (dependencyIdToModIds[dependency.ModId].Add(modId))
            {
                DependencyAdded?.Invoke(dependency.ModId);
            }
        }
    }

    private void OnSubscriptionRemoved(uint modId)
    {
        foreach (KeyValuePair<uint, HashSet<uint>> kvp in dependencyIdToModIds)
        {
            if (kvp.Value.Remove(modId))
            {
                DependencyRemoved?.Invoke(kvp.Key);
            }
        }
    }

    public bool IsDependency(Mod mod)
    {
        return IsDependency(mod.Id);
    }

    public bool IsDependency(uint modId)
    {
        if (dependencyIdToModIds.TryGetValue(modId, out HashSet<uint>? dependencyIds))
            return dependencyIds.Count > 0;
        return false;
    }
}
