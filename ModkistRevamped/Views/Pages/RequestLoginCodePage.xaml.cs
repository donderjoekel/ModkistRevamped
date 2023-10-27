using System.Windows.Controls;
using TNRD.Modkist.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Views.Pages;

public partial class RequestLoginCodePage : Page, INavigableView<RequestLoginCodeViewModel>
{
    public RequestLoginCodePage(RequestLoginCodeViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public RequestLoginCodeViewModel ViewModel { get; set; }
}
