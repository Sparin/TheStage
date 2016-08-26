using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TheStage.ViewModel
{
    class wndMenuViewModel : INotifyPropertyChanged
    {
        private string title = "The Stage";
        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }

        public wndMenuViewModel()
        {
            new wndGameController() { DataContext = new wndGameControllerViewModel(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Missions\[Demo] The Blob Symphony") }.ShowDialog();
        }

        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
