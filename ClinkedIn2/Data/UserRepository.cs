using ClinkedIn2.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Linq;

namespace ClinkedIn2.Data
{
    public class UserRepository
    {
        const string ConnectionString = "Server = localhost; Database = ClinkedIn; Trusted_Connection = True;";

        //static List<User> _users;
        static List<User> _users = new List<User>
        {
        };

        static List<string> _intrest = new List<string> {
            "Killing",
            "Hair Braiding",
            "Murder",
            "Drinking",
            "Drugs"
        };

        public string ReadInterestList()
        {
            var listOfIntrest = " ";
            foreach (string intrestThing in _intrest)
            {
                listOfIntrest += intrestThing + (" , ");
            }
            return listOfIntrest;
        }

        public User AddUser(string username, string password, DateTime releaseDate, int age)
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                var repository = new UserRepository();

                var insertQuery = @"
                        INSERT INTO [dbo].[Users]
                                   ([Username]
                                   ,[Password]
                                   ,[ReleaseDate]
                                   ,[Age]
                                   ,[IsPrisoner])
                        OUTPUT inserted.*
                             VALUES
                                   (@username
		                           ,@password
		                           ,@releaseDate
                                   ,@age
                                   ,@isPrisoner)";

                var parameters = new
                {
                    Username = username,
                    Password = password,
                    ReleaseDate = releaseDate,
                    Age = age,
                    IsPrisoner = true
                };

                var newUser = db.QueryFirstOrDefault<User>(insertQuery, parameters);

                if (newUser != null)
                {
                    return newUser;
                }
            }

            throw new Exception("No user found");
        }

        public IEnumerable<User> GetAllUsers()
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                var getQuery = "SELECT * FROM Users";

                var users = db.Query<User>(getQuery).ToList();

                return users;
            }
        }

        public User GetSingleUser(int userId)
        {
            //using (var db = new SqlConnection(ConnectionString))
            //{
            //    var singleQuery = @"SELECT *5
            //                        FROM users
            //                        WHERE id = @userId";

            //    var singleUser = db.QueryFirstOrDefault<User>(singleQuery);

            //    return singleUser;
            //}
            //throw new Exception("No user found");
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var getSingleUserCommand = connection.CreateCommand();
                getSingleUserCommand.CommandText = @"SELECT *
                                                    FROM users u
                                                    WHERE u.id = @userId";

                getSingleUserCommand.Parameters.AddWithValue("@userId", userId);
                var reader = getSingleUserCommand.ExecuteReader();

                // if no users were returned, an error is thrown.
                if (!reader.Read())
                {
                    throw new InvalidOperationException("No user were returned.");
                }

                var singleUserId = (int)reader["id"];
                var singleUserUsername = reader["username"].ToString();
                var singleUserPassword = reader["password"].ToString();
                var singleUserReleaseDate = (DateTime)reader["releaseDate"];
                var singleUserAge = (int)reader["age"];
                var singleUserIsPrisoner = (bool)reader["isPrisoner"];

                var singleUser = new User()
                {
                    Id = singleUserId,
                    Username = singleUserUsername,
                    Password = singleUserPassword,
                    ReleaseDate = singleUserReleaseDate,
                    Age = singleUserAge,
                    IsPrisoner = singleUserIsPrisoner
                };

                return singleUser;
            }
            throw new Exception("No user found");
        }

        public bool UpdateIsPrisoner(int id)
        {
            var user = GetSingleUser(id);
            var isPrisoner= user.IsPrisoner;

            // if isPrisoner is true
            if (isPrisoner)
            {
                isPrisoner = false;
            }
            else
            {
                isPrisoner = true;
            }

            using (var db = new SqlConnection(ConnectionString))
            {
                var updateQuery = @"UPDATE Users
                                    SET IsPrisoner = @isPrisoner
                                    WHERE id = @id";

                var rowsAffected = db.ExecuteNonQuery(updateQuery, isPrisoner);

                if (rowsAffected == 1)
                    return isPrisoner;
            }

            throw new Exception("Could not update isPrisoner for the user");
        }

        public void DeleteUser(int userId)
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                var parameter = new { Id = userId };

                var deleteQuery = "DELETE From Users WHERE Id = @id";

                var rowsAffected = db.Execute(deleteQuery, parameter);

                if (rowsAffected != 1)
                {
                    throw new Exception("No user found");
                }
            }
        }
    }
}
