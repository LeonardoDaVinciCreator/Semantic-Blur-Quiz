using UnityEngine;
using System;

namespace AtributeEditor
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class LocalizationKeyAttribute : PropertyAttribute { }
}
