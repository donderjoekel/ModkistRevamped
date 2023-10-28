using System.Windows.Controls;
using TNRD.Modkist.Services;
using TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

namespace TNRD.Modkist.Views.Controls.Details.Sidebar;

public partial class ModStatistics : UserControl
{
    public ModStatistics()
    {
        ViewModel = new ModStatisticsViewModel(App.GetService<SelectedModService>());
        DataContext = this;

        InitializeComponent();
    }

    public ModStatisticsViewModel ViewModel { get; set; }
}
