using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Modio;
using Modio.Models;

namespace TNRD.Modkist.Services;

public class ModCachingService : IEnumerable<Mod>
{
    private readonly ModsClient modsClient;
    private readonly UserClient userClient;
    private readonly Dictionary<uint, Mod> idToMod = new();

    public ModCachingService(ModsClient modsClient, UserClient userClient)
    {
        this.modsClient = modsClient;
        this.userClient = userClient;
    }

    public async Task Initialize()
    {
        idToMod.Clear();

        var results = await Task.WhenAll(modsClient.Search().ToList(), userClient.GetSubscriptions().ToList(), userClient.GetMods().ToList());
        IReadOnlyList<Mod> mods = results.SelectMany(list => list)
                                        .DistinctBy(mod => mod.Id)
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
