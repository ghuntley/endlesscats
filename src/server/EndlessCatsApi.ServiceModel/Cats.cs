using EndlessCatsApi.ServiceModel.Types;
using ServiceStack;
using System.Collections.Generic;

namespace EndlessCatsApi.ServiceModel
{
    [Route("/cats")]
    public class Cats : IReturn<CatsResponse>
    {
    }

    public class CatsResponse
    {
        public IEnumerable<Cat> Results { get; set; }
    }
}