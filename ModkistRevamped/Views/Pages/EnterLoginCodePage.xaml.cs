using System.Windows.Controls;
using TNRD.Modkist.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Views.Pages;

public partial class EnterLoginCodePage : Page, INavigableView<EnterLoginCodeViewModel>
{
    public EnterLoginCodePage(EnterLoginCodeViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public EnterLoginCodeViewModel ViewModel { get; set; }
}
