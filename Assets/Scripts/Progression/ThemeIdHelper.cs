namespace Progression
{
    public static class ThemeIdHelper
    {
        public static string FromFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder)) return string.Empty;
            return System.IO.Path.GetFileName(folder);
        }
    }
}