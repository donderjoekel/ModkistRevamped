using System.Windows.Controls;
using TNRD.Modkist.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Views.Pages;

public partial class VerifyBepInExPage : Page, INavigableView<VerifyBepInExViewModel>
{
    public VerifyBepInExPage(VerifyBepInExViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public VerifyBepInExViewModel ViewModel { get; set; }
}
