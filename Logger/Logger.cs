using System;
using System.IO;

namespace Logger
{
    public sealed class Logger
    {
        #region Private Variables

        private readonly static Logger _instance = new Logger();
        private readonly string FileLogName = "INVOX_Medical.log";

        private string LogFullPath => Path.Combine(LogPath, FileLogName);

        #endregion

        #region Constructor

        private Logger()
        {
        }

        #endregion

        #region Public Properties

        public string LogPath { set; get; } = Environment.CurrentDirectory;

        public static Logger Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        #region Public Methods

        public void LogMessage(string message)
        {
            if (!File.Exists(LogFullPath)) 
            { 
                File.Create(LogFullPath);
            }

            File.AppendAllText(LogFullPath, $"{DateTime.Now}:  {message}\n");

        }

        #endregion
    }
}
