using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TheStage.Elements.Base.Factories;

namespace TheStage.ViewModel
{
    public class StatisticsViewModel : INotifyPropertyChanged
    {
        private ImageSource image;
        public ImageSource Image
        {
            get { return image; }
            set { image = value; RaisePropertyChanged(); }
        }

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

        private int miss = 0;
        public int Miss
        {
            get { return miss; }
            set { miss = value; RaisePropertyChanged(); }
        }

        public double Success
        {
            get
            {
                double reality = Excellent + Good * 0.9 + Safe * 0.7 + Awful * 0.5 + Bad * 0.3;
                double summ = Excellent + Good + Safe + Awful + Bad + Miss;
                return reality / summ * 100;
            }
        }

        public string Status
        {
            get
            {
                switch (Convert.ToInt32(Success) / 5)
                {
                    case 20: //x = 100%
                        Image = ElementFactory.GetBitmapSource(@"Resources\Images\UI\GameController\Faces\excellent.png");
                        return string.Format("Идеально! ({0:N1}%)", Success);
                    case 19: //x > 95%
                        Image = ElementFactory.GetBitmapSource(@"Resources\Images\UI\GameController\Faces\good.png");
                        return string.Format("Изумительно! ({0:N1}%)", Success);
                    case 18: //x > 90%
                        Image = ElementFactory.GetBitmapSource(@"Resources\Images\UI\GameController\Faces\safe.png");
                        return string.Format("Хорошо! ({0:N1}%)", Success);
                    case 17: //x > 80%
                    case 16:
                        Image = ElementFactory.GetBitmapSource(@"Resources\Images\UI\GameController\Faces\awful.png");
                        return string.Format("Пройден ({0:N1}%)", Success);
                    default:
                        Image = ElementFactory.GetBitmapSource(@"Resources\Images\UI\GameController\Faces\miss.png");
                        return string.Format("Неудача ({0:N1}%)", Success);
                }
            }
        }

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
