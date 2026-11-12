using ExamPortal.Api.Services;
using Xunit;

namespace ExamPortal.Api.Tests;

public class ArchiveServiceTests
{
    [Fact]
    public void BuildCommand_IncludesTheArchiveName()
    {
        var startInfo = new ArchiveService().BuildCommand("july-export");

        Assert.Contains("exports/july-export.tgz", startInfo.Arguments);
    }
}
