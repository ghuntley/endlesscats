using System.Collections.Generic;
using System.Xml.Serialization;

namespace EndlessCatsApi.Models
{
    // http://thecatapi.com/api/images/get?format=xml&results_per_page=50&type=jpg&size=full

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", ElementName = "response", IsNullable = false)]
    public class GetCatsXmlResponse
    {
        public responseData data { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class responseData
    {
        [XmlArrayItem("image", IsNullable = false)]
        public List<responseDataImage> images { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class responseDataImage
    {
        public string url { get; set; }

        public string id { get; set; }

        public string source_url { get; set; }
    }
}