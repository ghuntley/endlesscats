using ServiceStack;

namespace EndlessCatsApi.ServiceModel
{
    [Route("/favourite/{Identifier}/add")]
    public class AddFavourite : IReturn<AddFavouriteResponse>
    {
    }

    public class AddFavouriteResponse
    {
    }
}