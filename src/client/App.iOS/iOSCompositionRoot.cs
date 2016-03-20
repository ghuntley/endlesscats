using System;
using EndlessCatsApp.Services.iOS.Logging;
using Splat;

namespace EndlessCatsApp.iOS
{
    public sealed class iOSCompositionRoot : CompositionRoot
    {
        protected override ILogger CreateLoggingService() => new LoggingService();
    }
}

