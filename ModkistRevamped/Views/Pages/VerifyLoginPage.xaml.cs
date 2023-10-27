using System.Windows.Controls;
using TNRD.Modkist.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Views.Pages;

public partial class VerifyLoginPage : Page, INavigableView<VerifyLoginViewModel>
{
    public VerifyLoginPage(VerifyLoginViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public VerifyLoginViewModel ViewModel { get; set; }
}
