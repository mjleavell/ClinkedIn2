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

        public User AddUser(string username, string password, DateTime releaseDate, int age, bool isPrisoner)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var insertUserCommand = connection.CreateCommand();
                insertUserCommand.CommandText = $@"Insert into Users (username,password, releaseDate, age, isPrisoner)
                                            Output inserted.*
                                            Values(@username,@password,@releaseDate,@age,@isPrisoner)";

                insertUserCommand.Parameters.AddWithValue("username", username);
                insertUserCommand.Parameters.AddWithValue("password", password);
                insertUserCommand.Parameters.AddWithValue("releaseDate", releaseDate);
                insertUserCommand.Parameters.AddWithValue("age", age);
                insertUserCommand.Parameters.AddWithValue("isPrisoner", isPrisoner);

                var reader = insertUserCommand.ExecuteReader();

                if (reader.Read())
                {
                    var insertedPassword = reader["password"].ToString();
                    var insertedUsername = reader["username"].ToString();
                    var insertedReleaseDate = (DateTime)reader["releaseDate"];
                    var insertedAge = (int)reader["age"];
                    var insertedId = reader["Id"].ToString();
                    var insertedIsPrisoner = (bool)reader["isPrisoner"];

                    var newUser = new User(insertedUsername, insertedPassword, insertedReleaseDate, insertedAge, insertedIsPrisoner) { Id = insertedId };

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
                //var users = new List<User>(); 
                //var connection = new SqlConnection(ConnectionString);
                //connection.Open();

                //var getAllUsersCommand = connection.CreateCommand(); 
                //getAllUsersCommand.CommandText = "select * from users";

                //var reader = getAllUsersCommand.ExecuteReader(); 

                //while (reader.Read())
                //{
                //    var id = reader["Id"].ToString(); 
                //    var username = reader["username"].ToString();
                //    var password = reader["password"].ToString();
                //    var releaseDate = (DateTime)reader["releaseDate"];
                //    var age = (int)reader["age"];
                //    var isPrisoner = (bool)reader["isPrisoner"];
                //    var user = new User(username, password, releaseDate, age, isPrisoner) { Id = id };

                //    users.Add(user);
                //}

                //connection.Close();

                //return users;
            }

        public User GetSingleUser(int userId)
        {
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

                var singleUserId = reader["id"];
                var singleUserUsername = reader["username"].ToString();
                var singleUserPassword = reader["password"].ToString();
                var singleUserReleaseDate = (DateTime)reader["releaseDate"];
                var singleUserAge = (int)reader["age"];
                var singleUserIsPrisoner = (bool)reader["isPrisoner"];

                var singleUser = new User(singleUserUsername, singleUserPassword, singleUserReleaseDate, singleUserAge, singleUserIsPrisoner) { Id = singleUserId };
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

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var updateUserCommand = connection.CreateCommand();
                updateUserCommand.CommandText = $@"Update Users
                                                Set IsPrisoner = @isPrisoner
                                                where id = @id";

                updateUserCommand.Parameters.AddWithValue("@id", id);
                updateUserCommand.Parameters.AddWithValue("@isPrisoner", isPrisoner);
                var reader = updateUserCommand.ExecuteReader();
                return isPrisoner;
            }

            throw new Exception("No user found");
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
