using EndlessCatsApi.ServiceModel;
using ServiceStack;
using System.Threading.Tasks;

namespace EndlessCatsApi.ServiceInterface
{
    public class WebServices : Service
    {
        private readonly ITheCatApiService _theCatApiService;

        public WebServices(ITheCatApiService theCatApiService)
        {
            _theCatApiService = theCatApiService;
        }

        public async Task<AddFavouriteResponse> Put(AddFavourite request)
        {
            return new AddFavouriteResponse();
        }

        public async Task<RemoveFavouriteResponse> Put(RemoveFavourite request)
        {
            return new RemoveFavouriteResponse();
        }

        public async Task<FavouritesResponse> Get(Favourites request)
        {
            return new FavouritesResponse();
        }

        public async Task<VoteResponse> Put(Vote request)
        {
            return new VoteResponse();
        }

        public async Task<CatsResponse> Get(Cats request)
        {
            var cats = await _theCatApiService.GetCats();

            return new CatsResponse {Results = cats};
        }
    }
}