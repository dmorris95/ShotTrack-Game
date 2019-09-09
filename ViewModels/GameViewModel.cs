using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ShotTrack.Commands;
using ShotTrack.Models;
using ShotTrack.ViewModels;

namespace ShotTrack.ViewModels
{
    internal class GameViewModel
    {
        static HttpClient client = new HttpClient { BaseAddress = new Uri("https://localhost:44354/") };

        public UserModel CurrentUser {get; set; }
        public ImageModel TargetImage { get; set; }
        public ImageModel ProjectileImage { get; set; }
        public GameSession CurrentUserScore { get; set; }
        public PropertyModel MessageProp { get; set; }

        private string DirectIdentifier = "Right"; //Identifies what direction the target is moving in

        //Relay Commands for the game
        public BaseRelayCommand StartGameCommand { get; private set; }
        public BaseRelayCommand LeftKeyCommand { get; private set; }
        public BaseRelayCommand RightKeyCommand { get; private set; }
        public BaseRelayCommand UpKeyCommand { get; private set; }
        
        //Intialize timers for the target and the projectile
        DispatcherTimer gameTicker = new DispatcherTimer();
        DispatcherTimer upTicker = new DispatcherTimer();
        
        public GameViewModel()
        {
            //Get Current User info
            CurrentUser = new UserModel
            {
                UsernameProp = UserLoginViewModel.CurrentUser,
                HighscoreProp = Convert.ToInt32(UserLoginViewModel.UserHighScore)
            };

            TargetImage = new ImageModel
            {
                ImageLeft = 0,
                ImageTop = 0
            };

            ProjectileImage = new ImageModel
            {
                ImageLeft = 400,
                ImageTop = 400
            };

            CurrentUserScore = new GameSession
            {
                CurScore = 0
            };

            MessageProp = new PropertyModel
            {
                VisibilityProp = "Hidden",
                MsgTextProp = ""
            };

            StartGameCommand = new BaseRelayCommand(CanStart, Start);
            LeftKeyCommand = new BaseRelayCommand(CanGoLeft, GoLeft);
            RightKeyCommand = new BaseRelayCommand(CanGoRight, GoRight);
            UpKeyCommand = new BaseRelayCommand(CanGoUp, GoUp);

            gameTicker.Tick += GameTicker_Tick;
            upTicker.Tick += UpTicker_TickAsync;
        }

        //StartCommand Methods
        void Start(object parameter)
        {
            StartGame();
        }
        bool CanStart(object parameter)
        {
            return true;
        }

        //LeftCommand Methods
        void GoLeft(object parameter)
        {
            //Won't let the User move the projectile off the screen
            if (ProjectileImage.ImageLeft == 0)
            {
                ProjectileImage.ImageLeft = 0;
            }
            else
            {
                ProjectileImage.ImageLeft = ProjectileImage.ImageLeft - 10;
            }
        }
        bool CanGoLeft(object parameter)
        {
            //Ensure player can't move object once it has been shot
            if (ProjectileImage.ImageTop < 400 || gameTicker.IsEnabled == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //RightCommand Methods
        void GoRight(object parameter)
        {
            //Won't let user move the projectile off the screen
            if (ProjectileImage.ImageLeft == 950)
            {
                ProjectileImage.ImageLeft = 950;
            }
            else
            {
                ProjectileImage.ImageLeft = ProjectileImage.ImageLeft + 10;
            }
        }
        bool CanGoRight(object parameter)
        {
            //Ensure user can't move object once it has been shot
            if (ProjectileImage.ImageTop < 400 || gameTicker.IsEnabled == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //UpCommand Methods
        void GoUp(object parameter)
        {
            MessageProp.VisibilityProp = "Hidden";
            MessageProp.MsgTextProp = ""; //Resets Message once User shoots the Projectile again
        
            //Enable the Object to move up
            upTicker.Interval = TimeSpan.FromMilliseconds(5);
            upTicker.IsEnabled = true;
        }
        bool CanGoUp(object parameter)
        {
            //If the object is already moving up it won't let it be shot again until it has finished
            if (upTicker.IsEnabled == true || gameTicker.IsEnabled == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //Firing Logic for the Projectile
        private async void UpTicker_TickAsync(object sender, EventArgs e)
        {
            string msg; //Placeholder for UpdateScore
            if (ProjectileImage.ImageTop <= 10) //Stops Projectile before reaching the top of the page
            {
                //Check to see if the projectile is within the marks of the target
                if (ProjectileImage.ImageLeft <= (TargetImage.ImageLeft + 25) && ProjectileImage.ImageLeft >= (TargetImage.ImageLeft - 25))
                {
                    CurrentUserScore.CurScore = CurrentUserScore.CurScore + 1; //Adds to the Users Score 
                    upTicker.IsEnabled = false;
                    ProjectileImage.ImageTop = 400; // Resets the projectile back to the bottom
                    if (CurrentUserScore.CurScore >= 5 && CurrentUserScore.CurScore < 8) //Increases the target speed to increase difficulty
                    {
                        gameTicker.Interval = TimeSpan.FromMilliseconds(5); 
                    }
                    else if (CurrentUserScore.CurScore >= 8)
                    {
                        gameTicker.Interval = TimeSpan.FromMilliseconds(2);
                    }
                }
                else
                {
                    upTicker.IsEnabled = false;
                    ProjectileImage.ImageTop = 400;
                    if (CurrentUserScore.CurScore <= CurrentUser.HighscoreProp) // Check if the Current Score is higher than the Highscore
                    {
                        MessageProp.MsgTextProp = "Try Again, You didn't beat your highscore";
                        MessageProp.VisibilityProp = "Visible";
                        CurrentUserScore.CurScore = 0; //Resets Current Score on Miss
                        gameTicker.Interval = TimeSpan.FromMilliseconds(25);
                    }
                    else
                    {
                        CurrentUser.HighscoreProp = CurrentUserScore.CurScore;
                        msg = await UpdateAsync(CurrentUser.UsernameProp, CurrentUser.HighscoreProp); //Updates the Users Highscore in the DB
                        MessageProp.MsgTextProp = msg;
                        MessageProp.VisibilityProp = "Visible"; // Provide User with Feedback
                        CurrentUserScore.CurScore = 0;
                        gameTicker.Interval = TimeSpan.FromMilliseconds(25);
                    }
                    //resets score as well as check to see if current score is greater than highscore
                }
            }
            else
            {
                MoveProjectileUp();
            }
        }

        private void GameTicker_Tick(object sender, EventArgs e) // Target Movement Logic
        {
            if (TargetImage.ImageLeft == 0)
            {
                DirectIdentifier = "Right";
                MoveImageRight();
            }
            else if (TargetImage.ImageLeft < 950 && DirectIdentifier == "Right" )
            {
                MoveImageRight();
            }
            else if (TargetImage.ImageLeft == 950)
            {
                DirectIdentifier = "Left";
                MoveImageLeft();
            }
            else if (TargetImage.ImageLeft < 950 && DirectIdentifier == "Left")
            {
                MoveImageLeft();
            }
        }

        private void StartGame() //Begin the Game
        {
            gameTicker.Interval = TimeSpan.FromMilliseconds(25);
            gameTicker.IsEnabled = true;
        }

        private void MoveImageRight()
        {
            TargetImage.ImageLeft = TargetImage.ImageLeft + 5;
        }
        private void MoveImageLeft()
        {
            TargetImage.ImageLeft = TargetImage.ImageLeft - 5;
        }

        private void MoveProjectileUp()
        {
            ProjectileImage.ImageTop = ProjectileImage.ImageTop - 5;
        }

        static async Task<Uri> UpdateScoreAsync(UserModel CurrUser)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/User/ScoreUpdate", CurrUser);
            response.EnsureSuccessStatusCode(); // Ensure the Update was successful
            return response.Headers.Location;
        }

        //Updates User Highscore in the DB
        public static async Task<string> UpdateAsync(string cUsername, int updatedScore)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            string errMsg = "";

            try
            {
                UserModel UpdateUser = new UserModel
                {
                    UsernameProp = cUsername,
                    HighscoreProp = updatedScore
                };

                var updateResponse = await UpdateScoreAsync(UpdateUser);
                errMsg = "New Highscore!"; 
                return errMsg;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return errMsg;
            }
        }
    }
}
