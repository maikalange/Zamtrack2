﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Info.Zainco.ZamTrack.ServiceListener
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[] 
                                              { 
                                                  new ZamTrackListenerService() 
                                              };
            ServiceBase.Run(servicesToRun);
        }
    }
}
