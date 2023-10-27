using System.IO;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using Microsoft.Win32;

namespace TNRD.Modkist.Services;

public class SteamService
{
    public string? GetSteamPath()
    {
        return Get32BitSteam() ?? Get64BitSteam();
    }

    private string? Get32BitSteam()
    {
        try
        {
            return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam",
                "InstallPath",
                null) as string;
        }
        catch
        {
            return null;
        }
    }

    private string? Get64BitSteam()
    {
        try
        {
            return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam",
                "InstallPath",
                null) as string;
        }
        catch
        {
            return null;
        }
    }

    public bool TryGetGameFolder(string steamFolder, out string? gameFolder)
    {
        try
        {
            gameFolder = null;
            string libraryFoldersPath = Path.Combine(steamFolder, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(libraryFoldersPath))
                return false;

            string contents = File.ReadAllText(libraryFoldersPath);
            VProperty root = VdfConvert.Deserialize(contents);

            foreach (VToken token in root.Value)
            {
                if (!TryGetPathFromToken(token, out string? steamGamesFolder))
                    continue;

                string manifestPath = Path.Combine(steamGamesFolder!, "steamapps", "appmanifest_1440670.acf");
                string zeepkistPath = Path.Combine(steamGamesFolder!, "steamapps", "common", "Zeepkist");

                if (File.Exists(manifestPath) && Directory.Exists(zeepkistPath))
                {
                    gameFolder = zeepkistPath;
                    return true;
                }
            }

            return false;
        }
        catch (Exception)
        {
            gameFolder = null;
            return false;
        }
    }

    private bool TryGetPathFromToken(VToken token, out string? path)
    {
        path = null;
        if (token.Type != VTokenType.Property)
            return false;

        VProperty property = token.Value<VProperty>();
        if (property.Value.Type != VTokenType.Object)
            return false;

        VObject vObject = property.Value.Value<VObject>();
        if (!vObject.TryGetValue("path", out VToken? pathToken))
            return false;

        if (pathToken.Type != VTokenType.Value)
            return false;

        VValue value = pathToken.Value<VValue>();
        path = value.Value?.ToString();

        return true;
    }
}
