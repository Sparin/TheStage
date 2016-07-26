using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TheStage.ViewModel
{
    class PrimitiveViewModel: INotifyPropertyChanged
    {
        private PathGeometry animationPath;
        public PathGeometry AnimationPath
        {
            get { return animationPath; }
            set { animationPath = value; RaisePropertyChanged(); }
        }

        private Storyboard animation;
        public Storyboard Animation
        {
            get { return animation; }
            set { animation = value; RaisePropertyChanged(); }
        }


        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
