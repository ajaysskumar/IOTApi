namespace IoT.Core.MessageProcessing
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
            this.QueueReaderInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.QueueReaderServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // QueueReaderInstaller
            // 
            this.QueueReaderInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.QueueReaderInstaller.Password = null;
            this.QueueReaderInstaller.Username = null;
            // 
            // QueueReaderServiceInstaller
            // 
            this.QueueReaderServiceInstaller.ServiceName = "QueueReader";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.QueueReaderInstaller,
            this.QueueReaderServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller QueueReaderInstaller;
        private System.ServiceProcess.ServiceInstaller QueueReaderServiceInstaller;
    }
}