using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheStage.ViewModel;

namespace TheStage.Controls
{
    /// <summary>
    /// Логика взаимодействия для Statistics.xaml
    /// </summary>
    public partial class Statistics : UserControl
    {
        public event Action Restart;
        public event Action BackToMenu;

        public Statistics()
        {
            InitializeComponent();
            btnRestart.Click += (s, e) => Restart();
            btnBackToMenu.Click += (s, e) => BackToMenu();
        }

        public Statistics(StatisticsViewModel dataContext):this()
        {
            DataContext = dataContext;
        }
    }
}
