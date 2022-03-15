using SQLite;

namespace sqlfsnet
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SqliteTableAttribute
        : Attribute
    {
    }

    #region FileSystem 을 통해 전달받을 수 있는(public) 정보들

    [SqliteTable]
    public class Item
    {
        public const char SEPARATOR = '/';
        public const long ROOT_ITEM_ID = 0L;
        public const string ROOT_ITEM_NAME = "";

        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        [Indexed]
        public long ParentItemId { get; set; } = ROOT_ITEM_ID;

        public enum Type
        {
            FILE,
            DIRECTORY
        }

        public Type ItemType { get; set; }

        [Indexed]
        public string Name { get; set; } = string.Empty;

        public long Size { get; set; } = 0;

        public DateTime LastModifiedUtc { get; set; } = DateTime.UtcNow;
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }

    #endregion FileSystem 을 통해 전달받을 수 있는(public) 정보들

    [SqliteTable]
    internal class FileContent
    {
        [Indexed]
        public long ItemId { get; set; }

        public byte[] Content { get; set; } = Array.Empty<byte>();
    }
}