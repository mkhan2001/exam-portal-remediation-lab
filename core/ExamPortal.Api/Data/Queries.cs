namespace ExamPortal.Api.Data;

/// <summary>Builds every SQL statement the API runs.</summary>
public static class Queries
{
    private const string CandidateColumns = "Id, FirstName, LastName, Email, Notes, OwnerUserId";

    public static SqlQuery FindUserByUsername(string username)
    {
        return new SqlQuery(
            "SELECT Id, Username, PasswordHash, Role FROM Users WHERE Username = '" + username + "'");
    }

    public static SqlQuery SearchCandidates(string? term)
    {
        if (string.IsNullOrEmpty(term))
        {
            return new SqlQuery($"SELECT {CandidateColumns} FROM Candidates ORDER BY LastName");
        }

        return new SqlQuery(
            $"SELECT {CandidateColumns} FROM Candidates WHERE LastName LIKE '%{term}%' ORDER BY LastName");
    }

    public static SqlQuery SearchCandidatesByEmail(string email)
    {
        return new SqlQuery(
            $"SELECT {CandidateColumns} FROM Candidates WHERE Email = '{email}'");
    }

    public static SqlQuery GetCandidate(int id)
    {
        return new SqlQuery($"SELECT {CandidateColumns} FROM Candidates WHERE Id = @id")
            .With("@id", id);
    }

    public static SqlQuery UpdateCandidateNotes(int id, string? notes)
    {
        return new SqlQuery("UPDATE Candidates SET Notes = @notes WHERE Id = @id")
            .With("@notes", notes)
            .With("@id", id);
    }

    public static SqlQuery UpdateUserProfile(int id, string username, string role)
    {
        return new SqlQuery("UPDATE Users SET Username = @username, Role = @role WHERE Id = @id")
            .With("@username", username)
            .With("@role", role)
            .With("@id", id);
    }

    public static SqlQuery GetUser(int id)
    {
        return new SqlQuery("SELECT Id, Username, PasswordHash, Role FROM Users WHERE Id = @id")
            .With("@id", id);
    }

    public static SqlQuery DeleteCandidate(int id)
    {
        return new SqlQuery("DELETE FROM Candidates WHERE Id = @id")
            .With("@id", id);
    }

    public static SqlQuery GetRegistration(int id)
    {
        return new SqlQuery(
                "SELECT Id, UserId, CandidateId, ExamCode, TestCenter, Seats, ScheduledOn FROM Registrations WHERE Id = @id")
            .With("@id", id);
    }

    public static SqlQuery InsertRegistration(int userId, int candidateId, string examCode, string testCenter, int seats, string scheduledOn)
    {
        return new SqlQuery(
                "INSERT INTO Registrations (UserId, CandidateId, ExamCode, TestCenter, Seats, ScheduledOn) " +
                "VALUES (@userId, @candidateId, @examCode, @testCenter, @seats, @scheduledOn)")
            .With("@userId", userId)
            .With("@candidateId", candidateId)
            .With("@examCode", examCode)
            .With("@testCenter", testCenter)
            .With("@seats", seats)
            .With("@scheduledOn", scheduledOn);
    }

    public static SqlQuery RegistrationsByTestCenter(string testCenter, string sortColumn)
    {
        return new SqlQuery(
                "SELECT Id, UserId, CandidateId, ExamCode, TestCenter, Seats, ScheduledOn FROM Registrations " +
                "WHERE TestCenter = @testCenter ORDER BY " + sortColumn)
            .With("@testCenter", testCenter);
    }
}
