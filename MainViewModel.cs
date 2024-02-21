using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Waluty
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private List<Currency> _currencyList;
        public int SliderValue = 0;
        public int DataRefreshTime = 1000000;
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        public List<Currency> CurrencyList
        {
            get { return _currencyList; }
            set
            {
                _currencyList = value;
                OnPropertyChanged(nameof(CurrencyList));
            }
        }
        
        public MainViewModel()
        {
            CurrencyList = new List<Currency>() { new Currency("USD", "USD", "4.02") };
            Task.Run(async () =>
            {
                while (true)
                {
                    var T = DataManager.GetDataFromWebsite();
                    T.Wait();
                    CurrencyList = T.Result;
                    await Task.Delay(DataRefreshTime);
                }
            });
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    


    

}
