using Localization;
using System.Collections.Generic;

namespace Verification
{
    public interface IWordsCache
    {
        string[] GetCache(string tableName, string key);
        void Dispose();
    }
}