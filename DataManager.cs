using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Waluty
{
    public static class DataManager
    {
        public static void PrintData(List<Currency> data)
        {
            foreach (var d in data)
            {
                Console.WriteLine("------");
                Console.WriteLine($"currency: {d.currency}");
                Console.WriteLine($"code: {d.code}");
                Console.WriteLine($"mid: {d.mid}");
            }
        }
        public static async Task<Data> GetDataFromWebsite(string url = $"http://api.nbp.pl/api/exchangerates/tables/A/")
        {
            AdditionalDataInfo dataInfo = new AdditionalDataInfo();
            List<Currency> CurData = new List<Currency>();
            using (var client = new HttpClient())
            {
                HttpResponseMessage responseMessage = await client.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string json = await responseMessage.Content.ReadAsStringAsync();
                    dynamic? jsonObject = JsonConvert.DeserializeObject(json);
                    if (jsonObject == null)
                    {
                        Console.WriteLine("Data is null");
                        return new Data();
                    }
                    dataInfo = new AdditionalDataInfo(
                        jsonObject[0]["table"].ToString(),
                        jsonObject[0]["no"].ToString(),
                        jsonObject[0]["effectiveDate"].ToString()
                    );
                    JArray Rates = (JArray)jsonObject[0]["rates"];
                    foreach (var cur in Rates)
                    {
                        //Random rand = new Random();
                        //CurData.Add(new Currency(cur["currency"].ToString(), cur["code"].ToString(), rand.Next(10).ToString()));
                        CurData.Add(new Currency(cur["currency"].ToString(), cur["code"].ToString(), cur["mid"].ToString()));
                    }


                }
                else
                {
                    Console.WriteLine("Data download error");
                }

            }
            return new Data(dataInfo, CurData);

        }
        public static async Task<Dictionary<string, string>> GetLastNDataFromWebsite(string Currency, int points)
        {
            if (points > 90) points = 90;
            string url = $"http://api.nbp.pl/api/exchangerates/rates/a/{Currency}/last/{points}/?format=json";
            Dictionary<string, string> Data = new Dictionary<string, string>();

            using (var client = new HttpClient())
            {
                HttpResponseMessage responseMessage = await client.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string json = await responseMessage.Content.ReadAsStringAsync();
                    dynamic? jsonObject = JsonConvert.DeserializeObject(json);
                    if (jsonObject == null)
                    {
                        Console.WriteLine("Data is null");
                        return Data;
                    }
                    string D = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                    //Console.WriteLine(D);
                    JArray Rates = jsonObject["rates"];
                    foreach (var DataPoint in Rates)
                    {
                        Data[DataPoint["effectiveDate"].ToString()] = DataPoint["mid"].ToString();
                    }

                }
                else
                {
                    Console.WriteLine("Data download error");
                }
                return Data;
            }
        }
        public static void SaveDataToFile(string FileName, ObservableCollection<DataForCharts> currencies)
        {
            string json = JsonConvert.SerializeObject(currencies);
            File.WriteAllText(FileName, json);
        }
        public static async Task<ObservableCollection<DataForCharts>> ReadDataFromFile(string FileName)
        {
            await Task.Delay(1);
            string Data = File.ReadAllText(FileName);
            return JsonConvert.DeserializeObject<ObservableCollection<DataForCharts>>(Data);
        }


    }


    public class Data : INotifyPropertyChanged
    {
        public AdditionalDataInfo AdditionalDataInfo;
        public List<Currency> CurrencyList;
        public Data()
        {
            AdditionalDataInfo = new AdditionalDataInfo();
            CurrencyList = new List<Currency>();
        }
        public Data(AdditionalDataInfo _AddDataInfo, List<Currency> _Currencies)
        {
            AdditionalDataInfo = _AddDataInfo;
            CurrencyList = _Currencies;
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Currency : INotifyPropertyChanged
    {
        private string _currency;
        private string _code;
        private string _mid;
        private bool _isChecked;
        public string currency
        {
            get { return _currency; }
            set
            {
                _currency = value;
                OnPropertyChanged(nameof(currency));
            }
        }
        public string code
        {
            get { return _code; }
            set
            {
                _code = value;
                OnPropertyChanged(nameof(code));
            }
        }
        public string mid
        {
            get { return _mid; }
            set
            {
                _mid = value;
                OnPropertyChanged(nameof(mid));
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }
        public Currency(string Currency, string Code, string Mid, bool isChecked = false)
        {
            currency = Currency;
            code = Code;
            mid = Mid;
            IsChecked = isChecked;
        }






        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class AdditionalDataInfo : INotifyPropertyChanged
    {
        private string _table;
        private string _no;
        private string _effectiveDate;

        public string table
        {
            get { return _table; }
            set
            {
                _table = value;
                OnPropertyChanged(nameof(table));
            }
        }
        public string no
        {
            get { return _no; }
            set
            {
                _no = value;
                OnPropertyChanged(nameof(no));
            }
        }
        public string effectiveDate
        {
            get { return _effectiveDate; }
            set
            {
                _effectiveDate = value;
                OnPropertyChanged(nameof(effectiveDate));
            }
        }

        public AdditionalDataInfo(string Table = "-1", string No = "-1", string EffectiveDate = "-1")
        {
            table = Table;
            no = No;
            effectiveDate = EffectiveDate;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    public class DataForCharts : INotifyPropertyChanged
    {
        private string _name;
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(name));
            }

        }
        

        public DataForCharts(string Name)
        {
            name = Name;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(PropertyChanged, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ChartStats : INotifyPropertyChanged
    {
       
        private string _name;
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(name));
            }
        }
        private string _difference;

        public string difference
        {
            get { return _difference; }
            set
            {
                _difference = value;
                OnPropertyChanged(nameof(difference));
            }
        }

        public ChartStats(string Name="", string Difference = "")
        {
            name=Name;
            difference = Difference;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}