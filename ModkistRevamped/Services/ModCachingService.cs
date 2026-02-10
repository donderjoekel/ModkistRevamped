using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Modio;
using Modio.Models;

namespace TNRD.Modkist.Services;

public class ModCachingService : IEnumerable<Mod>
{
    private readonly ModsClient modsClient;
    private readonly UserClient userClient;
    private readonly SettingsService settingsService;
    private readonly Dictionary<uint, Mod> idToMod = new();

    public ModCachingService(ModsClient modsClient, UserClient userClient, SettingsService settingsService)
    {
        this.modsClient = modsClient;
        this.userClient = userClient;
        this.settingsService = settingsService;
    }

    public async Task Initialize()
    {
        idToMod.Clear();

        List<Task<IReadOnlyList<Mod>>> tasks = new()
        {
            modsClient.Search().ToList()
        };
        
        if (settingsService.AccessToken != null)
        {
            tasks.Add(userClient.GetSubscriptions().ToList());
            tasks.Add(userClient.GetMods().ToList());
        }

        var results = await Task.WhenAll(tasks);

        IReadOnlyList<Mod> mods = results.SelectMany(list => list)
                                        .DistinctBy(mod => mod.Id)
                                        .Where(x => x.GameId == 3213)
                                        .ToList();

        foreach (Mod mod in mods)
        {
            idToMod.Add(mod.Id, mod);
        }
    }

    public bool TryGetMod(uint modId, [NotNullWhen(true)] out Mod? mod)
    {
        return idToMod.TryGetValue(modId, out mod);
    }

    public IEnumerator<Mod> GetEnumerator()
    {
        return idToMod.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
