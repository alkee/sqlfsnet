namespace sqlfsnet
{
    public class FileSystem
    {
        public FileSystem(string dbFilePath)
        {
            db = new Db(dbFilePath);
        }

        //public async Task<List<Item>> ListAsync(string absolutePath)
        //{

        //}

        private readonly Db db;
    }
}