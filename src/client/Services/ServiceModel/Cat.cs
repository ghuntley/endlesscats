using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndlessCatsApp.Services.ServiceModel
{
    public class Cat
    {
        public string Identifier { get; set; }
        public Uri Url { get; set; }
        public Uri SourceUrl { get; set; }
    }
}
