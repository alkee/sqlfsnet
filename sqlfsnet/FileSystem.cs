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
            var tree = await db.GetItemTree(dirAbsolutePath);

            var dir = tree.LastOrDefault();
            if (dir is null
                || dir.ItemType != Item.Type.DIRECTORY)
                throw new DirectoryNotFoundException($"path not found : {dirAbsolutePath}");
            return await db.SelectItems(dir);
        }

        public async Task<Item> MakeDirAsync(string dirAbsolutePath, bool recursive = false)
        {
            var items = await db.GetItemTree(dirAbsolutePath);

            var itemNames = dirAbsolutePath
                .TrimEnd(Item.SEPARATOR)
                .Split(Item.SEPARATOR);
            if (items.Last() is not null)
                throw new IOException($"directory already exists : {dirAbsolutePath}");
            items.RemoveAt(items.Count - 1);

            if (recursive == false && items.Count == itemNames.Length - 1)
            { // 마지막 dir 만 없는 경우
                return await db.CreateDirectory(itemNames.Last(), items.Last()!);
            }

            for (var i = items.Count; i < itemNames.Length; ++i)
            {
                var itemName = itemNames[i];
                items.Add(await db.CreateDirectory(itemName, items.Last()!));
            }
            return items.Last()!;
        }

        public async Task RemoveAsync(string absolutePath, bool force)
        {
            var item = await db.SelectItem(absolutePath);
            if (item is null)
                throw new IOException($"not found : {absolutePath}");
            if (item.ItemType == Item.Type.DIRECTORY)
            { // directory 인 경우 force 가 아니면 비어있는 경우에만.
                var count = await db.CountItems(item);
                if (count > 0 && force == false)
                    throw new IOException($"not empty directory : {absolutePath}");
            }
            await db.DeleteItem(item);
        }

        public async Task<Item> TouchAsync(string absoluteFilePath)
        {
            var dirPath = Path.GetDirectoryName(absoluteFilePath) ?? Item.SEPARATOR.ToString()/*root*/;
            var filePath = Path.GetFileName(absoluteFilePath);
            var file = await db.SelectItem(absoluteFilePath);
            if (file is not null)
            {
                if (file.ItemType != Item.Type.FILE)
                    throw new IOException($"not a file : {absoluteFilePath}");
                file.LastModifiedUtc = DateTime.UtcNow;
                await db.Conn.UpdateAsync(file);
                return file;
            }

            var dir = await db.SelectItem(dirPath);
            if (dir is null)
                throw new IOException($"path not found : {dirPath}");
            file = new Item
            {
                ItemType = Item.Type.FILE,
                Name = filePath,
                ParentItemId = dir.Id,
            };
            await db.Conn.InsertAsync(file);
            return file;
        }

        private readonly Db db;
    }
}