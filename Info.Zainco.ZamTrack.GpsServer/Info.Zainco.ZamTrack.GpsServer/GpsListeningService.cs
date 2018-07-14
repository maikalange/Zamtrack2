using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Info.Zainco.ZamTrack.PersistenceManager;

namespace Info.Zainco.ZamTrack.GpsServer
{
    public class GpsListeningService
    {
        private TcpListener _listener;
        private readonly IPAddress _address;
        private readonly int _port;
        private bool _listening;
        private readonly object _syncRoot = new object();

        public GpsListeningService(IPAddress address, int port)
        {
            _port = port;
            _address = address;
        }

        public IPAddress Address { get { return _address; } }
        public int Port  { get { return _port; } }
        public bool Listening  { get { return _listening; } }

        public void  Listen()
        {
            try
            {
                lock (_syncRoot)
                {
                    _listener = new TcpListener(_address,_port);
                    _listener.Start();
                    _listening = true;
                }
                do
                {
                    var newClient = _listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(ProcessClient,newClient);
                } while (_listening);
            }
            catch (SocketException se)
            {
                EventLog.WriteEntry("GpsListeningService.Listent()", se.Message, EventLogEntryType.Error);                
            }
            finally
            {
                StopListening();
            }
        }

        private static void ProcessClient(object client)
        {
            var newClient = client as TcpClient;
            try
            {
                var clientData = new StringBuilder();

                if (newClient != null)
                    using (var networkStream = newClient.GetStream())
                    {
                        networkStream.ReadTimeout = int.Parse(ConfigurationManager.AppSettings["stream.read.timeout"]);
                        while (networkStream.DataAvailable)
                        {
                            try
                            {
                                if (networkStream.CanRead)
                                {
                                    var bytes = new byte[1024];
                                    var bytesRead = networkStream.Read(bytes, 0, bytes.Length);
                                    Repository.Factory(clientData.Append(Encoding.ASCII.GetString(bytes, 0, bytesRead)).ToString()).SaveDataPoints();
                                }
                            }
                            catch (SocketException sex)
                            {
                                EventLog.WriteEntry("ZamTrackListener", sex.Message,
                                                    EventLogEntryType.Information);
                            }
                            catch (IOException ioe)
                            {
                                EventLog.WriteEntry("ZamTrackListener", ioe.Message, EventLogEntryType.Information);
                            }
                        } 
                    }
            }
            finally
            {
                if (newClient != null)
                    newClient.Close();
            }
        }

        private void StopListening()
        {
            if (_listening)
                lock (_syncRoot)
                {
                    _listening = false;
                    _listener.Stop();
                }
        }
    }
}
