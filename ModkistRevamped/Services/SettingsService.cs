using System.IO;
using Modio;
using Newtonsoft.Json;
using Wpf.Ui.Appearance;

namespace TNRD.Modkist.Services;

public partial class SettingsService : ObservableObject
{
    [ObservableProperty] private string zeepkistDirectory = string.Empty;
    [ObservableProperty] private AccessToken? accessToken;
    [ObservableProperty] private bool skippedLogin;
    [ObservableProperty] private string selectedProfile = string.Empty;

    public SettingsService()
    {
        if (File.Exists(GetSettingsPath()))
        {
            try
            {
                JsonConvert.PopulateObject(File.ReadAllText(GetSettingsPath()), this);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    private string GetSettingsPath()
    {
        string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Modkist",
            "settings.json");

        string directoryName = Path.GetDirectoryName(settingsPath)!;

        if (!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);

        return settingsPath;
    }

    partial void OnZeepkistDirectoryChanged(string value)
    {
        Save();
    }

    partial void OnAccessTokenChanged(AccessToken? value)
    {
        Save();
    }

    partial void OnSkippedLoginChanged(bool value)
    {
        Save();
    }

    partial void OnSelectedProfileChanged(string value)
    {
        Save();
    }

    private void Save()
    {
        File.WriteAllText(GetSettingsPath(), JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public bool HasValidAccessToken()
    {
        if (AccessToken == null)
            return false;

        if (string.IsNullOrEmpty(AccessToken.Value))
            return false;

        if (!AccessToken.ExpiredAt.HasValue)
            return false;

        return DateTimeOffset.UtcNow < DateTimeOffset.FromUnixTimeSeconds(AccessToken.ExpiredAt!.Value);
    }
}
