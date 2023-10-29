using System.Windows.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

namespace TNRD.Modkist.Views.Controls.Details.Sidebar;

public partial class ModStatistics : UserControl
{
    public ModStatistics()
    {
        ViewModel = App.GetService<ModStatisticsViewModelFactory>().Create();
        DataContext = this;

        InitializeComponent();
    }

    public ModStatisticsViewModel ViewModel { get; set; }
}
