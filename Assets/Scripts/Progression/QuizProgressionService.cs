using System;
using Data;
using UnityEngine.Rendering;
using Verification;

namespace Progression
{
    public class QuizProgressionService : IProgressionService
    {
        private int _wrongCount = 0;

        public int WrongCount => _wrongCount;

        public void ResetProgress()
        {
            _wrongCount = 0;
        }

        public float CalculatePixelSize(VerificationResult result, PixelizationStat pixelization, float step)
        {
            switch (result)
            {
                case VerificationResult.Win:
                    //получить данные
                    pixelization.CurrentPixelSize = pixelization.MaxPixelSize;
                    return pixelization.CurrentPixelSize;

                case VerificationResult.CloseMatch:
                    _wrongCount++;
                    pixelization.CurrentPixelSize = System.Math.Min(pixelization.CurrentPixelSize + step, pixelization.MaxPixelSize);
                    return pixelization.CurrentPixelSize;

                case VerificationResult.Wrong:
                    _wrongCount++;

                    float nextSize = pixelization.CurrentPixelSize - step;
                    if (nextSize <= pixelization.MinPixelSize)
                    {
                        pixelization.CurrentPixelSize = pixelization.MinPixelSize;
                    }
                    else
                    {
                        pixelization.CurrentPixelSize = nextSize;
                    }
                    return pixelization.CurrentPixelSize;

                default:
                    return pixelization.CurrentPixelSize;
            }
        }
    }
}