using EndlessCatsApi.Models;
using EndlessCatsApi.ServiceModel;
using EndlessCatsApi.ServiceModel.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EndlessCatsApi.ServiceInterface
{
    public interface ITheCatApiService
    {
        Task<IReadOnlyList<Cat>> GetCats();

        Task<IReadOnlyList<Cats>> GetFavouriteCats();

        Task AddCatToFavourites(string identifier);

        Task RemoveCatFromFavourites(string identifier);

        Task CastVote(string identifier, int score);
    }

    public class TheCatApiService : ITheCatApiService
    {
        private readonly string _apiKey;

        public TheCatApiService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<IReadOnlyList<Cat>> GetCats()
        {
            var uri =
                string.Format(
                    "http://thecatapi.com/api/images/get?format=xml&results_per_page=50&type=jpg&size=med&api_key={0}",
                    _apiKey);

            var xmlString = await GetAsync(uri).ConfigureAwait(false);

            var serializer = new XmlSerializer(typeof (GetCatsXmlResponse));
            using (var reader = new StringReader(xmlString))
            {
                var xmlData = (GetCatsXmlResponse) serializer.Deserialize(reader);

                var query = from cat in xmlData.data.images
                    select new Cat
                    {
                        Identifier = cat.id,
                        Url = new Uri(cat.url),
                        SourceUrl = new Uri(cat.source_url)
                    };

                return query.ToList().AsReadOnly();
            }
        }

        public Task<IReadOnlyList<Cats>> GetFavouriteCats()
        {
            throw new NotImplementedException();
        }

        public Task AddCatToFavourites(string identifier)
        {
            throw new NotImplementedException();
        }

        public Task RemoveCatFromFavourites(string identifier)
        {
            throw new NotImplementedException();
        }

        public Task CastVote(string identifier, int score)
        {
            throw new NotImplementedException();
        }

        private async Task<string> GetAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url).ConfigureAwait(false);

                // throw exception if not successfull HTTP error code.
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
    }
}