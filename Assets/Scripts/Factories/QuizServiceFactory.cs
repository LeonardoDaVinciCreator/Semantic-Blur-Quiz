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
            => new QuizJsonService();

        public IWordsCache CreateWordsCache() 
            => new WordsCache();

        public IVerificationService CreateVerificationService(IWordsCache cache) 
            => new QuizVerificationService(cache);

        public IPixelizationService CreateProgressionService() 
            => new PixelizationService();
    }
}
