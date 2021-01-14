using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wault.CS.Constants;

namespace Wault.CS.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual("Token", nameof(DocumentCreationResultType.Token));
        }
    }
}
