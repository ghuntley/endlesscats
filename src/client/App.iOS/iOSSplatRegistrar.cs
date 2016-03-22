﻿using System;
using EndlessCatsApp.iOS.Views;
using EndlessCatsApp.ViewModels;
using ReactiveUI;
using Splat;
namespace EndlessCatsApp.iOS
{
    public sealed class iOSSplatRegistrar : SplatRegistrar
    {

        protected override void RegisterPlatformComponents(IMutableDependencyResolver splatLocator, CompositionRoot compositionRoot)
        {
            splatLocator.Register(() => compositionRoot.ResolveLoggingService(), typeof(ILogger));
        }

        protected override void RegisterViews(IMutableDependencyResolver splatLocator)
        {
        }
    }
}

