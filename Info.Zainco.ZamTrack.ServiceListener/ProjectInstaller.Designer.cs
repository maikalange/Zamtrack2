namespace Info.Zainco.ZamTrack.ServiceListener
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GPSProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.GPSServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // GPSProcessInstaller
            // 
            this.GPSProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.GPSProcessInstaller.Password = null;
            this.GPSProcessInstaller.Username = null;
            // 
            // GPSServiceInstaller
            // 
            this.GPSServiceInstaller.Description = "Zam-Track Listener Service";
            this.GPSServiceInstaller.DisplayName = "Zam-Track Listener Service";
            this.GPSServiceInstaller.ServiceName = "ZamTrackListenerService";
            this.GPSServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.GPSProcessInstaller,
            this.GPSServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller GPSProcessInstaller;
        private System.ServiceProcess.ServiceInstaller GPSServiceInstaller;
    }
}