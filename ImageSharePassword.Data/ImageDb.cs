using System.Collections.Generic;
using System.Data.SqlClient;

namespace ImageSharePassword.Data
{
    public class ImageDb
    {
        private string _connectionString;

        public ImageDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Add(Image image)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Images (FileName, Password, Views, UserId) " +
                                  "VALUES (@fileName, @password, 0, @userId) SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@fileName", image.FileName);
                cmd.Parameters.AddWithValue("@password", image.Password);
                cmd.Parameters.AddWithValue("@userId", image.UserId);
                connection.Open();
                image.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public Image GetById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 * FROM Images WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }

                return new Image
                {
                    Id = (int)reader["Id"],
                    Password = (string)reader["Password"],
                    FileName = (string)reader["FileName"],
                    Views = (int)reader["Views"]
                };
            }
        }

        public void IncrementViewCount(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE Images SET Views = Views + 1 WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<Image> GetUserImages(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT i.* FROM Images i JOIN Users u " +
                                  "ON i.UserId = u.Id " +
                                  "WHERE u.Email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                connection.Open();
                var reader = cmd.ExecuteReader();
                var images = new List<Image>();
                while (reader.Read())
                {
                    images.Add(new Image
                    {
                        Id = (int)reader["Id"],
                        Password = (string)reader["Password"],
                        FileName = (string)reader["FileName"],
                        Views = (int)reader["Views"]
                    });
                }
                return images;
            }
        }

        public void DeleteImage(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Images WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}