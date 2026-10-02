using Data;
using Localization;
using Progression;
using UnityEngine;

namespace Services
{
    public class QuizServiceFactory : IQuizServiceFactory
    {
        public IQuizJsonService CreateJsonService() 
            => new Json.QuizJsonService();

        public IWordsCache CreateWordsCache() 
            => new WordsCache();

        public IVerificationService CreateVerificationService(IWordsCache cache) 
            => new Verification.QuizVerificationService(cache);

        public IProgressionService CreateProgressionService() 
            => new Progression.QuizProgressionService();
    }
}
