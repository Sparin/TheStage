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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TheStage.Controls
{
    /// <summary>
    /// Логика взаимодействия для PauseMenu.xaml
    /// </summary>
    public partial class PauseMenu : UserControl
    {
        public event Action Restart;
        public event Action Resume;
        public event Action BackToMenu;

        public PauseMenu()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                DoubleAnimation widthAnim = new DoubleAnimation(0, 150, TimeSpan.FromMilliseconds(500));
                shortStatistics.BeginAnimation(WidthProperty, widthAnim);
            };
            btnResume.Click += (s,e)=> Resume();
            btnRestart.Click += (s, e) => Restart();
            btnBackToMenu.Click += (s, e) => BackToMenu();
        }
    }
}
