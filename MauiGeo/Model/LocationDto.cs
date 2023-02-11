using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiGeo.Model
{
    public class LocationDto
    {
        public double? Accuracy { get; set; }
        public bool? ReducedAccuracy { get; set; }
        public double? VerticalAccuracy { get; set; }
        public double? Altitude { get; set; }
        public double? Course { get; set; }
        public bool? IsFromMockProvider { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
