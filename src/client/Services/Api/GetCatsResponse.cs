using System.Collections.Generic;

namespace EndlessCatsApp.Services.Api
{
    public class GetCatsResponse
    {
        public IEnumerable<Cat> Results { get; set; } 
    }
}
