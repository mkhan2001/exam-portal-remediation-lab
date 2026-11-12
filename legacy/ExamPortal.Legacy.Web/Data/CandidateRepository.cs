using System.Collections.Generic;
using System.Data.SqlClient;
using ExamPortal.Legacy.Web.Models;

namespace ExamPortal.Legacy.Web.Data
{
    public class CandidateRepository
    {
        public List<Candidate> SearchByLastName(string lastName)
        {
            return ReadAll(SqlQueries.SearchCandidatesByLastName(lastName ?? string.Empty));
        }

        public List<Candidate> SearchByEmail(string email)
        {
            return ReadAll(SqlQueries.SearchCandidatesByEmail(email));
        }

        public Candidate GetById(int id)
        {
            var matches = ReadAll(SqlQueries.GetCandidateById(id));
            return matches.Count == 0 ? null : matches[0];
        }

        public void UpdateNotes(int id, string notes)
        {
            Execute(SqlQueries.UpdateCandidateNotes(id, notes));
        }

        public void Delete(int id)
        {
            Execute(SqlQueries.DeleteCandidate(id));
        }

        private static List<Candidate> ReadAll(SqlCommand command)
        {
            var candidates = new List<Candidate>();
            using (var connection = Database.Open())
            using (command)
            {
                command.Connection = connection;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        candidates.Add(new Candidate
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Notes = reader.IsDBNull(4) ? null : reader.GetString(4),
                            OwnerUserId = reader.GetInt32(5)
                        });
                    }
                }
            }
            return candidates;
        }

        private static void Execute(SqlCommand command)
        {
            using (var connection = Database.Open())
            using (command)
            {
                command.Connection = connection;
                command.ExecuteNonQuery();
            }
        }
    }
}
