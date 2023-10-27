using System.Windows.Controls;
using TNRD.Modkist.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Views.Pages;

public partial class InitializationPage : Page, INavigableView<InitializationViewModel>
{
    public InitializationPage(InitializationViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public InitializationViewModel ViewModel { get; set; }
}
