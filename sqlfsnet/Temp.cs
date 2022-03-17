namespace sqlfsnet
{
    public static class Path
    {
        public static string GetDirectoryName(string path)
        {
            if (path.Length == 0) return string.Empty;
            return path[..path.LastIndexOf(Item.SEPARATOR)];
        }

        public static string GetFileName(string path)
        {
            if (path.Length == 0) return string.Empty;
            return path[(path.LastIndexOf(Item.SEPARATOR) + 1)..];
        }
    }
}