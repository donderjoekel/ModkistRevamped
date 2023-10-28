using System.Windows.Controls;
using TNRD.Modkist.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Views.Pages;

public partial class BrowsePluginsPage : Page, INavigableView<BrowsePluginsViewModel>
{
    public BrowsePluginsPage(BrowsePluginsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public BrowsePluginsViewModel ViewModel { get; set; }
}
