using System;
using System.Collections.ObjectModel;
using MahApps.Metro.IconPacks;
using Handheld_Control_Panel.Classes.Mvvm;
using Handheld_Control_Panel.Classes;

using System.Resources;
using System.Windows;
using System.Drawing;

namespace Handheld_Control_Panel.Classes.ViewModels
{
    public class ShellViewModelMI : BindableBase
    {
        public ObservableCollection<MenuItem> Menu { get; } = new();

        public ObservableCollection<MenuItem> OptionsMenu { get; } = new();

        public ShellViewModelMI()
        {

            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Speedometer, Height = 25, Width = 25 },
                Label = Application.Current.Resources["MainWindow_Menu_PerformanceSettings"].ToString(),
                //NavigationType = typeof(CPUPage),
                NavigationDestination = new Uri("Pages/CPUPage.xaml", UriKind.RelativeOrAbsolute),
            });
         
            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Tune, Height = 25, Width = 25 },
                Label = Application.Current.Resources["MainWindow_Menu_SystemSettings"].ToString(),
                //NavigationType = typeof(HomePage),
                NavigationDestination = new Uri("Pages/HomePage.xaml", UriKind.RelativeOrAbsolute),
            });
            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Monitor, Height = 25, Width = 25 },
                Label = Application.Current.Resources["MainWindow_Menu_DisplaySettings"].ToString(),
                //NavigationType = typeof(DisplayPage),
                NavigationDestination = new Uri("Pages/DisplayPage.xaml", UriKind.RelativeOrAbsolute),
            });

            if (Global_Variables.Global_Variables.cpuType == "AMD")
            {
                this.Menu.Add(new MenuItem()
                {
                    Icon = new PackIconSimpleIcons() { Kind = PackIconSimpleIconsKind.Amd, Height=40, Width=40 },
                    Label = Application.Current.Resources["MainWindow_Menu_AMDSettings"].ToString(),
                    //NavigationType = typeof(DisplayPage),
                    NavigationDestination = new Uri("Pages/AMDPage.xaml", UriKind.RelativeOrAbsolute),
                });
            }
            if (Global_Variables.Global_Variables.cpuType == "Intel")
            {
                this.Menu.Add(new MenuItem()
                {
                    Icon = new PackIconSimpleIcons() { Kind = PackIconSimpleIconsKind.Intel, Height = 35, Width = 35 },
                    Label = Application.Current.Resources["MainWindow_Menu_IntelSettings"].ToString(),
                    //NavigationType = typeof(DisplayPage),
                    NavigationDestination = new Uri("Pages/DisplayPage.xaml", UriKind.RelativeOrAbsolute),
                });
            }
            //new for apps
            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconUnicons() { Kind = PackIconUniconsKind.Browser, Height = 25, Width = 25 },
                Label = Application.Current.Resources["MainWindow_Menu_Apps"].ToString(),
                //NavigationType = typeof(GamepadSettingsPage),
                NavigationDestination = new Uri("Pages/AppPage.xaml", UriKind.RelativeOrAbsolute),
            });


            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.GoogleController, Height = 25, Width = 25 },
                Label = Application.Current.Resources["MainWindow_Menu_GamepadSettings"].ToString(),
                //NavigationType = typeof(GamepadSettingsPage),
                NavigationDestination = new Uri("Pages/GamepadSettingsPage.xaml", UriKind.RelativeOrAbsolute),
            });
            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.NoteMultipleOutline, Height = 25, Width = 25 },
                Label = Application.Current.Resources["MainWindow_Menu_Profiles"].ToString(),
                //NavigationType = typeof(ProfilesPage),
                NavigationDestination = new Uri("Pages/ProfilesPage.xaml", UriKind.RelativeOrAbsolute),
            });
            this.OptionsMenu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.InfoCircleSolid, Height = 25, Width = 25 },
                Label = Application.Current.Resources["MainWindow_Menu_Info"].ToString(),
                //NavigationType = typeof(InformationPage),
                NavigationDestination = new Uri("Pages/InformationPage.xaml", UriKind.RelativeOrAbsolute),
            });
            this.OptionsMenu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.CogSolid, Height = 25, Width = 25 },
                Label = Application.Current.Resources["MainWindow_Menu_Settings"].ToString(),
                //NavigationType = typeof(SettingsPage),
                NavigationDestination = new Uri("Pages/SettingsPage.xaml", UriKind.RelativeOrAbsolute),
            });
        }

       
    }
}