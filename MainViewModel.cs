using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Waluty
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Data _data;
        private AdditionalDataInfo _additionalDataInfo;
        private List<Currency> _currencyList;

        public int DataRefreshTime = 1000000;
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        public AdditionalDataInfo AdditionalDataInfo
        {
            get { return _additionalDataInfo; }
            set
            {
                _additionalDataInfo = value;
                OnPropertyChanged(nameof(AdditionalDataInfo));
            }
        }
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
            CurrencyList = new List<Currency>();
            AdditionalDataInfo = new AdditionalDataInfo();
            Task.Run(async () =>
            {
                while (true)
                {
                    var T = DataManager.GetDataFromWebsite();
                    await T;
                    CurrencyList = T.Result.CurrencyList;
                    AdditionalDataInfo = T.Result.AdditionalDataInfo;
                    

                    await Task.Delay(DataRefreshTime);
                }
            });
        }

        private RelayCommand _updateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                {
                    _updateCommand = new RelayCommand(param => UpdateData(), param => CanUpdateData());
                }
                return _updateCommand;
            }
        } 
        private void UpdateData()
        {
            
        }
        private bool CanUpdateData()
        {
            return true;
        }








        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }




    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

}
