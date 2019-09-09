using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace ShotTrackService
{
    internal sealed class SQLAccess
    {
        // Database Connection
        private readonly SqlConnection _sqlConnection;
        private readonly IConfigurationRoot _configRoot;
        

        public SQLAccess()
        {
            //Get Connection string from Appsettings.Json
            var configBuild = new ConfigurationBuilder();
            configBuild.AddJsonFile("appsettings.json");
            _configRoot = configBuild.Build();

            var connString = _configRoot["Data:ConnectionStrings:AzureConnection"];
            _sqlConnection = new SqlConnection(connString);
        }

        private async Task OpenConnection()
        {
            await _sqlConnection.OpenAsync();
        }

          private void CloseConnection()
        {
            _sqlConnection.Close();
        }

        //Update Highscore 
        public async Task<string> ScoreUpdateCommand(string Uname, int score)
        {
            var check = "";
            try
            {
                await OpenConnection();

                using (var command = new SqlCommand("ScoreUpdate", _sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Username", Uname));
                    command.Parameters.Add(new SqlParameter("@Highscore", score));

                    command.ExecuteNonQuery();
                    check = "Successful Update ";
                }
            }
            finally
            {
                CloseConnection();
            }
            return check;
        }

        //Check Username availability within Database
        public async Task<string> VerifyUsernameCommand(string Uname)
        {
            var check = "";
            try
            {
                await OpenConnection();

                using (var command = new SqlCommand("UserCheck", _sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Username", Uname));

                    // Execute the SP and read the data returned from it
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                           check = Convert.ToString(reader["UserId"]);
                        }
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
            return check;
        }
        

        //Create New User in Database
        public async Task<string> NewUserCommand(string Uname, string Pword)
        {
            var createCheck = "";
            try
            {
                await OpenConnection();

                using (var command = new SqlCommand("UserCreation", _sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Username", Uname));
                    command.Parameters.Add(new SqlParameter("@Password", Pword));

                    command.ExecuteNonQuery();
                    createCheck = "Success";
                }

            }
            finally
            {
                CloseConnection();
            }

            return createCheck;
        }

        //Password check for Login
        public async Task<int> UserLoginCommand(string Uname, string Pword) ///return task here
        {
            var highScore = -1;

            try
            {
                // Open the connection
                await OpenConnection();

                // Build out the Stored Proc
                using (var command = new SqlCommand("UserLogin", _sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Username", Uname));
                    command.Parameters.Add(new SqlParameter("@Password", Pword));

                    // Execute the SP and read the data returned from it
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            highScore = Convert.ToInt32(reader["Highscore"]);
                        }
                    }
                }
            }
            finally
            {
                CloseConnection();
            }

            return highScore;
        }

        //Get Leaderboard
        public async Task<List<UserProp>> LeaderboardCommand()
        {
            List<UserProp> Leaders = new List<UserProp>();

            try
            {
                await OpenConnection();

                using (var command = new SqlCommand("LeaderBoard", _sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while(reader.Read())
                        {
                            Leaders.Add(new UserProp { UsernameProp = Convert.ToString(reader["Username"]), HighscoreProp = Convert.ToInt32(reader["Highscore"])});
                        }
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
            return Leaders;
        }
    }
}
