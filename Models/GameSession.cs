using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShotTrack.Models
{
    public class GameSession : INotifyPropertyChanged
    {
        private int _curScore;

        public int CurScore
        {
            get { return _curScore; }
            set { _curScore = value;
                OnPropertyChange("CurScore");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChange(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
