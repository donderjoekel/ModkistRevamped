using System.IO;
using Ionic.Zip;
using Modio.Models;
using TNRD.Modkist.Extensions;
using TNRD.Modkist.Models;
using File = System.IO.File;

namespace TNRD.Modkist.Services;

public class InstallationService
{
    private readonly SettingsService settingsService;

    public InstallationService(SettingsService settingsService)
    {
        this.settingsService = settingsService;
    }

    public List<InstalledModModel> GetInstalledMods()
    {
        List<InstalledModModel> installedMods = new();

        installedMods.AddRange(GetInstalledMods(Path.Combine(settingsService.ZeepkistDirectory,
            "BepInEx",
            "plugins",
            "Mods")));

        installedMods.AddRange(GetInstalledMods(Path.Combine(settingsService.ZeepkistDirectory,
            "BepInEx",
            "plugins",
            "Blueprints")));

        return installedMods;
    }

    private IEnumerable<InstalledModModel> GetInstalledMods(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string[] directories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
        foreach (string directory in directories)
        {
            string? directoryName = Path.GetFileName(directory);
            if (string.IsNullOrEmpty(directoryName))
                continue;

            string[] parts = directoryName.Split('_');
            if (parts.Length != 2)
                continue;

            if (!uint.TryParse(parts[0], out uint modId) || !uint.TryParse(parts[1], out uint version))
                continue; // Log this?

            yield return new InstalledModModel()
            {
                ModId = modId,
                ModVersion = version
            };
        }
    }

    public void InstallMod(Mod mod, string path)
    {
        string destinationFolder;

        if (mod.IsPlugin())
        {
            destinationFolder = Path.Combine(settingsService.ZeepkistDirectory,
                "BepInEx",
                "plugins",
                "Mods",
                mod.Id + "_" + mod.Modfile!.Id);
        }
        else if (mod.IsBlueprint())
        {
            destinationFolder = Path.Combine(settingsService.ZeepkistDirectory,
                "BepInEx",
                "plugins",
                "Blueprints",
                mod.Id + "_" + mod.Modfile!.Id);
        }
        else
        {
            throw new InvalidOperationException();
        }

        Directory.CreateDirectory(destinationFolder);

        using (FileStream stream = File.OpenRead(path))
        {
            using (ZipFile? zip = ZipFile.Read(stream))
            {
                zip.ExtractAll(destinationFolder);
            }
        }
    }

    public void UninstallMod(Mod mod)
    {
        string path;

        if (mod.IsPlugin())
        {
            path = Path.Combine(settingsService.ZeepkistDirectory,
                "BepInEx",
                "plugins",
                "Mods");
        }
        else if (mod.IsBlueprint())
        {
            path = Path.Combine(settingsService.ZeepkistDirectory,
                "BepInEx",
                "plugins",
                "Blueprints");
        }
        else
        {
            throw new InvalidOperationException();
        }

        string[] directories = Directory.GetDirectories(path, mod.Id + "_*", SearchOption.TopDirectoryOnly);

        foreach (string directory in directories)
        {
            Directory.Delete(directory, true);
        }
    }

    public void UninstallUnknownMod(uint modId)
    {
        string path = Path.Combine(settingsService.ZeepkistDirectory,
            "BepInEx",
            "plugins");

        string[] directories = Directory.GetDirectories(path, modId + "_*", SearchOption.AllDirectories);

        foreach (string directory in directories)
        {
            Directory.Delete(directory, true);
        }
    }

    public bool IsInstalled(Mod mod)
    {
        return mod.Modfile != null && GetInstalledMods().Any(x => x.ModId == mod.Id && x.ModVersion == mod.Modfile.Id);
    }
}
