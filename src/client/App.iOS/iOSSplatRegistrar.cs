using System;
using Splat;
namespace EndlessCatsApp.iOS
{
    public sealed class iOSSplatRegistrar : SplatRegistrar
    {
        protected override void RegisterPlatformComponents(IMutableDependencyResolver splatLocator, CompositionRoot compositionRoot)
        {
            splatLocator.Register(() => compositionRoot.ResolveLoggingService(), typeof(ILogger));
        }
    }
}

