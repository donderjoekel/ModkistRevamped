using System.Windows.Controls;
using TNRD.Modkist.Services;
using TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

namespace TNRD.Modkist.Views.Controls.Details.Sidebar;

public partial class ModTags : UserControl
{
    public ModTags()
    {
        ViewModel = new ModTagsViewModel(App.GetService<SelectedModService>());
        DataContext = this;

        InitializeComponent();
    }

    public ModTagsViewModel ViewModel { get; set; }
}
