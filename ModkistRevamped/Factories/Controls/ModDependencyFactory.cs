using Microsoft.Extensions.DependencyInjection;
using Modio.Models;
using TNRD.Modkist.Views.Controls.Details.Dependencies;
using TNRD.Modkist.Views.Controls.List;

namespace TNRD.Modkist.Factories.Controls;

public class ModDependencyFactory : FactoryBase<ModDependency>
{
    public ModDependencyFactory(IServiceProvider provider)
        : base(provider)
    {
    }
}
