using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace sqlfsnet_test
{
    [TestClass]
    public class Working
    {
        [TestMethod]
        public async Task Test_mkdir()
        {
            var fs = Env.CreateFileSystem();
            var e = await fs.MakeDirAsync("/a/b/c/d/e", true);
            Assert.AreEqual("e", e.Name);
            await Assert.ThrowsExceptionAsync<IOException>(() => fs.MakeDirAsync("/a/b/c/d/e", true)); // already exists
            var cc = await fs.MakeDirAsync("/a/b/cc/", true);
            Assert.AreEqual("cc", cc.Name);
            var items = await fs.ListAsync("/a/b");
            Assert.AreEqual(2/*c, cc*/, items.Count);
        }

        [TestMethod]
        public async Task Test_touch()
        {
            var fs = Env.CreateFileSystem();
            const string ENV_DIR = "/a/b/c/d/e";

            await fs.MakeDirAsync(ENV_DIR, true);
            await Assert.ThrowsExceptionAsync<IOException>(() => fs.TouchAsync("/a/bb/c"));
            var cc = await fs.TouchAsync("/a/b/cc");
            Assert.AreEqual(0, cc.Size);
            await Task.Delay(1000); // 시간확인을 위해 대기
            var touched_cc = await fs.TouchAsync("/a/b/cc");
            Assert.AreEqual(0, touched_cc.Size);
            Assert.AreEqual(cc.CreatedUtc, touched_cc.CreatedUtc);
            Assert.IsTrue(cc.LastModifiedUtc < touched_cc.LastModifiedUtc);
        }

        [TestMethod]
        public async Task Test_rmdir()
        {
            var fs = Env.CreateFileSystem();

            // create env
            await fs.MakeDirAsync("/a/b/c1/d/e", true);
            await fs.MakeDirAsync("/a/b/c2/d2", true);
            await fs.MakeDirAsync("/a/b/c3/d3", true);

            var items = await fs.ListAsync("/a/b");
            Assert.AreEqual(3, items.Count);

            await fs.RemoveAsync("/a/b/c1/d/e", false);
            await Assert.ThrowsExceptionAsync<DirectoryNotFoundException>(() => fs.ListAsync("/a/b/c1/d/e"));

            await Assert.ThrowsExceptionAsync<IOException>(() => fs.RemoveAsync("/a/b", false)); // not found
            await fs.RemoveAsync("/a/b", true);
            await Assert.ThrowsExceptionAsync<DirectoryNotFoundException>(() => fs.ListAsync("/a/b"));
            items = await fs.ListAsync("/a");
            Assert.AreEqual(0, items.Count);
        }
    }
}