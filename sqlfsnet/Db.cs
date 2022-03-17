using SQLite;
using System.Diagnostics;
using System.Reflection;

namespace sqlfsnet
{
    internal class Db
    {
        public readonly SQLiteAsyncConnection Conn;
        public readonly Item Root;

        public Db(string dbFilePath)
        {
            Conn = new SQLiteAsyncConnection(dbFilePath, storeDateTimeAsTicks: true);

            var syncConn = Conn.GetConnection(); // 생성자에서 비동기를 사용할 수 없으므로

            // initial database configuration
            syncConn.ExecuteScalar<string>($"PRAGMA journal_mode = WAL"); // to improve performace of trasaction for massive insertion
            syncConn.ExecuteScalar<int>($"PRAGMA fullfsync = ON"); // https://www.sqlite.org/atomiccommit.html#_incomplete_disk_flushes
            syncConn.ExecuteScalar<int>($"PRAGMA synchronous = EXTRA"); // for extra durability than FULL

            // code first entities
            var tables = from t in Assembly.GetExecutingAssembly().GetTypes()
                         where t.IsDefined(typeof(SqliteTableAttribute))
                            && t.IsClass
                         select t;
            // update schema
            syncConn.CreateTables(CreateFlags.None, tables.ToArray());

            // essential content
            Root = syncConn.Find<Item>(Item.ROOT_ITEM_ID);
            if (Root is null)
            {
                Root = new Item
                {
                    Id = Item.ROOT_ITEM_ID,
                    ItemType = Item.Type.DIRECTORY,
                    Name = Item.ROOT_ITEM_NAME,
                    ParentItemId = long.MaxValue, // invalid. root 가 parent 검사에 포함되지 않도록
                };
                syncConn.InsertOrReplace(Root);
            }
        }

        public async Task<Item?> SelectItem(string absolutePath)
        {
            absolutePath = absolutePath
                .ValidateAbsolutePath()
                .TrimEnd(Item.SEPARATOR);
            if (absolutePath.Length < 1) return Root;

            var itemNames = absolutePath.Split(Item.SEPARATOR);
            var items = await GetItemTree(absolutePath);
            if (items.Count < itemNames.Length) // not found
                return null;
            return items.Last();
        }

        private async Task<Item?> SelectItem(string itemName, Item parent)
        {
            itemName.ValidateItemName();
            return await Conn
                .Table<Item>()
                .Where(x => x.ParentItemId == parent.Id && x.Name == itemName)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Item>> SelectItems(Item directory)
        {
            if (directory.ItemType != Item.Type.DIRECTORY)
                throw new ArgumentException($"not a directory", nameof(directory));
            return await Conn
                .Table<Item>()
                .Where(x => x.ParentItemId == directory.Id)
                .ToListAsync();
        }

        public async Task<List<Item>> GetItemTree(string absolutePath)
        {
            var itemNames = absolutePath
                .ValidateAbsolutePath()
                .Trim(Item.SEPARATOR)
                .Split(Item.SEPARATOR);

            var ret = new List<Item>() { Root };
            foreach (var itemName in itemNames)
            {
                var item = await SelectItem(itemName, ret.Last());
                if (item is null) return ret;
                ret.Add(item);
            }
            return ret;
        }

        //public async Task<List<Item>> GetAncestorTree(Item item)
        //{
        //    var ret = new List<Item>();
        //    while (item.ParentItemId != Item.ROOT_ITEM_ID)
        //    {
        //        item = await Conn.FindAsync<Item>(item.ParentItemId);
        //        ret.Insert(0, item);
        //    }
        //    ret.Insert(0, Root); // must be
        //    return ret;
        //}

        public async Task<Item> CreateDirectory(string itemName, Item parent)
        {
            var prev = await SelectItem(itemName, parent);
            if (prev is not null)
                throw new InvalidOperationException($"already existing item : {itemName}");
            var item = new Item
            {
                Name = itemName,
                ParentItemId = parent.Id,
                ItemType = Item.Type.DIRECTORY,
            };
            await Conn.InsertAsync(item);
            Debug.Assert(item.Id != default);
            return item;
        }

        //public async Task<Item> GetParentDirectory(string absolutePath)
        //{
        //    var dirPath = absolutePath
        //        .ValidateAbsolutePath()
        //        .TrimEnd(Item.SEPARATOR)
        //        .Substring(0, absolutePath.LastIndexOf(Item.SEPARATOR));
        //    var dir = await SelectItem(dirPath);
        //    if (dir is null || dir.ItemType != Item.Type.DIRECTORY)
        //        throw new DirectoryNotFoundException($"dir not found : {Item.SEPARATOR}{dirPath}");
        //    return dir!;
        //}
    }
}