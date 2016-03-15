using System;

namespace EndlessCatsApi.ServiceModel.Types
{
    public class Cat
    {
        public string Identifier { get; set; }
        public Uri Url { get; set; }
        public Uri SourceUrl { get; set; }
    }
}