
using ShotTrack.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ShotTrack.ViewModels;
using ShotTrack.Commands;

namespace ShotTrack.ViewModels
{
    public class UserCreationViewModel
    {
        //Work Around to avoid exception thrown when calling HttpClient multiple times within a page
        static HttpClient client = new HttpClient {BaseAddress = new Uri("https://localhost:44354/")};

        public UserModel NewUser { get; set; }
        public PropertyModel LabelProp { get; set; }
        public BaseRelayCommand CreateUserCommand { get; private set; }
        public static string MessageLabel = "";

        public UserCreationViewModel()
        {
            NewUser = new UserModel
            {
                UsernameProp = "",
                PasswordProp = ""
            };

            LabelProp = new PropertyModel
            {
                VisibilityProp = "Hidden",
                MsgTextProp = ""
            };
            CreateUserCommand = new BaseRelayCommand(CanCreateUser, GoCreateUser);
        }

        //Methods for the CreateUserCommand
        bool CanCreateUser(object parameter)
        {
            return true;
        }
        async void GoCreateUser(object parameter)
        {
            string userN = "";
            string userP = "";
            userN = NewUser.UsernameProp;
            userP = NewUser.PasswordProp;
            MessageLabel = await RunAsync(userN, userP);
            //Show Message to User to notify on what to do next
            LabelProp.MsgTextProp = MessageLabel;
            LabelProp.VisibilityProp = "Visible";

            if (MessageLabel == "User Created")
            {
                LabelProp.MsgTextProp = "User has been created. Proceed to Login Page.";
            }
        }
            
        //API Call
        static async Task<Uri> CreateUserAsync(UserModel NewUser)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/User/Create", NewUser);
            response.EnsureSuccessStatusCode();
            return response.Headers.Location;
        }
        
        //API Call
        static async Task<string> CheckUserAsync(UserModel NewUser)
        {
            HttpResponseMessage respo = await client.PostAsJsonAsync("api/User/Verify", NewUser);
            respo.EnsureSuccessStatusCode();
            return await respo.Content.ReadAsStringAsync();
        }

        public static async Task<string> RunAsync(string uName, string uPass)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            // Above tells server data should be sent in JSON

            string errMsg;

            try
            {
                //Null Value Check
                if (uName == "" || uPass == "")
                {
                    errMsg = "Username and Password fields can't be empty";
                    return errMsg;
                }
                //Length Check
                else if (uPass.Length > 30 || uName.Length > 30)
                {
                    errMsg = "Username and Password must be less than 30 characters";
                    return errMsg;
                }
                else
                {
                    UserModel newUser = new UserModel
                    {
                        UsernameProp = uName,
                        PasswordProp = uPass
                    };

                    // Call The User Check First
                    var rep = "";
                    rep = await CheckUserAsync(newUser);
                    string test = rep.ToString();

                    //If user exists then exit and show prompt that the username already exists
                    if (rep.ToString() == "\"\"")
                    {
                        //If Valid Username then Create User in DB
                        var respMsg = await CreateUserAsync(newUser);
                        return errMsg = "User Created";
                    }
                    else
                    {
                        //throw prompt
                        return errMsg = "Username already exists, Please choose another Username";
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
