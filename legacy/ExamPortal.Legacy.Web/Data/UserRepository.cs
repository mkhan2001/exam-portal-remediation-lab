using ExamPortal.Legacy.Web.Models;

namespace ExamPortal.Legacy.Web.Data
{
    public class UserRepository
    {
        public User FindByUsername(string username)
        {
            using (var connection = Database.Open())
            using (var command = SqlQueries.FindUserByUsername(username))
            {
                command.Connection = connection;
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new User
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        PasswordHash = reader.GetString(2),
                        Role = reader.GetString(3)
                    };
                }
            }
        }
    }
}
