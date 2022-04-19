using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearestOperator
{
    internal class positionCord
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public positionCord(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
