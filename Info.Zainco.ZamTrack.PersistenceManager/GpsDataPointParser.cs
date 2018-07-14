using System;
using System.Configuration;
using System.Text;

namespace Info.Zainco.ZamTrack.PersistenceManager
{
    public class GpsDataPointParser
    {
        //%%800704,A,110402201650,N5121.3489W00011.1780,000,230,NA,47000000,531,CFG:Z31,10,1|
        //%%800704,A,110506090808,N5126.0508W00031.6528,000,052,NA,17800000,110
        //%%[ID],[GPS Valid],[Date & Time],[Loc],[Speed],[Dir],[Temp],[Status], 531 [CR][LF]


        private const int GpsUnitId = 0;

        private const int GpsValid = 1;
        private const int DateAndTime = 2;
        private const int DeviceLocation = 3;

        private const int VehicleSpeed = 4;
        private const int VehicleDirection = 5;
        //private const int DeviceTemperature = 6;

        private const int GpsStatus = 7;
        private const int CommandEventId = 8;
        //private const int CommandDescription = 9;
        private static string[] _gpsTokens;
        private readonly string _gpsDataPoint;

        public GpsDataPointParser(string gpsDataPoint)
        {
            _gpsDataPoint = gpsDataPoint;
            _gpsTokens = _gpsDataPoint.Split(new[] { ',' });
        }

        private static string[] GpsTokens
        {
            get { return _gpsTokens; }
        }

        public decimal EventId
        {
            get { return int.Parse(GpsTokens[CommandEventId]); }
        }


        private static decimal ConvertLocationToDecimalDegrees(decimal location)
        {
            var degrees = Math.Truncate(location / 100);
            var position = degrees + 100 * ((location / 100) - degrees) / 60;
            return position;
        }


        public LatLong Position
        {
            get
            {
                var location = GpsTokens[DeviceLocation];
                var cardinalPoints = new LatLong();
                const string west = "W";
                const string east = "E";
                const string south = "S";
                const string north = "N";

                if (location.Contains(east))
                {
                    var points = location.Split(new[] { east }, StringSplitOptions.None);
                    var isNorth = points[0].Contains(north);
                    cardinalPoints.Longitude = decimal.Parse(points[1]); //East
                    cardinalPoints.Longitude = ConvertLocationToDecimalDegrees((decimal)cardinalPoints.Longitude);

                    if (isNorth)
                    {
                        cardinalPoints.Latitude =
                            decimal.Parse(new StringBuilder(points[0].Replace(north, string.Empty)).ToString());
                        cardinalPoints.Latitude = ConvertLocationToDecimalDegrees((decimal)cardinalPoints.Latitude);
                    }
                    else
                    {
                        cardinalPoints.Latitude = -1 *
                                               decimal.Parse(
                                                   new StringBuilder(points[0].Replace(south, string.Empty)).ToString());
                        cardinalPoints.Latitude = ConvertLocationToDecimalDegrees((decimal)cardinalPoints.Latitude);
                    }
                }
                else
                {
                    var points = location.Split(new[] { west }, StringSplitOptions.None);
                    var isNorth = points[0].Contains(north);
                    cardinalPoints.Longitude = -1 * decimal.Parse(points[1]);
                    cardinalPoints.Longitude = ConvertLocationToDecimalDegrees((decimal)cardinalPoints.Longitude);


                    if (isNorth)
                    {
                        cardinalPoints.Latitude =
                            decimal.Parse(new StringBuilder(points[0].Replace(north, string.Empty)).ToString());
                        cardinalPoints.Latitude = ConvertLocationToDecimalDegrees((decimal)cardinalPoints.Latitude);
                    }
                    else
                    {
                        cardinalPoints.Latitude = -1 *
                                               decimal.Parse(
                                                   new StringBuilder(points[0].Replace(south, string.Empty)).ToString());

                        cardinalPoints.Latitude = ConvertLocationToDecimalDegrees((decimal)cardinalPoints.Latitude);
                    }
                }

                return cardinalPoints;
            }
        }

        public long Status
        {
            get { return long.Parse(GpsTokens[GpsStatus]); }
        }

        public decimal Speed
        {
            get { return decimal.Parse(GpsTokens[VehicleSpeed]); }
        }

        public bool IsGpsValid
        {
            get
            {
                return GpsTokens[GpsValid].Contains("A");
            }
        }

        public DateTime GpsTimeStamp
        {
            get
            {
                var timeStamp = DateTime.Now;
                var dateTime = GpsTokens[DateAndTime];
                if (dateTime.Length == 12)
                {
                    var year = dateTime.Substring(0, 2);
                    var month = dateTime.Substring(2, 2);
                    var day = dateTime.Substring(4, 2);

                    var hour = dateTime.Substring(6, 2);
                    var minute = dateTime.Substring(8, 2);
                    var second = dateTime.Substring(10, 2);
                    var gmtOffSet = int.Parse(ConfigurationManager.AppSettings["gmt.offset"]);
                    timeStamp = new DateTime(2000 + int.Parse(year), int.Parse(month), int.Parse(day),
                                             int.Parse(hour + gmtOffSet),
                                             int.Parse(minute), int.Parse(second));
                }

                return timeStamp;
            }
        }

        public string DeviceId
        {
            get { return GpsTokens[GpsUnitId].Replace("%", string.Empty); }
        }

        public double Direction
        {
            get { return double.Parse(GpsTokens[VehicleDirection]); }
        }
    }
}