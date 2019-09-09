using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShotTrack.Models
{
    public class ImageModel : INotifyPropertyChanged
    {
        private int _imageLeft;
        private int _imageTop;

        public int ImageLeft
        {
            get { return _imageLeft; }
            set { _imageLeft = value;
                OnPropertyChange("ImageLeft");
            }
        }

        public int ImageTop
        {
            get { return _imageTop; }
            set { _imageTop = value;
                OnPropertyChange("ImageTop");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChange(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
