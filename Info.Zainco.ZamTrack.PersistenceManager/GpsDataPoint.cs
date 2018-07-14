
using System;

namespace Info.Zainco.ZamTrack.PersistenceManager
{
    public struct GpsDataPoint
    {
        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string Command { get; set; }

        public string DeviceId { get; set; }

        public decimal? Direction { get; set; }

        public int? EventId { get; set; }

        public DateTime GpsTimeStamp { get; set; }

        public bool? IsValidGps { get; set; }

        public int? Status { get; set; }

        public decimal? VehicleSpeed { get; set; }
    }
}