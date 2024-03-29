﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearestOperator
{
    internal class stationInfo
    {
        public int Number { get; set; }
        public string Id { get; set; }
        public string Network { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public double Distance { get; set; }

        public stationInfo(int number, string id, string network, string city, string address, string longitude, string latitude, double distance)
        {
            Number = number;
            Id = id;
            Network = network;
            City = city;
            Address = address;
            Longitude = longitude;
            Latitude = latitude;
            Distance = distance;
        }
    }
}
