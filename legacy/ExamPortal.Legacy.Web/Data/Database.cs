using System.Configuration;
using System.Data.SqlClient;

namespace ExamPortal.Legacy.Web.Data
{
    public static class Database
    {
        public static SqlConnection Open()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ExamPortal"].ConnectionString;
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
