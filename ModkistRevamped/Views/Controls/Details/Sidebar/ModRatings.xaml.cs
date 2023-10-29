using System.Windows.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

namespace TNRD.Modkist.Views.Controls.Details.Sidebar;

public partial class ModRatings : UserControl
{
    public ModRatings()
    {
        ViewModel = App.GetService<ModRatingsViewModelFactory>().Create();
        DataContext = this;

        InitializeComponent();
    }

    public ModRatingsViewModel ViewModel { get; set; }
}
