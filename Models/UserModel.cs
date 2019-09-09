using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShotTrack.Models
{
    public class UserModel : INotifyPropertyChanged
    {
        //User Class for personalized User experience within App
        private string _username;
        private string _password;
        private int _highscore;

        public string UsernameProp
        {
            get { return _username; }
            set { _username = value;
                OnPropertyChange("UsernameProp");
            }
        }

        public string PasswordProp
        {
            get { return _password; }
            set { _password = value;
                OnPropertyChange("PasswordProp");
            }
        }

        public int HighscoreProp
        {
            get { return _highscore; }
            set { _highscore = value;
                OnPropertyChange("HighscoreProp");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //Ensure View is notified when properties are changed

        protected virtual void OnPropertyChange(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
