using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TheStage.ViewModel
{
    public class StatisticsViewModel:INotifyPropertyChanged
    {
        private long score = 0;
        public long Score
        {
            get { return score; }
            set { score = value; RaisePropertyChanged(); }
        }

        private int excellent = 0;
        public int Excellent
        {
            get { return excellent; }
            set { excellent = value; RaisePropertyChanged(); }
        }

        private int good = 0;
        public int Good
        {
            get { return good; }
            set { good = value; RaisePropertyChanged(); }
        }

        private int safe = 0;
        public int Safe
        {
            get { return safe; }
            set { safe = value; RaisePropertyChanged(); }
        }

        private int awful = 0;
        public int Awful
        {
            get { return awful; }
            set { awful = value; RaisePropertyChanged(); }
        }

        private int bad = 0;
        public int Bad
        {
            get { return bad; }
            set { bad = value; RaisePropertyChanged(); }
        }

        //private int miss = 0;
        //public int Miss
        //{
        //    get { return miss; }
        //    set { miss = value; RaisePropertyChanged(); }
        //}


        #region MVVM Related
        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
