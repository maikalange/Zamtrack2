using System;
using System.Net;
using System.Threading;

namespace Info.Zainco.ZamTrack.GpsServer
{
    class Program
    {
        private static GpsListeningService _listeningService;
        static void Main()
        {
            ThreadPool.QueueUserWorkItem(RunServer);
            Console.WriteLine("Press ESC to stop the server...");
            ConsoleKeyInfo cki;
            while (true)
            {
                cki = Console.ReadKey();
                if (cki.Key==ConsoleKey.Escape)
                {
                    break;
                }
            }
        }

        private static void RunServer(object state)
        {
            _listeningService = new GpsListeningService(IPAddress.Loopback,8080);
            _listeningService.Listen();
        }
    }
}
