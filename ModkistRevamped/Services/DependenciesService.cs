using System.IO;
using Modio;
using Modio.Models;
using Newtonsoft.Json;
using TNRD.Modkist.Models;
using TNRD.Modkist.Services.Subscription;
using File = System.IO.File;

namespace TNRD.Modkist.Services;

public class DependenciesService
{
    private readonly Dictionary<uint, HashSet<uint>> dependencyIdToModIds = new();
    private readonly ModsClient modsClient;
    private readonly ModCachingService modCachingService;
    private readonly ISubscriptionService subscriptionService;

    public DependenciesService(
        ModsClient modsClient,
        ModCachingService modCachingService,
        ISubscriptionService subscriptionService
    )
    {
        this.modsClient = modsClient;
        this.modCachingService = modCachingService;
        this.subscriptionService = subscriptionService;
    }

    public async Task Initialize()
    {
        dependencyIdToModIds.Clear();

        List<uint> modsToCheck = new();
        List<DependencyModel> dependencyModels = LoadDependenciesFromDisk();

        foreach (Mod mod in modCachingService)
        {
            DependencyModel? dependency = dependencyModels.FirstOrDefault(x => x.ModId == mod.Id);

            if (dependency == null)
            {
                modsToCheck.Add(mod.Id);
            }
            else if (mod.DateUpdated > dependency.Stamp)
            {
                modsToCheck.Add(mod.Id);
            }
            else
            {
                dependencyIdToModIds.Add(mod.Id, dependency.Dependencies);
            }
        }

        foreach (uint modId in modsToCheck)
        {
            IReadOnlyList<Dependency> dependencies = await modsClient[modId].Dependencies.Get();

            HashSet<uint> dependencyIds = new();
            foreach (Dependency dependency in dependencies)
            {
                dependencyIds.Add(dependency.ModId);
            }

            dependencyIdToModIds.Add(modId, dependencyIds);
        }

        SaveDependenciesToDisk();
    }

    public bool IsDependency(Mod mod)
    {
        return IsDependency(mod.Id);
    }

    public bool IsDependency(uint modId)
    {
        IEnumerable<KeyValuePair<uint, HashSet<uint>>> kvps = dependencyIdToModIds.Where(x => x.Value.Contains(modId));
        return kvps.Any(kvp => subscriptionService.IsSubscribed(kvp.Key));
    }

    public List<Mod> GetDependingMods(Mod mod)
    {
        return GetDependingMods(mod.Id);
    }

    public List<Mod> GetDependingMods(uint modId)
    {
        List<uint> dependingModIds = new();

        foreach (KeyValuePair<uint, HashSet<uint>> kvp in dependencyIdToModIds)
        {
            if (kvp.Value.Contains(modId))
            {
                dependingModIds.Add(kvp.Key);
            }
        }

        return dependingModIds.Distinct().Select(x => modCachingService[x]).ToList();
    }

    public List<Mod> GetDependencies(Mod mod)
    {
        return GetDependencies(mod.Id);
    }

    public List<Mod> GetDependencies(uint modId)
    {
        if (!dependencyIdToModIds.TryGetValue(modId, out HashSet<uint>? dependencyIds))
        {
            return new List<Mod>();
        }

        return dependencyIds.Select(x => modCachingService[x]).ToList();
    }

    public IReadOnlyList<uint> GetAllDependencies()
    {
        return dependencyIdToModIds.Keys.ToList();
    }

    private List<DependencyModel> LoadDependenciesFromDisk()
    {
        string dependenciesPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Modkist",
            "dependencies.json");

        if (!File.Exists(dependenciesPath))
        {
            return JsonConvert.DeserializeObject<List<DependencyModel>>(PreloadedDependencies.Value)!;
        }

        string json = File.ReadAllText(dependenciesPath);
        return JsonConvert.DeserializeObject<List<DependencyModel>>(json)!;
    }

    private void SaveDependenciesToDisk()
    {
        string dependenciesPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Modkist",
            "dependencies.json");

        List<DependencyModel> dependencyModels = dependencyIdToModIds
            .Select(x => new DependencyModel(x.Key, modCachingService[x.Key].DateUpdated, x.Value)).ToList();
        string json = JsonConvert.SerializeObject(dependencyModels, Formatting.Indented);
        File.WriteAllText(dependenciesPath, json);
    }
}
