using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MySqlConnector;
using ScanIT.Models;

namespace ScanIT
{

    public class DbConnectionManager
    {

        private readonly string _connectionString;

        public DbConnectionManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        //save new user
        public bool RegisterUser(string username, string password)
        {

            if (IsUsernameExisting(username))
                throw new ArgumentException("Username already exists. Please choose a different one.");

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = "INSERT INTO USERS (USERNAME, PASSWORD, IS_LOGGED) " +
                                  "VALUES (@username, @password, @isLogged)";

            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", EncryptPassword(password));
            command.Parameters.AddWithValue("@isLogged", 1);

            return command.ExecuteNonQuery() > 0;

        }

        //hash given password
        public string EncryptPassword(string password)
        {

            var hashedPassword = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));

            if (password.Length < 8 || password.Length > 20)
                throw new ArgumentException("Password must be between 8 and 20 characters long.");

            return hashedPassword;

        }


        //check if the username already exists
        private bool IsUsernameExisting(string username)
        {

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = "SELECT COUNT(*) " +
                                  "FROM USERS " +
                                  "WHERE Username = @username";

            command.Parameters.AddWithValue("@username", username);

            var result = command.ExecuteScalar();

            return Convert.ToInt32(result) > 0;

        }


        //look for user's password
        public string GetPasswordForUser(string username)
        {

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = "SELECT PASSWORD " +
                                  "FROM USERS " +
                                  "WHERE Username = @username";

            command.Parameters.AddWithValue("@username", username);

            var result = command.ExecuteScalar();

            return result as string;

        }


        //save info about logged user
        public bool UpdateIsLoggedStatus(string username, bool isLogged)
        {

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = "UPDATE USERS " +
                                  "SET IS_LOGGED = @isLogged " +
                                  "WHERE USERNAME = @username";

            command.Parameters.AddWithValue("@isLogged", isLogged ? 1 : 0);
            command.Parameters.AddWithValue("@username", username);

            return command.ExecuteNonQuery() > 0;

        }


        //validate login data
        public bool ValidateUserCredentials(string username, string password)
        {

            string hashedPassword = GetPasswordForUser(username);
            bool isUserLogged = IsUserLogged(username);

            if (hashedPassword == null)
                throw new ArgumentException("User does not exist.");

            if (isUserLogged)
                throw new ArgumentException("User is already logged in.");

            string hashedInputPassword = EncryptPassword(password);

            return hashedPassword.Equals(hashedInputPassword);

        }


        //check if user is already logged in
        private bool IsUserLogged(string username)
        {

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = "SELECT IS_LOGGED " +
                                  "FROM USERS " +
                                  "WHERE Username = @username";

            command.Parameters.AddWithValue("@username", username);

            var result = command.ExecuteScalar();

            return Convert.ToInt32(result) == 1;

        }


        //check if the same code was saved into db without 10 seconds interval
        public bool IsDuplicateScan(string code, string username)
        {

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = "SELECT COUNT(*) " +
                                  "FROM SCANNING_HISTORY, USERS " +
                                  "WHERE CODE = @code AND SCANNING_HISTORY.USER_ID = USERS.USER_ID " +
                                  "AND USERS.USERNAME = @username AND SCANNING_DATE_TIME > DATE_SUB(NOW(), INTERVAL 10 SECOND)";

            command.Parameters.AddWithValue("@code", code);
            command.Parameters.AddWithValue("@username", username);

            var result = command.ExecuteScalar();

            return Convert.ToInt32(result) > 0;

        }


        //save scan into scanning_history table
        public bool InsertScanningHistory(string code, string codeType, string username)
        {

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var getUserIdCommand = connection.CreateCommand();

            getUserIdCommand.CommandText = "SELECT USER_ID " +
                                           "FROM USERS " +
                                           "WHERE USERNAME = @username";

            getUserIdCommand.Parameters.AddWithValue("@username", username);

            var userId = getUserIdCommand.ExecuteScalar();

            var insertCommand = connection.CreateCommand();

            insertCommand.CommandText = "INSERT INTO SCANNING_HISTORY (CODE, CODE_TYPE, USER_ID, SCANNING_DATE_TIME) " +
                                        "VALUES (@code, @codeType, @userId, NOW())";

            insertCommand.Parameters.AddWithValue("@code", code);
            insertCommand.Parameters.AddWithValue("@codeType", codeType);
            insertCommand.Parameters.AddWithValue("@userId", userId);

            return insertCommand.ExecuteNonQuery() > 0;

        }


        //insert scan into favourites table
        public bool InsertFavourites(string code, string codeType, string username)
        {

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var getUserIdCommand = connection.CreateCommand();

            getUserIdCommand.CommandText = "SELECT USER_ID " +
                                           "FROM USERS " +
                                           "WHERE USERNAME = @username";

            getUserIdCommand.Parameters.AddWithValue("@username", username);

            var userId = getUserIdCommand.ExecuteScalar();

            var insertCommand = connection.CreateCommand();

            insertCommand.CommandText = "INSERT INTO FAVOURITES (CODE, CODE_TYPE, USER_ID) " +
                                        "VALUES (@code, @codeType, @userId)";

            insertCommand.Parameters.AddWithValue("@code", code);
            insertCommand.Parameters.AddWithValue("@codeType", codeType);
            insertCommand.Parameters.AddWithValue("@userId", userId);

            return insertCommand.ExecuteNonQuery() > 0;

        }


        //get data from scanning_history table
        public List<ScanHistoryItem> GetScanHistory(string username)
        {

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = "SELECT SH.CODE, SH.CODE_TYPE, SH.SCANNING_DATE_TIME, EC.EAN_DESCRIPTION " +
                                  "FROM SCANNING_HISTORY SH " +
                                  "INNER JOIN USERS U ON SH.USER_ID = U.USER_ID " +
                                  "LEFT JOIN EAN_CODES EC ON SH.CODE = EC.EAN_CODE " +
                                  "WHERE U.USERNAME = @username " +
                                  "ORDER BY SH.SCANNING_DATE_TIME DESC";

            command.Parameters.AddWithValue("@username", username);

            var reader = command.ExecuteReader();
            var historyItems = new List<ScanHistoryItem>();

            while (reader.Read())
            {

                var item = new ScanHistoryItem
                {

                    Code = reader.GetString(0),
                    CodeType = reader.GetString(1),
                    DateTime = reader.GetDateTime(2),
                    Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)

                };

                historyItems.Add(item);

            }

            return historyItems;

        }


        //check if the code is already added to the favourites by the given user
        public bool IsAddedToFavourites(string code, string username)
        {

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = "SELECT COUNT(*) FROM FAVOURITES, USERS " +
                                  "WHERE CODE = @code AND FAVOURITES.USER_ID = USERS.USER_ID AND USERS.USERNAME = @username";

            command.Parameters.AddWithValue("@code", code);
            command.Parameters.AddWithValue("@username", username);

            var result = command.ExecuteScalar();

            return Convert.ToInt32(result) > 0;

        }


        //get data from favourites table
        public List<FavouriteItem> GetFavouriteItems(string username)
        {

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = "SELECT CODE, CODE_TYPE, EC.EAN_DESCRIPTION " +
                                  "FROM FAVOURITES F " +
                                  "INNER JOIN USERS U ON F.USER_ID = U.USER_ID " +
                                  "LEFT JOIN EAN_CODES EC ON F.CODE = EC.EAN_CODE " +
                                  "WHERE U.USERNAME = @username " +
                                  "ORDER BY ID DESC";

            command.Parameters.AddWithValue("@username", username);

            var reader = command.ExecuteReader();
            var favouriteItems = new List<FavouriteItem>();

            while (reader.Read())
            {

                var item = new FavouriteItem
                {

                    Code = reader.GetString(0),
                    CodeType = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)

                };

                favouriteItems.Add(item);

            }

            return favouriteItems;

        }


        //remove item from favourites table
        public void RemoveFavouriteItem(string code, string username)
        {

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var getUserIdCommand = connection.CreateCommand();

            getUserIdCommand.CommandText = "SELECT USER_ID " +
                                           "FROM USERS " +
                                           "WHERE USERNAME = @username";

            getUserIdCommand.Parameters.AddWithValue("@username", username);

            var userId = getUserIdCommand.ExecuteScalar();

            var removeCommand = connection.CreateCommand();

            removeCommand.CommandText = "DELETE FROM FAVOURITES " +
                                        "WHERE CODE = @code AND USER_ID = @userId";

            removeCommand.Parameters.AddWithValue("@code", code);
            removeCommand.Parameters.AddWithValue("@userId", userId);

            removeCommand.ExecuteNonQuery();

        }


        //get description of ean code from EAN_CODES table
        public string GetDescriptionFromDatabase(string code)
        {

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = "SELECT EAN_DESCRIPTION " +
                                  "FROM EAN_CODES " +
                                  "WHERE EAN_CODE = @code";

            command.Parameters.AddWithValue("@code", code);

            var result = command.ExecuteScalar();

            return result as string;

        }


        //update ean description
        public void UpdateDescription(string code, string codeType, string newDescription, string username)
        {

            var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var getUserIdCommand = connection.CreateCommand();

            getUserIdCommand.CommandText = "SELECT USER_ID " +
                                           "FROM USERS " +
                                           "WHERE USERNAME = @username";

            getUserIdCommand.Parameters.AddWithValue("@username", username);

            var userId = getUserIdCommand.ExecuteScalar();

            var oldDescription = GetDescriptionFromDatabase(code);

            if (!string.IsNullOrEmpty(oldDescription))
            {

                var updateCommand = connection.CreateCommand();

                updateCommand.CommandText = "UPDATE EAN_CODES " +
                                            "SET EAN_DESCRIPTION = @newDescription, EAN_DESCRIPTION_AUTHOR_ID = @userId " +
                                            "WHERE EAN_CODE = @code";

                updateCommand.Parameters.AddWithValue("@newDescription", newDescription);
                updateCommand.Parameters.AddWithValue("@userId", userId);
                updateCommand.Parameters.AddWithValue("@code", code);

                updateCommand.ExecuteNonQuery();

            }

            else
            {

                var updateCommand = connection.CreateCommand();

                updateCommand.CommandText = "INSERT INTO EAN_CODES (EAN_CODE, EAN_DESCRIPTION, EAN_DESCRIPTION_AUTHOR_ID)" +
                                            "VALUES (@code, @newDescription, @userId)";

                updateCommand.Parameters.AddWithValue("@newDescription", newDescription);
                updateCommand.Parameters.AddWithValue("@userId", userId);
                updateCommand.Parameters.AddWithValue("@code", code);

                updateCommand.ExecuteNonQuery();

            }

            var insertCommand = connection.CreateCommand();

            insertCommand.CommandText = "INSERT INTO DESCRIPTION_CHANGES (CODE, CODE_TYPE, USER_ID, OLD_DESCRIPTION, NEW_DESCRIPTION, DESCRIPTION_CHANGE_DATE_TIME) " +
                                        "VALUES (@code, @codeType, @userId, @oldDescription, @newDescription, NOW())";

            insertCommand.Parameters.AddWithValue("@code", code);
            insertCommand.Parameters.AddWithValue("@codeType", codeType);
            insertCommand.Parameters.AddWithValue("@userId", userId);
            insertCommand.Parameters.AddWithValue("@oldDescription", oldDescription);
            insertCommand.Parameters.AddWithValue("@newDescription", newDescription);

            insertCommand.ExecuteNonQuery();

        }

    }

}