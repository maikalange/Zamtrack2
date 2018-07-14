using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Info.Zainco.ZamTrack.PersistenceManager
{
    public static class InMemoryMessageQueue
    {
        public static Queue<String> GpsDataPoints { get; set; }
    }
}
