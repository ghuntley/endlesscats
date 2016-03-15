using EndlessCatsApi.ServiceModel.Types;
using ServiceStack;
using System.Collections.Generic;

namespace EndlessCatsApi.ServiceModel
{
    [Route("/favourites")]
    public class Favourites : IReturn<FavouritesResponse>
    {
    }

    public class FavouritesResponse
    {
        public IEnumerable<Cat> Results { get; set; }
    }
}