using Microsoft.VisualStudio.TestTools.UnitTesting;
using sqlfsnet;

namespace sqlfsnet_test
{
    [TestClass]
    public class Working
    {
        [TestMethod]
        public void Test1()
        {
            var fs = new FileSystem(Env.DB_PATH);
        }
    }
}