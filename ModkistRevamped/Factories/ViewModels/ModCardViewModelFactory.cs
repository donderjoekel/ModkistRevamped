using TNRD.Modkist.ViewModels.Controls;

namespace TNRD.Modkist.Factories.ViewModels;

public class ModCardViewModelFactory : FactoryBase<ModListCardItemViewModel>
{
    public ModCardViewModelFactory(IServiceProvider provider)
        : base(provider)
    {
    }
}
