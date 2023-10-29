using System.Windows.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

namespace TNRD.Modkist.Views.Controls.Details.Sidebar;

public partial class ModSubscription : UserControl
{
    public ModSubscription()
    {
        ViewModel = App.GetService<ModSubscriptionViewModelFactory>().Create();
        DataContext = this;

        InitializeComponent();
    }

    public ModSubscriptionViewModel ViewModel { get; set; }
}
