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
using System.Windows.Shapes;

namespace Handheld_Control_Panel
{
    public static class GameLauncherNavigation
    {
        public static bool windowNavigation = true;
    }
    public partial class GameLauncher : Window
    {
        public GameLauncher()
        {
            InitializeComponent();
        }
    }
}
