using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Input;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace Waluty
{
    public class MainViewModel : INotifyPropertyChanged
    {
        
        public MainViewModel()
        {
            CostModel = new PlotModel { Title = "" };
            //CostModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
            ChartsList = new ObservableCollection<DataForCharts>();

            CurrencyList = new List<Currency>();
            
            
            AdditionalDataInfo = new AdditionalDataInfo();
            GetDataFromWebsiteSemaphore = new SemaphoreSlim(1);
            bool IsDataAvailable = false;
            Task.Run(async () =>
            {
                while (true)
                {
                    
                    GetDataFromWebsiteSemaphore.Wait();
                    var T = DataManager.GetDataFromWebsite();
                    await T;

                    CurrencyList = T.Result.CurrencyList;
                    AdditionalDataInfo = T.Result.AdditionalDataInfo;

                    GetDataFromWebsiteSemaphore.Release();
                    IsDataAvailable = true;
                    await Task.Delay(DataRefreshTime);
                }
            });
            while (!IsDataAvailable) { }; //spin 
            LoadUserSettings();
        
            if (_chartsList.Count > 0)
            {
                RepairCheckBoxes();
            }
        }
        public void RepairCheckBoxes()
        {
            if (_chartsList.Count > 0)
            {
                //naprawic checobxy
                CreateChart(_chartsList[0].name);
                foreach (var currency in ChartsList)
                {

                    int i = 0;
                    while (i < _currencyList.Count && _currencyList[i].code != currency.name) i++;
                    if (i < _currencyList.Count && _currencyList[i].code == currency.name) CurrencyList[i].IsChecked = true;


                }
            }
        }
        public void LoadUserSettings()
        {
            try
            {
                string Data = File.ReadAllText("UserSettings.json");
                ChartsList = JsonConvert.DeserializeObject<ObservableCollection<DataForCharts>>(Data);
       
            } catch { }
        }
        public void SaveUserSettings()
        {
            if (CurrencyList.Count > 0)
            {
                DataManager.SaveDataToFile("UserSettings.json", _chartsList);
            }
        }

        public async void CreateChart(string msg)
        {
            var GetDataTask = DataManager.GetLastNDataFromWebsite(msg, 90);
            PlotModel Chart = new PlotModel { Title = msg };
            Chart.Axes.Add(new DateTimeAxis 
            {
                Position = AxisPosition.Bottom,
                StringFormat = "yyyy-MM-dd",
                Title = "Date"
            });
            Chart.Axes.Add(new LinearAxis
            {
                Position= AxisPosition.Left,
                Title = "Currency value [PLN]",
            });
            var series = new LineSeries { Title = msg };

            await GetDataTask;
            double T1, T2 = 1;
            T1 = 0;
            var Res = GetDataTask.Result;
            List<double> DataForStats = new List<double>(); //important for extra stats above charts (like 30 days/90 days avg value) 
            foreach (var Date in Res.Keys)
            {
                T2 = T1;
                int year = int.Parse(Date.Substring(0, 4));
                int month = int.Parse(Date.Substring(5, 2));
                int day = int.Parse(Date.Substring(8, 2));
                series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(year, month, day)), double.Parse(Res[Date])));
                T1 = double.Parse(Res[Date]);
                DataForStats.Add(T1); 
            }
            Chart.Series.Add(series);
            CostModel = Chart;

            if (ChartsList.Count > 0)
            {
                int i = 0;
                var D = ChartsList.FirstOrDefault(x => x.name == msg);
                if (D != null)
                {
                    //For 24h difference
                    double DifferenceBetweenPrices = Math.Round((T1 / T2) * 100-100, 2);
                    if (DifferenceBetweenPrices > 0)
                    {
                        AllChartStats[0] = new ChartStats($"{msg} {Math.Round(T1,2)} PLN", $"+{DifferenceBetweenPrices}%");
                        ArrowIcon = "RISE";
                        ChartStatsTextColor = Brushes.Green;
                    }
                    else
                    {
                        AllChartStats[0] = new ChartStats($"{msg} {Math.Round(T1, 2)} PLN", $"{DifferenceBetweenPrices}%");
                        ArrowIcon = "DROP";
                        ChartStatsTextColor = Brushes.Red;
                    }
                    //for 30 days difference
                    double Max = DataForStats[DataForStats.Count - 1];
                    double Sum = 0;
                    for (int k = DataForStats.Count - 1; k >= DataForStats.Count - 30; k--)
                    {
                        Sum += DataForStats[k];
                        if (DataForStats[k] > Max) Max = DataForStats[k];
                    }
                    double AVG = Math.Round(Sum / 30,2);
                    AllChartStats[1] = new ChartStats($"Max: {Math.Round(Max, 2)}", $"AVG: {AVG}");

                    //for 90 days difference
                    Sum = 0;
                    foreach (var value in DataForStats)
                    {
                        Sum += value;
                        if (value > Max) Max = value;
                    }
                    
                    AVG = Math.Round(Sum / DataForStats.Count, 2);
                    AllChartStats[2] = new ChartStats($"Max: {Math.Round(Max,2)}", $"AVG: {AVG}");

                }
            }
        }

        private Data _data;
        private AdditionalDataInfo _additionalDataInfo;
        private List<Currency> _currencyList;
        
        private SemaphoreSlim GetDataFromWebsiteSemaphore;

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

        private ObservableCollection<DataForCharts> _chartsList;
        public ObservableCollection<DataForCharts> ChartsList
        {
            get { return _chartsList;  }
            set { _chartsList = value; }
        }

        private ObservableCollection<ChartStats> _allChartStats = new ObservableCollection<ChartStats> 
        {
            new ChartStats(),
            new ChartStats(),
            new ChartStats(),
        };
        public ObservableCollection<ChartStats> AllChartStats
        {
            get { return _allChartStats; }
            set
            {
                _allChartStats = value;
            }
        }
        


        private string _arrowIcon;
        public string ArrowIcon
        {
            get { return _arrowIcon; }
            set
            {
                _arrowIcon = value;
                OnPropertyChanged(nameof(ArrowIcon));
            }
        }

        private Brush _charStatsTextColor = Brushes.Black;
        public Brush ChartStatsTextColor
        {
            get { return _charStatsTextColor; }
            set
            {
                if (_charStatsTextColor != value)
                {
                    _charStatsTextColor = value;
                    OnPropertyChanged(nameof(ChartStatsTextColor)); // Wywołanie powiadomienia o zmianie
                }
            }
        }

        public void AddItem(string data)
        {
            if (_chartsList.Count == 5)
            {
                var ItemToRemove = ChartsList[_chartsList.Count - 1];
                Currency Curr = CurrencyList.FirstOrDefault(x => x.code == ItemToRemove.name);
                if (Curr != null)
                {
                    Curr.IsChecked = false;
                }
                _chartsList.RemoveAt(_chartsList.Count - 1);
            } 

            ChartsList.Add(new DataForCharts(data));
            
        }
        public void RemoveItem(string data)
        {
            int i = 0;
            while (i < ChartsList.Count) 
            { 
                if (ChartsList[i].name == data)
                {
                    ChartsList.RemoveAt(i);
                    Currency Curr = CurrencyList.FirstOrDefault(x => x.code == data);
                    if (Curr != null)
                    {
                        Curr.IsChecked = false;
                    }
                    return;
                }
                i++;
            }
             
        }


        private PlotModel _costModel;
        public PlotModel CostModel {
            get { return _costModel;  }
            set 
            {
                _costModel = value;
                OnPropertyChanged(nameof(CostModel));
            } 
        
        }

        
        

        private ICommand _updateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                {
                    _updateCommand = new RelayCommand(async param => await UpdateData(), param => true);
                }
                return _updateCommand;
            }
        } 
        private async Task UpdateData()
        {
            GetDataFromWebsiteSemaphore.Wait();
            var T = DataManager.GetDataFromWebsite();
            await T;

            CurrencyList = T.Result.CurrencyList;
            AdditionalDataInfo = T.Result.AdditionalDataInfo;

            GetDataFromWebsiteSemaphore.Release();
            if (_chartsList.Count > 0)
            {
                //naprawic checobxy
                CreateChart(_chartsList[0].name);
                foreach (var currency in ChartsList)
                {

                    int i = 0;
                    while (i < _currencyList.Count && _currencyList[i].code != currency.name) i++;
                    if (i < _currencyList.Count && _currencyList[i].code == currency.name) CurrencyList[i].IsChecked = true;


                }
            }
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

    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targerType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() == "DROP")
            {
                return "RedArrowDown.png";
            }
            else
            {
                return "GreenArrowUp.png";
            }
        }
        public object ConvertBack(object value, Type targetType,  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
