using ShotTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ShotTrack.Commands;
using ShotTrack.ViewModels;

namespace ShotTrack.ViewModels
{
    internal class UserLoginViewModel
    {
        static HttpClient client = new HttpClient { BaseAddress = new Uri("https://localhost:44354/") };

        public UserModel LogUser { get; set; }

        public static string CurrentUser = "";
        public static string UserHighScore = "0";
        public static string MessagePrompt = "";
        public static bool LoggedInCheck = false;

        public BaseRelayCommand LoginCommand { get; private set; }

        public PropertyModel LabelProp { get; set; }

        public UserLoginViewModel()
        {
            LogUser = new UserModel
            {
                UsernameProp = "",
                PasswordProp = ""
            };

            LabelProp = new PropertyModel
            {
                VisibilityProp = "Hidden",
                MsgTextProp = ""
            };

            LoginCommand = new BaseRelayCommand(CanGoLog, GoLog);
        }

        //LoginCommand Methods
        bool CanGoLog(object parameter)
        {
            return true;
        }
        async void GoLog(object parameter)
        {
            string userN = "";
            string userP = "";
            userN = LogUser.UsernameProp;
            userP = LogUser.PasswordProp;
            MessagePrompt = await LoginAsync(userN, userP);

            //Navigation to the game if User login is successful
            LabelProp.MsgTextProp = MessagePrompt;
            LabelProp.VisibilityProp = "Visible";

            //Check for the Successful Login
            if (MessagePrompt == "Success")
            {
                LabelProp.MsgTextProp = "Successful Login, Click the Play Game button to play.";
                //Sets the Login Check to enable the Game Button
                LoggedInCheck = true;
            }
            
        }
        //API Call
        static async Task<string> LoginUserAsync(UserModel LoginUser)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/User/Login", LoginUser);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> LoginAsync(string uName, string uPass)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            string errMsg = "";

            try
            {
                UserModel LogUser = new UserModel
                {
                    UsernameProp = uName,
                    PasswordProp = uPass
                };

                if (LogUser.UsernameProp == null || LogUser.PasswordProp == null)
                {
                    errMsg = "Please ensure both username and password fields are filled in";
                    return errMsg;
                }
                else
                {
                    //Send User information for Login check
                    var logResp = await LoginUserAsync(LogUser);

                    if (logResp == "\"\"" || logResp == "-1")
                    {
                        errMsg = "Invalid Username/Password combinaion. Please try again";
                        return errMsg;
                    }
                    else
                    {
                        // Puts current user and the score for them in public variables for the game page //
                        CurrentUser = LogUser.UsernameProp;
                        UserHighScore = logResp;
                        errMsg = "Success";
                        return errMsg;
                    }
                }
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return errMsg;
            }
        }
    }
}
