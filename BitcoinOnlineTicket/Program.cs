using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using NSN.Core;

namespace BitcoinOnlineTicket
{
    class Program
    {
        static void Main(string[] args)
        {
            BtcPrice price = new BtcPrice();
            var runner = new Runner(price, "http://localhost:8080");
            runner.Start();
            Console.Read();
            runner.Stop();
        }
    }

    internal class BtcPrice
    {
        private DateTime _lastUpdate 
            = DateTime.ParseExact("2001-01-01 01:01:01,001", "yyyy-MM-dd HH:mm:ss,fff", CultureInfo.InvariantCulture);
        private List<BitcoinTarget> _json;
        public int? GetPrice(string symbol)
        {
            if ((DateTime.Now - _lastUpdate).TotalMinutes >= 15.0)
            {
                _lastUpdate = DateTime.Now;
                var restClient = new RestClient("http://api.bitcoincharts.com/");
                var request = new RestRequest("v1/markets.json");
                var response = restClient.Execute(request);

                _json = JsonConvert.DeserializeObject<List<BitcoinTarget>>(response.Content);
            }
            var symbolList = _json.Where(a => a.symbol.ToLower().Equals(symbol.ToLower()));
            var bitcoinTargetSubs = symbolList as BitcoinTarget[] ?? symbolList.ToArray();
            return bitcoinTargetSubs.Any() ? (int?)bitcoinTargetSubs.First().avg : null;
        }
    }

    internal class BitcoinTarget
    {
        // ReSharper disable once InconsistentNaming
        public string currency;
        // ReSharper disable once InconsistentNaming
        public double? avg;
        // ReSharper disable once InconsistentNaming
        public string symbol;
    }
}
