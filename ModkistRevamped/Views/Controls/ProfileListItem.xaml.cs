using System.Windows.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.Models;
using TNRD.Modkist.ViewModels.Controls;

namespace TNRD.Modkist.Views.Controls;

public partial class ProfileListItem : UserControl
{
    public ProfileListItem(ProfileModel profile)
    {
        ViewModel = App.GetService<ProfileListItemViewModelFactory>().Create(profile);
        DataContext = this;

        InitializeComponent();
    }

    public ProfileListItemViewModel ViewModel { get; set; }
}
