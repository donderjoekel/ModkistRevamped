using System.IO;
using Ionic.Zip;
using Microsoft.VisualBasic;
using TNRD.Modkist.Models;

namespace TNRD.Modkist.Services;

public class SideloadService
{
    public delegate void ModSideloadedDelegate();

    public delegate void SideloadedModRemovedDelegate();

    private readonly SettingsService settingsService;

    public SideloadService(SettingsService settingsService)
    {
        this.settingsService = settingsService;
    }

    public event ModSideloadedDelegate? ModSideloaded;
    public event SideloadedModRemovedDelegate? SideloadedModRemoved;

    public List<SideloadedModModel> GetSideloadedPlugins()
    {
        return GetSideloadedMods(ModType.Plugin);
    }

    public List<SideloadedModModel> GetSideloadedBlueprints()
    {
        return GetSideloadedMods(ModType.Blueprint);
    }

    private List<SideloadedModModel> GetSideloadedMods(ModType modType)
    {
        string sideloadedDirectory =
            Path.Combine(settingsService.ZeepkistDirectory,
                "BepInEx",
                "plugins",
                "Sideloaded",
                modType == ModType.Plugin ? "Plugins" : "Blueprints");
        if (!Directory.Exists(sideloadedDirectory))
            Directory.CreateDirectory(sideloadedDirectory);

        List<string> paths = new();
        paths.AddRange(Directory.GetDirectories(sideloadedDirectory, "*", SearchOption.TopDirectoryOnly));

        switch (modType)
        {
            case ModType.Plugin:
                paths.AddRange(Directory.GetFiles(sideloadedDirectory, "*.dll", SearchOption.TopDirectoryOnly));
                break;
            case ModType.Blueprint:
                paths.AddRange(Directory.GetFiles(sideloadedDirectory, "*.zeeplevel", SearchOption.TopDirectoryOnly));
                break;
            case ModType.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(modType), modType, null);
        }

        List<SideloadedModModel> sideloadedMods = new();

        foreach (string path in paths)
        {
            sideloadedMods.Add(new SideloadedModModel()
            {
                Path = path,
                Filename = Path.GetFileName(path)
            });
        }

        return sideloadedMods;
    }

    public void SideloadPlugin(string path)
    {
        SideloadMod(ModType.Plugin, path);
    }

    public void SideloadBlueprint(string path)
    {
        SideloadMod(ModType.Blueprint, path);
    }

    private void SideloadMod(ModType modType, string path)
    {
        if (modType == ModType.Plugin)
        {
            if (Sideload(path, ".dll", "Plugins"))
                return;
        }

        if (modType == ModType.Blueprint)
        {
            if (Sideload(path, ".zeeplevel", "Blueprints"))
                return;
        }

        try
        {
            using (ZipFile? zip = ZipFile.Read(path))
            {
                if (zip.Entries.Count == 0)
                    return;

                zip.ExtractAll(Path.Combine(settingsService.ZeepkistDirectory,
                        "BepInEx",
                        "plugins",
                        "Sideloaded",
                        modType == ModType.Plugin ? "Plugins" : "Blueprints",
                        Path.GetFileNameWithoutExtension(path)),
                    ExtractExistingFileAction.OverwriteSilently);

                ModSideloaded?.Invoke();
            }
        }
        catch (Exception e)
        {
            // TODO: Log error
        }
    }

    private bool Sideload(string path, string ext, string dir)
    {
        if (!string.Equals(Path.GetExtension(path), ext, StringComparison.InvariantCultureIgnoreCase))
            return false;

        File.Copy(path,
            Path.Combine(settingsService.ZeepkistDirectory,
                "BepInEx",
                "plugins",
                "Sideloaded",
                dir,
                Path.GetFileName(path)),
            true);

        ModSideloaded?.Invoke();
        return true;
    }

    public void Remove(SideloadedModModel sideloadedMod)
    {
        FileAttributes fileAttributes = File.GetAttributes(sideloadedMod.Path);

        if (fileAttributes.HasFlag(FileAttributes.Directory))
        {
            Directory.Delete(sideloadedMod.Path, true);
        }
        else
        {
            File.Delete(sideloadedMod.Path);
        }

        SideloadedModRemoved?.Invoke();
    }
}
