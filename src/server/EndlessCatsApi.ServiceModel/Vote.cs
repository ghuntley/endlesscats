using ServiceStack;

namespace EndlessCatsApi.ServiceModel
{
    [Route("/vote/{Identifier}")]
    public class Vote : IReturn<VoteResponse>
    {
        public string Identifier { get; set; }
        public int Score { get; set; } // 1 = bad || 10 = good
    }

    public class VoteResponse
    {
    }
}