using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndlessCatsApp.Services.ServiceModel
{
    public class GetCatsResponse
    {
        public IEnumerable<Cat> Results { get; set; } 
    }
}
