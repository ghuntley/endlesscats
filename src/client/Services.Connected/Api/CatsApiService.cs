using System;
using System.Net.Http;
using EndlessCatsApp.Services.Api;
using Fusillade;
using ModernHttpClient;
using Refit;

namespace EndlessCatsApp.Services.Connected.Api
{
    public class CatsApiService : ICatsApiService
    {
        public const string ApiBaseAddress = "http://endlesscats.azurewebsites.net";

        private readonly Lazy<ICatsApi> _background;
        private readonly Lazy<ICatsApi> _speculative;
        private readonly Lazy<ICatsApi> _userInitiated;


        public CatsApiService(string apiBaseAddress = null)
        {
            Func<HttpMessageHandler, ICatsApi> createClient = messageHandler =>
            {
                var client = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri(apiBaseAddress ?? ApiBaseAddress)
                };

                return RestService.For<ICatsApi>(client);
            };

            _background = new Lazy<ICatsApi>(() => createClient(
                new RateLimitedHttpMessageHandler(new NativeMessageHandler(), Priority.Background)));

            _userInitiated = new Lazy<ICatsApi>(() => createClient(
                new RateLimitedHttpMessageHandler(new NativeMessageHandler(), Priority.UserInitiated)));

            _speculative = new Lazy<ICatsApi>(() => createClient(
                new RateLimitedHttpMessageHandler(new NativeMessageHandler(), Priority.Speculative)));
        }

        public ICatsApi Background => _background.Value;

        public ICatsApi Speculative => _speculative.Value;
        public ICatsApi UserInitiated => _userInitiated.Value;
    }
}
