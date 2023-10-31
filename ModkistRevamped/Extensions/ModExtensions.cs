using Modio.Models;

namespace TNRD.Modkist.Extensions;

public static class ModExtensions
{
    public static bool IsPlugin(this Mod mod)
    {
        return mod.Tags.Any(x => string.Equals(x.Name, "Plugin", StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsBlueprint(this Mod mod)
    {
        return mod.Tags.Any(x => string.Equals(x.Name, "Blueprint", StringComparison.OrdinalIgnoreCase));
    }

    public static bool MatchesTag(this Mod mod, string tag, bool returnWhenEmpty = true)
    {
        return string.IsNullOrEmpty(tag)
            ? returnWhenEmpty
            : mod.Tags.Any(x => string.Equals(x.Name, tag, StringComparison.OrdinalIgnoreCase));
    }
}
