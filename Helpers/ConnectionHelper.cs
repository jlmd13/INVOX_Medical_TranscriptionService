using DOM.Transcriptions;
using DOM.TranscriptionService.Enums;
using System;

namespace INVOX_Medical_TranscriptionService.Helpers
{
    public static class ConnectionHelper
    {
        #region Public Methods

        public static void CallTranscriptService(Transcription transcription)
        {
            TranscriptionService.FileTranscription fileTranscription = new TranscriptionService.FileTranscription();
            TranscriptionService.TranscriptService transcriptService = new TranscriptionService.TranscriptService();
            fileTranscription.FilePath = transcription.GetFileName();
            fileTranscription.UserId = transcription.UserId;
            string[] result = transcriptService.GetTranscription(fileTranscription);
            transcription.Result = result[1];
            Enum.TryParse(result[0], out TranscriptionStatusEnum transcriptionStatus);
            transcription.Processed = transcriptionStatus.Equals(TranscriptionStatusEnum.Ok);
            transcription.Finished = true;
            if (transcription.Processed)
            {
                Logger.Logger.Instance.LogMessage($"Transcription with Report ID: {transcription.ReportId} transcripted succesfuly");
            }
            else
            {
                Logger.Logger.Instance.LogMessage($"Transcription {transcription.ReportId} failed:  {result[0]}");
            }
        }

        #endregion

        
    }
}
