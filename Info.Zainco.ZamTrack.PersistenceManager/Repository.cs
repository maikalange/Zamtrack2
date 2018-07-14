using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Info.Zainco.ZamTrack.PersistenceManager
{
    public class Repository
    {
        private readonly GpsDataPointParser _dataPointParser;
        private readonly string _gpsDataPoint;
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["zamtrack.db.conn"].ConnectionString;
        private Repository(string gpsDataPoint)
        {
            _gpsDataPoint = gpsDataPoint;
            _dataPointParser = new GpsDataPointParser(gpsDataPoint);
        }

        public Repository()
        {
            
        }

        public static IEnumerable<Device> GetDevicesByDateRange(DateTime from, DateTime to)
        {
            return GetAllDevices().Where(x => x.GpsTimeStamp.Date >= from.Date && x.GpsTimeStamp.Date <= to);
        }

        public static IEnumerable<Device> GetDevicesOverSpeedLimit(Decimal speedLimit)
        {            
            return GetAllDevices().Where(x => x.Speed >= speedLimit) ;
        }



        public static string GetDistinctDevicesJson()
        {
            var devices = GetDistinctDevices();
            var distinctDevices = new StringBuilder();
            foreach (var device in devices)
            {
                var sb = new StringBuilder("{DeviceId:$},");
                sb.Replace("$",device.Id);
                distinctDevices.Append(sb);
            }

            return "[" + distinctDevices.Remove(distinctDevices.Length - 1, 1) + "]";
        }

        public static IList<Device> GetDistinctDevices()
        {
            IList<Device> devices = new List<Device>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("GetDistinctDevices"))
                {
                    cmd.Connection = connection;
                    var reader = cmd.ExecuteReader();
                    if (reader != null)
                        while (reader.Read())
                        {
                            var device = new Device
                            {
                                Id = reader["DeviceId"].ToString(),
                            };
                            devices.Add(device);
                        }
                }
            }
            return devices;
        }

        public static IList<Device> GetAllDevices()
        {
            IList<Device> devices  = new List<Device>();
            using (var connection  = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd  = new SqlCommand("GetAllDevices"))
                {
                    cmd.Connection = connection;
                    var reader = cmd.ExecuteReader();
                    if (reader != null)
                        while (reader.Read())
                        {
                            var device = new  Device
                                             {
                                                 Id = reader["DeviceId"].ToString(),
                                                 EventId = reader["EventId"].ToString(),
                                                 Driver = reader["Name"].ToString(),
                                                 Model = reader["Model"].ToString(),
                                                 Make = reader["Make"].ToString(),
                                                 Reg = reader["Registration"].ToString(),
                                                 EventDescription = reader["Description"].ToString(),
                                                 Speed = decimal.Parse(reader["VehicleSpeed"].ToString()),
                                                 GpsTimeStamp = DateTime.Parse(reader["GpsTimeStamp"].ToString()),                                                 
                                                 Latitude = decimal.Parse(reader["Latitude"].ToString()),
                                                 Longitude = decimal.Parse(reader["Longitude"].ToString())
                                             };
                            devices.Add(device);
                        }
                }
            }
            return devices;
        }

        public static string GetAllDevicesJson()
        {
            return EncodeAllDevicesAsJson(GetAllDevices());
        }

        public static string GetAllSpeedingVehicles()
        {
            var allDevices = GetAllDevices().Where(s => s.Speed >= 70).OrderBy(s=>s.Speed);
            return EncodeAllDevicesAsJson(allDevices.ToList());
        }

        public static  string GetAllVehiclesBreachingGeofence()
        {
            var allGeofenceBreaches = GetAllDevices().Where(g => g.EventId == 12.ToString()).ToList();
            return EncodeAllDevicesAsJson(allGeofenceBreaches);
        }

        private static string EncodeAllDevicesAsJson(IList<Device> allDevices)
        {
            var allDeviceJson = new StringBuilder();
            if (allDevices.Count > 0)
            {
                foreach (var device in allDevices)
                {
                    var deviceBuilder =
                        new StringBuilder(
                            "{DV:$device,MD:'$model',MK:'$make',RG:'$reg',LT:$lat,LG:$long,DT:'$date',EI:$evt,SP:$speed,DR:'$driver',EV:'$eventDescription'},");
                    deviceBuilder.Replace("$eventDescription", device.EventDescription);
                    deviceBuilder.Replace("$driver", device.Driver);
                    deviceBuilder.Replace("$make", device.Make);
                    deviceBuilder.Replace("$reg", device.Reg);
                    deviceBuilder.Replace("$model", device.Model);
                    deviceBuilder.Replace("$device", device.Id).Replace("$long", device.Longitude.ToString());
                    deviceBuilder.Replace("$lat", device.Latitude.ToString()).Replace("$evt", device.EventId);
                    deviceBuilder.Replace("$date",device.GpsTimeStamp.ToString("u"));
                    deviceBuilder.Replace("$speed",
                                                                                                       device.Speed.ToString());
                    allDeviceJson.Append(deviceBuilder);
                }
                return "{items:[" + allDeviceJson.Remove(allDeviceJson.Length - 1, 1) + "]}";
            }
            return "[" + allDevices + "]";
        }

        public static Repository Factory(string dataPoint)
        {
            return new Repository(dataPoint);
        }

        public static Repository Factory()
        {
            return new Repository();
        }

        private static void LogDataPoint(string dataPoint)
        {
            using (var connection  = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("CreateGpsDataPoint"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = connection;
                    cmd.Parameters.Add(new SqlParameter("gpsDataPoint", dataPoint));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static IEnumerable<string> Tokenize(string gpsDataPoints)
        {
            var dataPoints = gpsDataPoints.Split(new[] { "%%" }, StringSplitOptions.RemoveEmptyEntries);

            return dataPoints;
        }


        public void SaveDataPoints()
        {
            foreach (var dataPoint in Tokenize(_gpsDataPoint))
                Factory(dataPoint).SaveDataPoint();
        }

        public bool SaveDataPoint()
        {
            try
            {
                LogDataPoint(_gpsDataPoint);
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (var cmd = new SqlCommand("CreateGpsDataTrack"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = connection;                        
                        cmd.Parameters.Add(new SqlParameter("Longitude", _dataPointParser.Position.Longitude));
                        cmd.Parameters.Add(new SqlParameter("Latitude", _dataPointParser.Position.Latitude));

                        cmd.Parameters.Add(new SqlParameter("DeviceId", _dataPointParser.DeviceId));
                        cmd.Parameters.Add(new SqlParameter("Direction", _dataPointParser.Direction));
                        cmd.Parameters.Add(new SqlParameter("EventId", _dataPointParser.EventId));

                        cmd.Parameters.Add(new SqlParameter("GpsTimeStamp", _dataPointParser.GpsTimeStamp));
                        cmd.Parameters.Add(new SqlParameter("IsValidGps", _dataPointParser.IsGpsValid));
                        cmd.Parameters.Add(new SqlParameter("Status", _dataPointParser.Status));

                        cmd.Parameters.Add(new SqlParameter("VehicleSpeed", _dataPointParser.Speed));

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (SqlException exception)
            {
                EventLog.WriteEntry("ZamTrackGpsListener.Repository", exception.Message);   
            }
            catch(Exception exception)
            {
                EventLog.WriteEntry("ZamTrackGpsListener.Repository",exception.Message);                
            }
            return false;
        }

        public static void DeleteDataPointsForDevice(string deviceId)
        {
            using (var sqlConnection  = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                using (var cmd  = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DeleteAllDataPointsForDevice";
                    cmd.Connection = sqlConnection;
                    cmd.Parameters.Add(new SqlParameter("DeviceId", deviceId));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static string VehicleSpeedHistory(string deviceId, DateTime from, DateTime to)
        {
            var vehicleSpeeds =
                GetGpsPointsForDevice(deviceId,from,to).OrderBy(
                    x => x.GpsTimeStamp);
            if (vehicleSpeeds.Count()>0)
            {
                var sb = new StringBuilder();
                foreach (var speed in vehicleSpeeds)
                {
                    sb.Append(speed.VehicleSpeed).Append(",");
                }
                return "[" + sb.Remove(sb.Length - 1, 1) + "]";
            }
            return "[NaN,NaN]";
        }

        public static string GetGpsPointsForDeviceJson(string deviceId,DateTime from, DateTime to)
        {
            var dataPoints = GetGpsPointsForDevice(deviceId,from,to);
            var jsDeviceBuilder = new StringBuilder();
            if (dataPoints.Count > 0)
            {
                foreach (var gpsDataPoint in dataPoints)
                {
                    var jsonDeviceData =
                        new StringBuilder("{LT:$lat,LG:$long,TM:'$date',SP:$speed},");

                    jsonDeviceData.Replace("$lat", gpsDataPoint.Latitude.ToString());
                    jsonDeviceData.Replace("$long", gpsDataPoint.Longitude.ToString());
                    jsonDeviceData.Replace("$date", gpsDataPoint.GpsTimeStamp.ToString("o"));
                    jsonDeviceData.Replace("$speed",gpsDataPoint.VehicleSpeed.ToString());

                    jsDeviceBuilder.Append(jsonDeviceData);
                }
                return "{items:[" + jsDeviceBuilder.Remove(jsDeviceBuilder.Length - 1, 1) + "]}";
            }
            return "{items:[{LT:-15.416667,LG:28.283333,TM:0,SP:0}]}";
        }

        public static ICollection<GpsDataPoint> GetGpsPointsForDevice(string deviceId, DateTime from, DateTime to)
        {
            ICollection<GpsDataPoint> dataPoints = new List<GpsDataPoint>();
            using (var connection = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand())
            {
                connection.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetGpsPointsForDevice";
                cmd.Parameters.Add(new SqlParameter("deviceId", deviceId));
                cmd.Parameters.Add(new SqlParameter("from", from));
                cmd.Parameters.Add(new SqlParameter("to", to));
                cmd.Connection = connection;
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader != null)
                        while (reader.Read())
                        {
                            var dataPoint = new GpsDataPoint
                                                {
                                                    Latitude = decimal.Parse(reader["Latitude"].ToString()),
                                                    Longitude = decimal.Parse(reader["Longitude"].ToString()),
                                                   
                                                    Direction = decimal.Parse(reader["Direction"].ToString()),
                                                    GpsTimeStamp = DateTime.Parse(reader["GpsTimeStamp"].ToString()),
                                                    VehicleSpeed = decimal.Parse(reader["VehicleSpeed"].ToString())
                                                };
                            dataPoints.Add(dataPoint);
                        }
                }
            }
            return dataPoints;
        }

        public static string VehiclePathInJson(string deviceId, DateTime from, DateTime to)
        {
            var jsonPath = new StringBuilder();
            var gpsDataPoints = GetGpsPointsForDevice(deviceId,from,to);
            if (gpsDataPoints.Count() > 0)
            {
                foreach (var gpsDataPoint in gpsDataPoints)
                {
                    var jsonVehiclePathBuilder = new StringBuilder("{LT:$lat,LG:$long},");
                    jsonVehiclePathBuilder.Replace("$lat", gpsDataPoint.Latitude.ToString()).Replace("$long",
                                                                                                  gpsDataPoint.Longitude
                                                                                                      .
                                                                                                      ToString());
                    jsonPath.Append(jsonVehiclePathBuilder);
                }

                return "[" + jsonPath.Remove(jsonPath.Length - 1, 1) + "]";
            }
            return "[{LT:-15.416667,LG:28.283333}]";
        }
    }
}