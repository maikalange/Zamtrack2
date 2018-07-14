using System;

namespace Info.Zainco.ZamTrack.PersistenceManager
{
    public struct Device
    {
        public string Id { get; set; }
        public Decimal Latitude { get; set; }
        public Decimal Longitude { get; set; }
        public string EventId { get; set; }
        public Decimal Direction { get; set; }
        public DateTime GpsTimeStamp { get; set; }
        public Boolean IsValidGps { get; set; }
        public decimal Speed { get; set; }
        public string Driver { get; set; }
        public string EventDescription { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public string Reg { get; set; }
    }

}
