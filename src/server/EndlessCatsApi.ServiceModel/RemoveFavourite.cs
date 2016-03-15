using ServiceStack;

namespace EndlessCatsApi.ServiceModel
{
    [Route("/favourite/{Identifier}/remove")]
    public class RemoveFavourite : IReturn<RemoveFavouriteResponse>
    {
        public string Identifier { get; set; }
    }

    public class RemoveFavouriteResponse
    {
    }
}