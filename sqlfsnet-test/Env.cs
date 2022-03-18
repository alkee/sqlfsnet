namespace sqlfsnet_test
{
    internal class Env
    {
        private static int dbCount = 0;

        public static sqlfsnet.FileSystem CreateFileSystem()
        {
            // 단순히 `:memory:` 나
            //    var dbPath = "file::memory:";
            // 와 같은 database open 으로는 독립적인 memory database 가 생성되지 않고 항상 공유된 db 가
            // 생성되어 비동기테스트함수가 실행될 때 다른 테스트의 간섭을 받는 문제를 피하기 위해
            var dbPath = $"file:memdb{dbCount++}?mode=memory"; // https://github.com/praeclarum/sqlite-net/issues/1077

            return new sqlfsnet.FileSystem(dbPath);
        }
    }
}