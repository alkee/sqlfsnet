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

        [TestMethod]
        public async Task Test_Touch()
        {
            var fs = new FileSystem(Env.DB_PATH);
            const string ENV_DIR = "/a/b/c/d/e";

            await fs.CreateDirAsync(ENV_DIR, true);
            await Assert.ThrowsExceptionAsync<IOException>(() => fs.Touch("/a/bb/c"));
            var cc = await fs.Touch("/a/b/cc");
            Assert.AreEqual(0, cc.Size);
            await Task.Delay(1000);
            var touched_cc = await fs.Touch("/a/b/cc");
            Assert.AreEqual(0, touched_cc.Size);
            Assert.AreEqual(cc.CreatedUtc, touched_cc.CreatedUtc);
            Assert.IsTrue(cc.LastModifiedUtc < touched_cc.LastModifiedUtc);
        }
    }
}