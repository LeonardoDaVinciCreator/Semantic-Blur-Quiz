using Data;
using UnityEngine.Rendering;
using Verification;

namespace Progression
{
    public class PixelizationStat
    {
        public float MinPixelSize;
        public float CurrentPixelSize;
        public float MaxPixelSize;
    }

    public interface IPixelizationService
    {
        void ResetProgress();
        float CalculatePixelSize(VerificationResult result, PixelizationStat pixelization, float step);
    }
}