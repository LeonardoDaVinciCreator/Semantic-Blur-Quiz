using Localization;
using System.Collections.Generic;

public interface IWordsCache
{
    string[] GetCache(string tableName, string key);
    void Dispose();
    void Clear();
}