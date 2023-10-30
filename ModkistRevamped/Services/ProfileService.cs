using System.IO;
using Newtonsoft.Json;
using TNRD.Modkist.Models;

namespace TNRD.Modkist.Services;

public class ProfileService
{
    public delegate void ProfileDeletedDelegate();

    public delegate void ProfileSelectedDelegate(ProfileModel profile);

    private readonly SettingsService settingsService;

    public ProfileService(SettingsService settingsService)
    {
        this.settingsService = settingsService;

        SetActiveProfile();
    }

    private ProfileModel selectedProfile = null!;

    public event ProfileDeletedDelegate? ProfileDeleted;
    public event ProfileSelectedDelegate? ProfileSelected;

    public string SelectedProfileId => selectedProfile.Id;
    public ProfileType SelectedProfileType => selectedProfile.Type;

    private void SetActiveProfile()
    {
        List<ProfileModel> profiles = GetProfiles();

        ProfileModel? profile = profiles.FirstOrDefault(x => x.Id == settingsService.SelectedProfile);

        if (profile != null)
        {
            selectedProfile = profile;
        }
        else if (settingsService.HasValidAccessToken())
        {
            selectedProfile = profiles.First(x => x.Type == ProfileType.Remote);
        }
        else
        {
            selectedProfile = profiles.First(x => x.Type == ProfileType.Empty);
        }
    }

    private static string GetProfileDirectory()
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Profiles");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    public List<ProfileModel> GetProfiles()
    {
        List<ProfileModel> profiles = new();

        profiles.Add(new ProfileModel()
        {
            Id = "Remote",
            Type = ProfileType.Remote,
            DisplayName = "Mod.io"
        });

        string[] existingProfiles = Directory.GetFiles(GetProfileDirectory(), "*.profile");
        profiles.AddRange(existingProfiles
            .Select(existingProfile => JsonConvert.DeserializeObject<ProfileModel>(File.ReadAllText(existingProfile)))
            .Where(profile => profile != null)!);

        profiles.Add(new ProfileModel()
        {
            Id = "Empty",
            Type = ProfileType.Empty,
            DisplayName = "No Mods"
        });

        return profiles;
    }

    public void AddProfile(string displayName)
    {
        ProfileModel newProfile = new()
        {
            Id = Guid.NewGuid().ToString(),
            Type = ProfileType.Local,
            DisplayName = displayName
        };

        SaveProfile(newProfile);
    }

    public void SaveProfile(ProfileModel profile)
    {
        if (profile.Type != ProfileType.Local)
            return;

        string json = JsonConvert.SerializeObject(profile);
        string path = Path.Combine(GetProfileDirectory(), $"{profile.Id}.profile");
        File.WriteAllText(path, json);
    }

    public void SelectProfile(ProfileModel profileModel)
    {
        if (profileModel == selectedProfile)
            return;

        selectedProfile = profileModel;
        settingsService.SelectedProfile = profileModel.Id;
        ProfileSelected?.Invoke(profileModel);
    }

    public bool IsSelectedProfile(ProfileModel profileModel)
    {
        return Equals(profileModel.Id, selectedProfile?.Id);
    }

    public void DeleteProfile(ProfileModel profileModel)
    {
        if (profileModel.Type != ProfileType.Local)
            return;

        string path = Path.Combine(GetProfileDirectory(), $"{profileModel.Id}.profile");
        File.Delete(path);
        ProfileDeleted?.Invoke();
    }
}
