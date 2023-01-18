using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlzEx.Theming;
using Handheld_Control_Panel.Views;

namespace Handheld_Control_Panel.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        public ShellViewModel() 
        {
           

        }


        public void LoadPage()
        {
            ChangeActiveItemAsync(new HomePageViewModel(),true);
        }
    }
}
