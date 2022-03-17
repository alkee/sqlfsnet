namespace sqlfsnet
{
    internal static class ValidationExt
    {
        public static string ValidateAbsolutePath(this string absolutePath)
        {
            ArgumentNullException.ThrowIfNull(absolutePath);
            if (absolutePath.StartsWith(Item.SEPARATOR) == false)
                throw new ArgumentException($"should start with {Item.SEPARATOR}", nameof(absolutePath));
            return absolutePath;
        }

        public static string ValidateItemName(this string itemName)
        {
            ArgumentNullException.ThrowIfNull(itemName);
            if (itemName.Length == 0) throw new ArgumentException($"empty itemName", nameof(itemName));
            if (itemName.Contains(Item.SEPARATOR)) throw new ArgumentException($"contains not allowed character", nameof(itemName));
            return itemName;
        }
    }
}