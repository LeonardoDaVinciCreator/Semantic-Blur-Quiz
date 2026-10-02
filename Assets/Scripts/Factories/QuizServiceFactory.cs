using Data;
using Json;
using Localization;
using Progression;
using Services;
using Verification;
using UnityEngine;

namespace Services
{
    public class QuizServiceFactory : IQuizServiceFactory
    {
        public IQuizJsonService CreateJsonService() 
            => new Json.QuizJsonService();

        public Verification.IWordsCache CreateWordsCache() 
            => new Verification.WordsCache();

        public Verification.IVerificationService CreateVerificationService(Verification.IWordsCache cache) 
            => new Verification.QuizVerificationService(cache);

        public Progression.IProgressionService CreateProgressionService() 
            => new Progression.QuizProgressionService();
    }
}
