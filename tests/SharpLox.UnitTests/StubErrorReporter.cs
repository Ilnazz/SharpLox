using SharpLox.Errors;

namespace SharpLox.UnitTests;

public class StubErrorReporter : IErrorReporter
{
    public bool WasErrorOccured { get; private set; }

    public void ReportError(Error error) => WasErrorOccured = true;
}