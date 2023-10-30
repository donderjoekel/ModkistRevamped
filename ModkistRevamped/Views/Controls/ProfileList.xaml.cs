using System.Windows.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.ViewModels.Controls;

namespace TNRD.Modkist.Views.Controls;

public partial class ProfileList : UserControl
{
    public ProfileList()
    {
        ViewModel = App.GetService<ProfileListViewModelFactory>().Create();
        DataContext = this;

        InitializeComponent();
    }

    public ProfileListViewModel ViewModel { get; set; }
}
