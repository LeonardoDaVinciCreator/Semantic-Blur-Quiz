using Data;
using Json;
using Localization;
using Progression;
using Services;
using Verification;
using UnityEngine;

namespace Services
{
    public interface IQuizServiceFactory
    {
        IQuizJsonService CreateJsonService();
        IWordsCache CreateWordsCache();
        IVerificationService CreateVerificationService(IWordsCache cache);
        IPixelizationService CreateProgressionService();
    }
}
