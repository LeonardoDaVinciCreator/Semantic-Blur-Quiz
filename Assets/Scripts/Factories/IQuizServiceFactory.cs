using Data;
using Localization;
using Progression;
using UnityEngine;

namespace Services
{
    public interface IQuizServiceFactory
    {
        IQuizJsonService CreateJsonService();
        IWordsCache CreateWordsCache();
        IVerificationService CreateVerificationService(IWordsCache cache);
        IProgressionService CreateProgressionService();
    }
}
