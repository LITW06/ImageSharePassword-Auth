using System.Data.SqlClient;

namespace ImageSharePassword.Data
{
    public class AuthDb
    {
        private readonly string _connectionString;

        public AuthDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user, string password)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) " +
                                  "VALUES (@name, @email, @passwordHash)";
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public User GetByEmail(string email)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }

                return new User
                {
                    Email = email,
                    Name = (string)reader["Name"],
                    Id = (int)reader["Id"],
                    PasswordHash = (string)reader["PasswordHash"]
                };
            }
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isCorrectPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (isCorrectPassword)
            {
                return user;
            }

            return null;
        }
    }
}