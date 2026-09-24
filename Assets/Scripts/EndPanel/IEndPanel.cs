
namespace EndPanel
{
    public interface IEndPanel
    {
        bool TryGetBestTime(out float time);
        bool TryGetBestScore(out int score);
        void Save(float time, int score);
    }
}