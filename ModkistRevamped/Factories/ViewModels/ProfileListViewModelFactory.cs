using TNRD.Modkist.ViewModels.Controls;

namespace TNRD.Modkist.Factories.ViewModels;

public class ProfileListViewModelFactory : FactoryBase<ProfileListViewModel>
{
    public ProfileListViewModelFactory(IServiceProvider provider)
        : base(provider)
    {
    }
}
