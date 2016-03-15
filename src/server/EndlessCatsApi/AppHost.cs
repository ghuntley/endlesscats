using EndlessCatsApi.ServiceInterface;
using Funq;
using ServiceStack;
using ServiceStack.Configuration;

namespace EndlessCatsApi
{
    public class AppHost : AppHostBase
    {
        private readonly IAppSettings _appSettings;

        public AppHost() : base("EndlessCatsApi", typeof (WebServices).Assembly)
        {
            _appSettings = new AppSettings();
        }

        public override void Configure(Container container)
        {
            var apiKey = _appSettings.Get<string>("ApiKey");

            container.Register<ITheCatApiService>(c => new TheCatApiService(apiKey));

            Plugins.Add(new PostmanFeature());
            Plugins.Add(new CorsFeature());
        }
    }
}