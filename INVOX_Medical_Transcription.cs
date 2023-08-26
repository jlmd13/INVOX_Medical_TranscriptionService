using DOM.Jobs;
using DOM.Transcriptions;
using INVOX_Medical_TranscriptionService.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Timers;

namespace INVOX_Medical_TranscriptionService
{
    public partial class INVOX_Medical_Transcription : ServiceBase
    {
        #region Private Variables

        private System.Timers.Timer _timer = new System.Timers.Timer();
        private bool _executing;
        private DailyJob _dailyJob = new DailyJob();
        private Queue<Transcription> _transactionsQueue = new Queue<Transcription>();
        private int _userId;
        private string _path;

        #endregion

        #region Protected Methods 
        protected override void OnStart(string[] args)
        {
            ReadParameters(args);
            _timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
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
            _transactionsQueue.Clear();
            if (!Directory.Exists(_path))
            {
                Logger.Logger.Instance.LogMessage("Main directory does not exist. Aborting job.");
                return;
            }

            EnqueFileTranscriptions(_path);

            while (_transactionsQueue.Any())
            {
                List<Transcription> transcriptionPack = new List<Transcription>();
                transcriptionPack.AddRange(GetTranscriptionPack());
                foreach (Transcription currentTranscription in transcriptionPack)
                {
                    ConnectionHelper.CallTranscriptService(currentTranscription);
                    if (!currentTranscription.Processed && currentTranscription.Attemps > 0)
                    {
                        currentTranscription.UseAttemp();
                        _transactionsQueue.Enqueue(currentTranscription);
                    }
                }

                SpinWait.SpinUntil(() => transcriptionPack.All(t => t.Finished));
            }

            _executing = false;
            WriteResults();
        }

        #endregion

        #region Private Methods

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            if (DateTime.Now.TimeOfDay.Hours == 0 && DateTime.Now.TimeOfDay.Minutes == 0 && !_executing)
            {
                _executing = true;
                StartJob();
            }
        }

        private void EnqueFileTranscriptions(string path)
        {
            Directory.GetFiles(path).ToList().ForEach(f => _dailyJob.AddTranscription(f, _userId));
            _dailyJob.Transcriptions.Where(t => t.IsValid).ToList().ForEach(t => _transactionsQueue.Enqueue(t));
        }

        private List<Transcription> GetTranscriptionPack(int nTranscriptions = 3)
        {
            List<Transcription> transcriptionPack = new List<Transcription>();

            for(int i = 0; i < nTranscriptions && _transactionsQueue.Any(); i++)
            {
                transcriptionPack.Add(_transactionsQueue.Dequeue());
            }

            return transcriptionPack;
        }

        private bool ReadParameters(string[] args)
        {
            try
            {
                _path = args[0];
                _userId = int.Parse(args[1]);

                return true;
            }
            catch (Exception e)
            {
                Logger.Logger.Instance.LogMessage($"El formato de los parámetros establecidos para el servicio es incorrecto. Error: {e.Message}");
                return false;
            }
        }

        private void WriteResults()
        {
            foreach (Transcription transcription in _dailyJob.Transcriptions)
            {
                File.WriteAllText(Path.Combine(_path,  $"{transcription.GetFileName()}.txt"), transcription.Result);
            }
        }

        #endregion
    }
}
