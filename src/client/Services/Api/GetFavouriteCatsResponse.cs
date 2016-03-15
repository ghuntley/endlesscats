using System.Collections.Generic;

namespace EndlessCatsApp.Services.Api
{
    public class GetFavouriteCatsResponse
    {
        public IEnumerable<Cat> Results { get; set; }

    }
}
