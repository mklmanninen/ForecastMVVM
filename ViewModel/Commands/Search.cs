using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forecast_app.ViewModel.Commands
{
    public class Search : ICommand
    {
        public WeatherVM VM { get; set; }

        // If the textbox for searching is empty, Search Button is unusable.
        // Check CanExecute();
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Search(WeatherVM vm) 
        {
            VM = vm;
        }       
        public bool CanExecute(object parameter)
        {
            string query = parameter as string;
            if (string.IsNullOrWhiteSpace(query))
            {
                return false;
            }
            return true;
        }

        //The MakeQuery method from WeatherVM is called
        //MakeQuery uses GetCities method from AccuWeatherHelper,
        //which fetches relevant city objects to a List<City>.
        public void Execute(object parameter)
        {
            VM.MakeQuery();
        }
    }
}
