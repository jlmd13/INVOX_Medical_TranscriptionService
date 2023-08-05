using DOM.Jobs;
using DOM.Transcriptions;
using INVOX_Medical_TranscriptionService.Helpers;
using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace INVOX_Medical_TranscriptionService
{
    public partial class INVOX_Medical_Transcription : ServiceBase
    {
        #region Private Variables

        private Timer _timer = new Timer();
        private bool _executing;

        #endregion

        #region Protected Methods 
        protected override void OnStart(string[] args)
        {
            _timer.Elapsed += new ElapsedEventHandler(OnelapsedTime);
            _timer.Interval = 5000;
            _timer.Enabled = true;
        }

        protected override void OnStop()
        {
        }

        #endregion

        #region Public Methods 

        

        public INVOX_Medical_Transcription()
        {
            InitializeComponent();
        }

        public void StartJob()
        {
            DailyJob dailyJob = new DailyJob();
            string path = "D:\\MP3Files";
            if (!Directory.Exists(path))
            {
                Logger.Logger.Instance.LogMessage("Main directory does not exist. Aborting job.");
                return;
            }

            //cambiar el 1
            Directory.GetFiles(path).ToList().ForEach(f => dailyJob.AddTranscription(f, 1));
            Queue<Transcription> transactionsQueue = new Queue<Transcription>();

            dailyJob.Transcriptions.Where(t => t.IsValid).ToList().ForEach(t => transactionsQueue.Enqueue(t)) ;

            while(transactionsQueue.Any())
            {
                Transcription currentTranscription = transactionsQueue.Dequeue();
                ConnectionHelper.CallTranscriptService(currentTranscription);
                if (!currentTranscription.Processed && currentTranscription.Attemps > 0)
                {
                    currentTranscription.UseAttemp();
                    transactionsQueue.Enqueue(currentTranscription);
                }
            }

            _executing = false;
        }

        #endregion

        #region Private Methods

        private void OnelapsedTime(object source, ElapsedEventArgs e)
        {
            if (DateTime.Now.TimeOfDay.Hours == 0 && DateTime.Now.TimeOfDay.Minutes == 0 && !_executing)
            {
                _executing = true;
                StartJob();
            }
        }

        #endregion
    }
}
