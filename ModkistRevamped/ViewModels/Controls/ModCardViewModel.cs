using System.Windows.Media.Imaging;
using Modio.Models;
using TNRD.Modkist.Services;
using Visibility = System.Windows.Visibility;

namespace TNRD.Modkist.ViewModels.Controls;

public partial class ModCardViewModel : ObservableObject
{
    private readonly ImageCachingService imageCachingService;
    private readonly Mod mod;

    public ModCardViewModel(ImageCachingService imageCachingService, Mod mod)
    {
        this.imageCachingService = imageCachingService;
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
    }
}
