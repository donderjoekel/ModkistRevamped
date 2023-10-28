using System.Windows.Controls;
using TNRD.Modkist.Services;
using TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

namespace TNRD.Modkist.Views.Controls.Details.Sidebar;

public partial class ModRatings : UserControl
{
    public ModRatings()
    {
        ViewModel = new ModRatingsViewModel(App.GetService<SelectedModService>());
        DataContext = this;

        InitializeComponent();
    }

    public ModRatingsViewModel ViewModel { get; set; }
}
