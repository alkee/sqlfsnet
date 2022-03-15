namespace sqlfsnet
{
    public class FileSystem
    {
        public FileSystem(string dbFilePath)
        {
            db = new Db(dbFilePath);
        }

        public async Task<List<Item>> ListAsync(string dirAbsolutePath)
        {
            var dir = (await db.GetItemTree(dirAbsolutePath))
                .LastOrDefault();
            if (dir is null
                || dir.ItemType != Item.Type.DIRECTORY)
                throw new DirectoryNotFoundException($"path not found : {dirAbsolutePath}");
            return await db.SelectItems(dir);
        }

        public async Task<Item> CreateDirAsync(string dirAbsolutePath, bool recursive = false)
        {
            var items = await db.GetItemTree(dirAbsolutePath);

            var itemNames = dirAbsolutePath
                .TrimEnd(Item.SEPARATOR)
                .Split(Item.SEPARATOR);
            if (items.Count == itemNames.Length)
                throw new IOException($"directory already exists : {dirAbsolutePath}");
            if (recursive == false && items.Count == itemNames.Length - 1)
            {
                return await db.CreateDirectory(itemNames.Last(), items.Last());
            }
            for (var i = items.Count; i < itemNames.Length; ++i)
            {
                var itemName = itemNames[i];
                items.Add(await db.CreateDirectory(itemName, items.Last()));
            }
            return items.Last();
        }

        private readonly Db db;
    }
}