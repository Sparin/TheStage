using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
            Application.Current.Exit += (s, e) => { MessageBox.Show(
@"The Stage - Demo (build #870da92) версия для конкурса ВОКИ
---------------------------------------------------------------------

Sparin - vk.com/sparin - Программист костылей и автор карты
Rougeasus - vk.com/id106856544 - Дизайнер в разных стилях

OhPonyBoy - facebook.com/OhPonyBoy - Автор The Blob Symphony

--------------------------------------------------------------------

Официальная страница проекта: github.com/Sparin/TheStage
Лицензия: MIT", "Авторы"); };

            MessageBox.Show(
@"Краткая инструкция, перед началом демо уровня
W или I - треугольник
A или J - квадрат
S или K - крест
D или L - круг

Фигура - одиночное нажатие
Стрелка - двойное нажатие
Фигура в круге - зажатие клавиши на время, указанное под кругом


Подробно иллюстрированную инструкцию вы можете найти на GitHub'е проекта или странице конкурса ВОКИ","How To Play");
            wndGameController controller = new wndGameController();
            controller.ContentRendered += (s, e) => { controller.DataContext = new wndGameControllerViewModel(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Missions\[Demo] The Blob Symphony");};
            controller.ShowDialog();
            if (Application.Current != null)
                Application.Current.Shutdown();
           
            //new wndGameController() { DataContext = new wndGameControllerViewModel(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Missions\[Demo] The Blob Symphony") }.ShowDialog();
        }

        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
