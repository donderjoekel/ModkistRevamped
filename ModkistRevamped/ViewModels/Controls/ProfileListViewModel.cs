using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using TNRD.Modkist.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Views.Controls;
using TNRD.Modkist.Views.Windows;

namespace TNRD.Modkist.ViewModels.Controls;

public partial class ProfileListViewModel : ObservableObject
{
    private readonly ProfileService profileService;

    public ProfileListViewModel(ProfileService profileService)
    {
        this.profileService = profileService;
        this.profileService.ProfileDeleted += OnProfileDeleted;
        this.profileService.ProfileSelected += OnProfileSelected;

        collectionView = CollectionViewSource.GetDefaultView(collection);

        LoadProfiles();
    }

    [ObservableProperty] private ObservableCollection<FrameworkElement> collection = new();
    [ObservableProperty] private ICollectionView collectionView;

    private void OnProfileDeleted()
    {
        LoadProfiles();
    }

    private void OnProfileSelected(ProfileModel profile)
    {
        Process.Start(Environment.ProcessPath!);
        Application.Current.Shutdown();
    }

    private void LoadProfiles()
    {
        Collection.Clear();
        List<ProfileModel> profiles = profileService.GetProfiles();
        for (int i = 0; i < profiles.Count; i++)
        {
            ProfileModel profile = profiles[i];

            if (i > 0)
            {
                Collection.Add(new Separator());
            }

            ProfileListItem profileListItem = new(profile);
            Collection.Add(profileListItem);
        }
    }

    [RelayCommand]
    private void AddProfile()
    {
        InputBox inputBox = new("Enter a name for the profile", true, "Profile name");
        bool? result = inputBox.ShowDialog();
        if (!result.HasValue || !result.Value)
            return;

        profileService.AddProfile(inputBox.ViewModel.Input!);
        LoadProfiles();
    }
}
