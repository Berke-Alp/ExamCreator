using ExamCreator.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace ExamCreator
{
    /// <summary>
    /// The class that handles all database operations
    /// </summary>
    public class DBOps
    {
        private static string connectionString;
        /// <summary>
        /// The connection string used to connect to the database
        /// </summary>
        public static string ConnectionString { get => connectionString; set => connectionString = value; }

        /// <summary>
        /// Checks if a user exists with given username and password.
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <param name="password">Password to check</param>
        /// <param name="user">User's data to return (null if returned value is false)</param>
        /// <returns><see langword="true"/> if username and password matches with a user, else <see langword="false"/>.</returns>
        public static bool CheckUser(string username, string password, out User user)
        {
            // Set user to null first
            user = null;

            // Prepare the query
            SqliteConnection con = new(ConnectionString);
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM users WHERE username = @username LIMIT 1";
            cmd.Parameters.AddWithValue("username", username);

            con.Open();

            var r = cmd.ExecuteReader();

            // If no match with username, return false
            if (!r.HasRows) return false;

            while (r.Read())
            {
                User u = new()
                {
                    Id = r.GetInt32(0),
                    Username = r.GetString(1),
                    Name = r.GetString(4),
                    Surname = r.GetString(5)
                };

                // Get password and salt from database, verify the hash from given password and the original password hash
                string pword = r.GetString(2);
                byte[] salt = Convert.FromBase64String(r.GetString(3));
                bool verify = Crypto.VerifyHash(password, salt, Convert.FromBase64String(pword));

                // If not verified, return false
                if (!verify) return false;

                // Finally, fill user's data
                user = u;
            }

            r.Close();
            con.Close();
            r.Dispose();
            con.Dispose();

            return true;
        }

        /// <summary>
        /// Gets last 5 posts from the database
        /// </summary>
        /// <returns>Last 5 posts</returns>
        public static List<Post> GetPosts()
        {
            SqliteConnection con = new(ConnectionString);
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM posts ORDER BY publishDate DESC LIMIT 5";

            List<Post> list = new();

            con.Open();

            var r = cmd.ExecuteReader();
            while (r.Read())
            {
                Post p = new()
                {
                    Id = r.GetInt32(0),
                    PublishDate = r.GetDateTime(1),
                    Title = r.GetString(2),
                    Link = r.GetString(3),
                    Description = r.GetString(4),
                    Content = r.IsDBNull(5) ? string.Empty : r.GetString(5)
                };
                list.Add(p);
            }

            r.Close();
            con.Close();
            r.Dispose();
            con.Dispose();

            return list;
        }

        /// <summary>
        /// Gets last 5 posts without exam
        /// </summary>
        /// <returns>Last 5 posts without exam</returns>
        public static List<Post> GetPostsWithOutExams()
        {
            SqliteConnection con = new(ConnectionString);
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM posts WHERE NOT EXISTS (SELECT * FROM exams WHERE posts.id = exams.id) ORDER BY publishDate DESC LIMIT 5";

            List<Post> list = new();

            con.Open();

            var r = cmd.ExecuteReader();
            while (r.Read())
            {
                Post p = new()
                {
                    Id = r.GetInt32(0),
                    PublishDate = r.GetDateTime(1),
                    Title = r.GetString(2),
                    Link = r.GetString(3),
                    Description = r.GetString(4),
                    Content = r.IsDBNull(5) ? string.Empty : r.GetString(5)
                };
                list.Add(p);
            }

            r.Close();
            con.Close();
            r.Dispose();
            con.Dispose();

            return list;
        }

        /// <summary>
        /// Returns a specific post from the database
        /// </summary>
        /// <param name="id">The ID of the post</param>
        /// <returns><see cref="Post"/></returns>
        public static Post GetPost(int id)
        {
            SqliteConnection con = new(ConnectionString);
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM posts WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);

            Post p = null;

            con.Open();

            var r = cmd.ExecuteReader();
            while (r.Read())
            {
                p = new Post()
                {
                    Id = r.GetInt32(0),
                    PublishDate = r.GetDateTime(1),
                    Title = r.GetString(2),
                    Link = r.GetString(3),
                    Description = r.GetString(4),
                    Content = r.IsDBNull(5) ? string.Empty : r.GetString(5)
                };
            }

            r.Close();
            con.Close();
            r.Dispose();
            con.Dispose();

            return p;
        }

        public static void UpdatePost(Post p)
        {
            SqliteConnection con = new(ConnectionString);
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE posts SET `content` = @c WHERE `id` = @id";
            cmd.Parameters.AddWithValue("id", p.Id);
            cmd.Parameters.AddWithValue("c", p.Content);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            con.Dispose();
        }

        public static List<Exam> GetExams()
        {
            SqliteConnection con = new(ConnectionString);
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM exams";

            List<Exam> list = new();

            con.Open();

            var r = cmd.ExecuteReader();
            while (r.Read())
            {
                Exam e = new()
                {
                    Id = r.GetInt32(0),
                    Title = r.GetString(1),
                    Content = r.GetString(2),
                    QnA = r.GetString(3),
                    CreatedAt = r.GetDateTime(4)
                };
                list.Add(e);
            }

            r.Close();
            con.Close();
            r.Dispose();
            con.Dispose();

            return list;
        }

        public static Exam GetExam(int id)
        {
            SqliteConnection con = new(ConnectionString);
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM exams WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);

            Exam e = null;

            con.Open();

            var r = cmd.ExecuteReader();
            while (r.Read())
            {
                e = new Exam()
                {
                    Id = r.GetInt32(0),
                    Title = r.GetString(1),
                    Content = r.GetString(2),
                    QnA = r.GetString(3),
                    CreatedAt = r.GetDateTime(4)
                };
            }

            r.Close();
            con.Close();
            r.Dispose();
            con.Dispose();

            return e;
        }

        public static async void DeleteExam(int id)
        {
            SqliteConnection con = new(ConnectionString);
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM exams WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            await con.DisposeAsync();
        }

        public static void CreateExam(Exam exam)
        {
            SqliteConnection con = new(ConnectionString);
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = "INSERT INTO exams(`id`,`title`,`content`,`QnA`,`createdAt`) VALUES(@id,@t,@c,@qna,@ca)";
            cmd.Parameters.AddWithValue("id", exam.Id);
            cmd.Parameters.AddWithValue("t", exam.Title);
            cmd.Parameters.AddWithValue("c", exam.Content);
            cmd.Parameters.AddWithValue("qna", exam.QnA);
            cmd.Parameters.AddWithValue("ca", exam.CreatedAt);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            con.Dispose();
        }
    }
}
