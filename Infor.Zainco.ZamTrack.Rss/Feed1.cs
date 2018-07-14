using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using Info.Zainco.ZamTrack.PersistenceManager;

namespace Infor.Zainco.ZamTrack.Rss
{
    public class Feed1 : IFeed1
    {
        public SyndicationFeedFormatter CreateFeed()
        {
            // Create a new Syndication Feed.
            var feed = new SyndicationFeed("Zamtrack Fleet Monitoring", "A Zainco Ventures Syndication Feed", null);
            var items = new List<SyndicationItem>();

            var devices = Repository.GetAllDevices();
            // Create a new Syndication Item.
            foreach (var device in devices)
            {
                var item = new SyndicationItem
                               {
                                   Id = Guid.NewGuid().ToString(),
                                   Title = new TextSyndicationContent(device.EventDescription),
                                   Content = GenerateContent(device),
                                   PublishDate = device.GpsTimeStamp
                               };

                item.Authors.Add(new SyndicationPerson("support@zainco.info"));
                item.Categories.Add(new SyndicationCategory(device.EventDescription));
                item.Links.Add(GenerateGoogleMapLink(device));
                items.Add(item);
            }

            feed.Items = items;

            // Return ATOM or RSS based on query string
            // rss -> http://localhost:8732/Design_Time_Addresses/Infor.Zainco.ZamTrack.Rss/Feed1/
            // atom -> http://localhost:8732/Design_Time_Addresses/Infor.Zainco.ZamTrack.Rss/Feed1/?format=atom
            var query = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["format"];
            SyndicationFeedFormatter formatter = null;
            if (query == "atom")
            {
                formatter = new Atom10FeedFormatter(feed);
            }
            else
            {
                formatter = new Rss20FeedFormatter(feed);
            }

            return formatter;
        }

        private static SyndicationLink GenerateGoogleMapLink(Device device)
        {
            var googleMapLink = @"http://maps.google.com/maps/api/staticmap?maptype=roadmap&center=" + device.Latitude + "," + device.Longitude + "&zoom=16&size=640x300&markers=color:0xEF2533|label:R|" + device.Latitude + "," + device.Longitude + "&sensor=false";

            return new SyndicationLink(new Uri(googleMapLink));
        }

        private static SyndicationContent GenerateContent(Device device)
        {
            var content = string.Format("Driver:{0} , Vehicle:{1}/{2}/{3}, Speed:{4} km/h,Time: {5}", device.Driver, device.Make,
                                        device.Model, device.Reg, device.Speed,device.GpsTimeStamp);
            return SyndicationContent.CreatePlaintextContent(content);
        }
    }
}
