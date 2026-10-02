using Localization;
using System.Collections.Generic;

public class IWordsCache
{
    public string[] GetCache(string tableName, string key);
    void Dispose();
    void Clear();
}