using System.Windows.Media.Imaging;
using Modio.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui;
using Visibility = System.Windows.Visibility;

namespace TNRD.Modkist.ViewModels.Controls;

public partial class ModCardViewModel : ObservableObject
{
    private readonly ImageCachingService imageCachingService;
    private readonly INavigationService navigationService;
    private readonly SelectedModService selectedModService;
    private readonly Mod mod;

    public ModCardViewModel(
        ImageCachingService imageCachingService,
        INavigationService navigationService,
        SelectedModService selectedModService,
        Mod mod
    )
    {
        this.imageCachingService = imageCachingService;
        this.navigationService = navigationService;
        this.selectedModService = selectedModService;
        this.mod = mod;

        Title = this.mod.Name!;
        LoadImage();
    }

    [ObservableProperty] private BitmapImage? imageSource;
    [ObservableProperty] private string title;
    [ObservableProperty] private Visibility progressVisibility = Visibility.Visible;

    public Mod Mod => mod;

    private async void LoadImage()
    {
        ProgressVisibility = Visibility.Visible;
        string imagePath = await imageCachingService.GetImagePath(mod.Logo!.Thumb320x180!);
        ImageSource = new BitmapImage(new Uri(imagePath));
        ProgressVisibility = Visibility.Collapsed;
    }

    [RelayCommand]
    private void Clicked()
    {
        selectedModService.SetSelectedMod(mod, false);
        navigationService.NavigateWithHierarchy(typeof(ModDetailsPage));
    }
}
