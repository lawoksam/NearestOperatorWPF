using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearestOperator
{
    internal class positionCord
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public string Locality { get; set; }
        public string Country { get; set; }

        public positionCord(string latitude, string longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public positionCord(string latitude, string longitude, string name, string postalCode, string region, string locality, string country) : this(latitude, longitude)
        {
            Name = name;
            PostalCode = postalCode;
            Region = region;
            Locality = locality;
            Country = country;
        }
    }
}
