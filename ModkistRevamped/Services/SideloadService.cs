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

    public List<SideloadedModModel> GetSideloadedMods()
    {
        string sideloadedDirectory =
            Path.Combine(settingsService.ZeepkistDirectory, "BepInEx", "plugins", "Sideloaded");
        if (!Directory.Exists(sideloadedDirectory))
            Directory.CreateDirectory(sideloadedDirectory);

        List<string> paths = new();
        paths.AddRange(Directory.GetDirectories(sideloadedDirectory, "*", SearchOption.TopDirectoryOnly));
        paths.AddRange(Directory.GetFiles(sideloadedDirectory, "*.dll", SearchOption.TopDirectoryOnly));

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

    public void SideloadMod(string path)
    {
        if (string.Equals(Path.GetExtension(path), ".dll", StringComparison.InvariantCultureIgnoreCase))
        {
            File.Copy(path,
                Path.Combine(settingsService.ZeepkistDirectory,
                    "BepInEx",
                    "plugins",
                    "Sideloaded",
                    Path.GetFileName(path)),
                true);

            ModSideloaded?.Invoke();
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
