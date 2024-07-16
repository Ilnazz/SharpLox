using SharpLox.Errors;
using SharpLox.Scanning;

namespace SharpLox.UnitTests
{
    [TestClass]
    public class ScannerTests
    {
        private IErrorReporter _errorReporter = null!;

        [TestInitialize]
        public void TestInitialize() => _errorReporter = new StubErrorReporter();

        [TestCleanup]
        public void TestCleanup() => _errorReporter = null!;

        [TestMethod]
        public void ReturnsTerminatorTokenWhenSourceCodeIsEmpty()
        {
            var sourceCode = string.Empty;
            var scanner = new Scanner(_errorReporter, sourceCode);

            var tokenArray = scanner.ScanTokens().ToArray();

            Assert.AreEqual(1, tokenArray.Length);
        }
    }
}