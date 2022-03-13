using SQLite;
using System.Reflection;

namespace sqlfsnet
{
    internal class Db
    {
        public readonly SQLiteAsyncConnection Conn;
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
        }
    }
}