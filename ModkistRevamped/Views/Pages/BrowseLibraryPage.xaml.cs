using System.Windows.Controls;
using TNRD.Modkist.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Views.Pages;

public partial class BrowseLibraryPage : Page, INavigableView<NavigationDummyViewModel>
{
    public BrowseLibraryPage()
    {
        ViewModel = new NavigationDummyViewModel();
        DataContext = this;

        InitializeComponent();

        ViewModel.NavigatedTo += ViewModelOnNavigatedTo;
    }

    public NavigationDummyViewModel ViewModel { get; set; }

    private void ViewModelOnNavigatedTo()
    {
        ModList.ViewModel.OnNavigatedTo();
    }
}
