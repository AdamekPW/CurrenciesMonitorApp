using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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

        public static async Task<List<Currency>> GetDataFromWebsite(string url = $"http://api.nbp.pl/api/exchangerates/tables/A/")
        {
            List<Currency> data = new List<Currency>();
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
                        return data;
                    }
                    JArray Rates = (JArray)jsonObject[0]["rates"];
                    foreach (var cur in Rates)
                    {
                        data.Add(new Currency(cur["currency"].ToString(), cur["code"].ToString(), cur["mid"].ToString()));
                    }
              

                } else
                {
                    Console.WriteLine("Data download error");
                }

            }
            return data;
           
        }
        

        public static async Task SaveDataToFile(string FileName, List<Currency> currencies)
        {
            await Task.Delay(1);
            string json = JsonConvert.SerializeObject(currencies);
            File.WriteAllText(FileName, json);     
        }
        public static async Task<List<Currency>> ReadDataFromFile(string FileName)
        {
            string Data = File.ReadAllText(FileName);
            return JsonConvert.DeserializeObject<List<Currency>>(Data);
        }


    }
    
      
    public class Currency : INotifyPropertyChanged
    {
        private string _currency;
        private string _code;
        private string _mid;

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

        public Currency(string Currency, string Code, string Mid)
        {
            currency = Currency;
            code = Code;
            mid = Mid;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}