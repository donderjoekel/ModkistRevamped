using System.Windows.Controls;
using TNRD.Modkist.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Views.Pages;

public partial class ModDetailsPage : Page, INavigableView<ModDetailsViewModel>
{
    public ModDetailsPage(ModDetailsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public ModDetailsViewModel ViewModel { get; set; }
}
