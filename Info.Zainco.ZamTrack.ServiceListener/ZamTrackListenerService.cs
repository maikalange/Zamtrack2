using System.Net;
using System.ServiceProcess;
using System.Threading;
using Info.Zainco.ZamTrack.GpsServer;

namespace Info.Zainco.ZamTrack.ServiceListener
{
    public partial class ZamTrackListenerService : ServiceBase
    {
        public ZamTrackListenerService()
        {
            InitializeComponent();
        }

        private static GpsListeningService _listeningService;
        static void InvokeService()
        {
            ThreadPool.QueueUserWorkItem(RunServer);            
        }

        private static void RunServer(object state)
        {
            _listeningService = new GpsListeningService(IPAddress.Parse("46.252.195.163"), 8080);
            _listeningService.Listen();
        }

        protected override void OnStart(string[] args)
        {
            InvokeService();
            
        }

        protected override void OnStop()
        {
        }
    }
}
