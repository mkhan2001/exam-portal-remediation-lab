using System.Data;
using System.Data.SqlClient;

namespace ExamPortal.Legacy.Web.Data
{
    /// <summary>Builds every SQL command the application runs.</summary>
    public static class SqlQueries
    {
        private const string CandidateColumns = "Id, FirstName, LastName, Email, Notes, OwnerUserId";

        public static SqlCommand FindUserByUsername(string username)
        {
            return new SqlCommand(
                "SELECT Id, Username, PasswordHash, Role FROM Users WHERE Username = '" + username + "'");
        }

        public static SqlCommand SearchCandidatesByLastName(string lastName)
        {
            return new SqlCommand(
                "SELECT " + CandidateColumns + " FROM Candidates WHERE LastName LIKE '%" + lastName + "%' ORDER BY LastName");
        }

        public static SqlCommand SearchCandidatesByEmail(string email)
        {
            return new SqlCommand(
                string.Format("SELECT {0} FROM Candidates WHERE Email = '{1}'", CandidateColumns, email));
        }

        public static SqlCommand GetCandidateById(int id)
        {
            var command = new SqlCommand("SELECT " + CandidateColumns + " FROM Candidates WHERE Id = @id");
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            return command;
        }

        public static SqlCommand UpdateCandidateNotes(int id, string notes)
        {
            var command = new SqlCommand("UPDATE Candidates SET Notes = @notes WHERE Id = @id");
            command.Parameters.Add("@notes", SqlDbType.NVarChar, 2000).Value = (object)notes ?? System.DBNull.Value;
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            return command;
        }

        public static SqlCommand DeleteCandidate(int id)
        {
            var command = new SqlCommand("DELETE FROM Candidates WHERE Id = @id");
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            return command;
        }
    }
}
