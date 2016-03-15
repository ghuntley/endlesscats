using EndlessCatsApi.ServiceInterface;
using EndlessCatsApi.ServiceModel;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using System.Linq;

namespace EndlessCatsApi.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        private readonly ServiceStackHost _appHost;

        public IntegrationTests()
        {
            _appHost = new BasicAppHost(typeof (WebServices).Assembly)
            {
                ConfigureContainer =
                    container => { container.Register<ITheCatApiService>(c => new TheCatApiService("dummyKey")); }
            }
                .Init();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _appHost.Dispose();
        }

        [Test]
        public async void GetCatsReturnsAtleast50Cats()
        {
            var service = _appHost.Container.Resolve<WebServices>();

            var response = await service.Get(new Cats());

            Assert.That(response.Results.Count(), Is.GreaterThanOrEqualTo(50));
            Assert.That(response.Results.ElementAt(0).Identifier, Is.Not.Null);
            Assert.That(response.Results.ElementAt(0).Url, Is.Not.Null);
            Assert.That(response.Results.ElementAt(0).SourceUrl, Is.Not.Null);
        }
    }
}