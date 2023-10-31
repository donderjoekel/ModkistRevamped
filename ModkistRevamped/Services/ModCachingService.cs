using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Modio;
using Modio.Models;

namespace TNRD.Modkist.Services;

public class ModCachingService : IEnumerable<Mod>
{
    private readonly ModsClient modsClient;
    private readonly Dictionary<uint, Mod> idToMod = new();

    public Mod this[uint modId] => idToMod[modId];

    public ModCachingService(ModsClient modsClient)
    {
        this.modsClient = modsClient;
    }

    public async Task Initialize()
    {
        idToMod.Clear();

        IReadOnlyList<Mod> mods = await modsClient.Search().ToList();

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
