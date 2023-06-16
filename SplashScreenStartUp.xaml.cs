﻿using System;
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
    /// <summary>
    /// Interaction logic for SplashScreenStartUp.xaml
    /// </summary>
    public partial class SplashScreenStartUp : Window
    {
        public SplashScreenStartUp()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //add this line to force ROG ally to normal state, ASUS armory crate forces all apps on built in screen to maximize
            if (this.WindowState == WindowState.Maximized) { this.WindowState = WindowState.Normal; }
        }
    }
}
