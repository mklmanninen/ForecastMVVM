using Forecast_app.Model;
using Forecast_app.ViewModel.Commands;
using Forecast_app.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Forecast_app.ViewModel
{
    public class WeatherVM : INotifyPropertyChanged
    {
        private string query;

        public string Query
        {
            get { return query; }
            set
            {
                query = value;
                OnPropertyChanged("Query");
            }
        }

        private CurrentConditions currentConditions;

        public CurrentConditions CurrentConditions
        {
            get { return currentConditions; }
            set
            {
                currentConditions = value;
                OnPropertyChanged("CurrentConditions");
            }
        }

        /*
         * There was previously a bug, which caused selectedCity property not 
         * to update when performing a 2nd search.
         * This was fixed by tailoring this property as such and changing the
         * selectedCity binding to TwoWay mode. Hence, this code is as it is right now
        */
        private City selectedCity;
        public City SelectedCity
        {
            get { return selectedCity; }
            set
            {
                if (selectedCity != value)
                {
                    selectedCity = value;
                    OnPropertyChanged(nameof(SelectedCity));
                    if (selectedCity != null)
                    {
                        _ = OnSelectedCityChangedAsync();
                    }
                }
            }
        }

        private async Task OnSelectedCityChangedAsync()
        {
            await GetCurrentConditions();
        }

        //This collection shows the searched cities in the ListView.
        public ObservableCollection<City> CityCollection { get; set; }
        public Search Search { get; set; }

        public WeatherVM()
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                selectedCity = new City
                {
                    LocalizedName = "Helsinki",
                    Key = "12345"
                };

                currentConditions = new CurrentConditions
                {
                    WeatherText = "rainy",
                    Temperature = new Temperature
                    {
                        Metric = new Units
                        {
                            Value = "21"
                        }
                    }
                };
            }
            Search = new Search(this);
            //Initializing city collection already here to not create multiple instances of collection
            CityCollection = new ObservableCollection<City>();
        }   

        private async Task GetCurrentConditions()
        {
            if (SelectedCity == null || string.IsNullOrEmpty(SelectedCity.Key))
            {
                Debug.WriteLine("SelectedCity or its Key is null or empty.");
                return;
            }

            CurrentConditions = await AccuWeatherHelper.GetCurrentConditionsAsync(SelectedCity.Key);
        }



        //This method is for printing the cities. 
        //It uses GetCities method from helpers.
        public async void MakeQuery()
        {
            try
            {
                var cities = await AccuWeatherHelper.GetCities(Query);

                // Clear the collection each time 'Search' Button is pressed.
                CityCollection.Clear();
                foreach (City city in cities)
                {
                    CityCollection.Add(city);
                }

                /*if (CityCollection.Any())
                {
                    SelectedCity = CityCollection.First(); //Testausta varten
                }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in MakeQuery: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                try
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error in OnPropertyChanged for property '{propertyName}': {ex.Message}");
                }
            }
        }
    }
}

