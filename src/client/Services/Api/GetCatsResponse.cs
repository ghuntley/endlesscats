using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndlessCatsApp.Services.Api
{
    public class GetCatsResponse
    {
        public IEnumerable<Cat> Results { get; set; }
    }
}
