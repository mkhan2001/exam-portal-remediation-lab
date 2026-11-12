using ExamPortal.Api.Data;
using Xunit;

namespace ExamPortal.Api.Tests;

public class QueriesTests
{
    [Fact]
    public void GetCandidate_PassesIdAsParameter()
    {
        var query = Queries.GetCandidate(42);

        Assert.Contains("@id", query.Text);
        Assert.Equal(42, query.Parameters["@id"]);
    }

    [Fact]
    public void GetRegistration_PassesIdAsParameter()
    {
        var query = Queries.GetRegistration(7);

        Assert.Contains("@id", query.Text);
        Assert.Equal(7, query.Parameters["@id"]);
    }

    [Fact]
    public void SearchCandidates_WithNoTerm_ReturnsAllCandidates()
    {
        var query = Queries.SearchCandidates(null);

        Assert.DoesNotContain("WHERE", query.Text);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void InsertRegistration_PassesEveryValueAsParameter()
    {
        var query = Queries.InsertRegistration(1, 2, "BAR-CA-2026", "Oakland", 1, "2026-07-28");

        Assert.Equal(6, query.Parameters.Count);
        Assert.DoesNotContain("BAR-CA-2026", query.Text);
        Assert.DoesNotContain("Oakland", query.Text);
    }
}
