using System.Windows.Controls;
using Modio.Models;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.Services;
using TNRD.Modkist.ViewModels.Controls;
using Wpf.Ui;

namespace TNRD.Modkist.Views.Controls;

public partial class ModCard : UserControl
{
    public ModCard(Mod mod)
    {
        ViewModel = App.GetService<ModCardViewModelFactory>().Create(mod);
        DataContext = this;

        InitializeComponent();
    }

    public ModCardViewModel ViewModel { get; set; }

    public bool IsValidForFilter(string filter)
    {
        return ViewModel.Title.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }

    public string SortTitle => ViewModel.Mod.Name!;
    public uint SortTotalDownloads => ViewModel.Mod.Stats!.TotalDownloads;
    public uint SortTotalSubscribers => ViewModel.Mod.Stats!.TotalSubscribers;
    public long SortDateAdded => ViewModel.Mod.DateAdded;
    public long SortDateUpdated => ViewModel.Mod.DateUpdated;
    public uint SortPopularityRank => ViewModel.Mod.Stats!.PopularityRank;
}
