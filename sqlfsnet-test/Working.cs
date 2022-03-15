using Microsoft.VisualStudio.TestTools.UnitTesting;
using sqlfsnet;
using System.IO;
using System.Threading.Tasks;

namespace sqlfsnet_test
{
    [TestClass]
    public class Working
    {
        [TestMethod]
        public async Task Test_CreateDir()
        {
            var fs = new FileSystem(Env.DB_PATH);
            var e = await fs.CreateDirAsync("/a/b/c/d/e", true);
            Assert.AreEqual("e", e.Name);
            await Assert.ThrowsExceptionAsync<IOException>(() => fs.CreateDirAsync("/a/b/c/d/e", true)); // already exists
            var cc = await fs.CreateDirAsync("/a/b/cc/", true);
            Assert.AreEqual("cc", cc.Name);
            var items = await fs.ListAsync("/a/b");
            Assert.AreEqual(2/*c, cc*/, items.Count);
        }
    }
}