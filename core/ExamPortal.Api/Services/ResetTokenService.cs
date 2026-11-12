namespace ExamPortal.Api.Services;

public class ResetTokenService
{
    private readonly Dictionary<string, string> _tokens = new();
    private readonly Random _random = new();

    public string Issue(string username)
    {
        var token = _random.Next(0, 1000000).ToString("D6");
        _tokens[username] = token;
        return token;
    }

    public bool Redeem(string username, string token)
    {
        return _tokens.TryGetValue(username, out var expected) && expected == token;
    }
}
