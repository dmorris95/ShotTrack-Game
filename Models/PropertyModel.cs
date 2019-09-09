using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShotTrack.Models
{
    public class PropertyModel : INotifyPropertyChanged
    {
        private string _visibilityProp;
        private string _msgTextProp;

        public string VisibilityProp
        {
            get { return _visibilityProp; }
            set { _visibilityProp = value;
                OnPropertyChange("VisibilityProp");
            }
        }

        public string MsgTextProp
        {
            get { return _msgTextProp; }
            set { _msgTextProp = value;
                OnPropertyChange("MsgTextProp");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChange(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
