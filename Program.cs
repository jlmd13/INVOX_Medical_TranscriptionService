using System;
using System.ServiceProcess;

namespace INVOX_Medical_TranscriptionService
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        { 
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new INVOX_Medical_Transcription()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
