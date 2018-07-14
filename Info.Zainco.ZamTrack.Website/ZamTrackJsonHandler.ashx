<%@ WebHandler Language="C#" Class="ZamTrackJsonHandler" %>

using System;
using System.Globalization;
using System.Web;
using Info.Zainco.ZamTrack.PersistenceManager;

public class ZamTrackJsonHandler : IHttpHandler {
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        context.Response.Write(CommandController(context));
    }

    private static string CommandController(HttpContext context)
    {
        var cmd = context.Request.QueryString["cmd"];
        var device = context.Request.QueryString["deviceId"];
        var start = DateTime.Now.AddDays(-2);
        var end = DateTime.Now;
        var startTime = DateTime.Now.ToLongTimeString();
        var endTime = DateTime.Now.ToLongTimeString();
        
        if (!string.IsNullOrEmpty(context.Request.QueryString["from"]) && !string.IsNullOrEmpty(context.Request.QueryString["to"]))
        {
            startTime = context.Request.QueryString["startTime"];
            endTime = context.Request.QueryString["endTime"];
            var culture = CultureInfo.CreateSpecificCulture("en-GB");
            
            var startDateTime = context.Request.QueryString["from"] + " " + startTime;
            var endDateTime = context.Request.QueryString["to"] + " " + endTime;
            
            start = DateTime.Parse(startDateTime, culture);
            end = DateTime.Parse(endDateTime, culture);
        }

        if (!string.IsNullOrEmpty(cmd))
        {
            if (!string.IsNullOrEmpty(device))
                if (cmd.Equals("DEVICE_DATA", StringComparison.OrdinalIgnoreCase))
                    return Repository.GetGpsPointsForDeviceJson(device,start,end);
            if (cmd.Equals("ALL_DEVICES", StringComparison.OrdinalIgnoreCase))
                return Repository.GetAllDevicesJson();
            if (cmd.Equals("DISTINCT_DEVICES", StringComparison.OrdinalIgnoreCase))
                return Repository.GetDistinctDevicesJson();
            if (cmd.Equals("VEHICLE_PATH_FOR_DATE_RANGE", StringComparison.OrdinalIgnoreCase))
                return Repository.VehiclePathInJson(device, start, end);
            if (cmd.Equals("DEVICE_SPEED_HISTORY", StringComparison.OrdinalIgnoreCase))
                return Repository.VehicleSpeedHistory(device, start,end);
            if (cmd.Equals("SPEEDING_EVENT", StringComparison.OrdinalIgnoreCase))
                return Repository.GetAllSpeedingVehicles();
            if (cmd.Equals("GEOFENCE_EVENT", StringComparison.OrdinalIgnoreCase))
                return Repository.GetAllVehiclesBreachingGeofence(); 
        }
        return "[ZamTrackServerError:Unable to retrieve data. Please check your command and arguments.]";
    }

    public bool IsReusable {
        get {
            return false;
        }
    }
}