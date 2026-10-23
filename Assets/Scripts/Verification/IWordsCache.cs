using Localization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Verification
{
    public interface IWordsCache
    {
        Task<string[]> GetCacheAsync(string tableName, string key);
        void Dispose();
    }
}