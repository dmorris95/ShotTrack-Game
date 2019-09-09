using ShotTrack.Commands;
using ShotTrack.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShotTrack.ViewModels
{
    public class NavigationViewModel : INotifyPropertyChanged
    {
        // Navigation Class

        public BaseRelayCommand LoginNavCommand { get; private set; }
        public BaseRelayCommand CreateNavCommand { get; private set; }
        public BaseRelayCommand GameNavCommand { get; private set; }

        private object selectedViewModel;
        public object SelectedViewModel 
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; OnPropertyChanged("SelectedViewModel"); }
        }

        public NavigationViewModel()
        {
            LoginNavCommand = new BaseRelayCommand(CanOpenLogin, OpenLogin); //Login View Navigation
            CreateNavCommand = new BaseRelayCommand(CanOpenCreate, OpenCreate); //Create User View Navigation
            GameNavCommand = new BaseRelayCommand(CanOpenGame, OpenGame); //Game View Navigation
        }

        private bool CanOpenLogin(object obj)
        {
            return true;
        }
        private void OpenLogin(object obj)
        {
            SelectedViewModel = new UserLoginViewModel();
        }

        private bool CanOpenCreate(object obj)
        {
            return true;
        }
        private void OpenCreate(object obj)
        {
            SelectedViewModel = new UserCreationViewModel();
        }

        //Can't play game unless you login
        public bool CanOpenGame(object obj)
        {
            if (UserLoginViewModel.LoggedInCheck == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void OpenGame(object obj)
        {
            SelectedViewModel = new GameViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
