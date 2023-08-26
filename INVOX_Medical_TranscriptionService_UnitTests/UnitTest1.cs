using DOM.Transcriptions;
using INVOX_Medical_TranscriptionService.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace INVOX_Medical_TranscriptionService_UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private readonly string SamplePath = "..\\Resources\\Otorrinologia.mp3";
        private readonly string InvalidSamplePath = "..\\Resources\\NoData.mp3";

        [TestMethod]
        public void TestTranscription_Valida()
        {
            Transcription transcription = new Transcription(SamplePath, 1);
            ConnectionHelper.CallTranscriptService(transcription);
            Assert.IsTrue(transcription.Result.Equals("Texto transcrito de prueba para otorrinologia"));
        }

        [TestMethod]
        public void TestTranscription_NoValida()
        {
            Transcription transcription = new Transcription(InvalidSamplePath, 1);
            ConnectionHelper.CallTranscriptService(transcription);
            Assert.IsFalse(transcription.IsValid);
            
        }
    }
}
