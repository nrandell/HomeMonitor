using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OilPalScraper
{
    public class Program
    {
        private static readonly HttpClient _client = new HttpClient();

        public static void Main(string[] args)
        {
            var cancel = CancellationToken.None;
            try
            {
                RunAsync(cancel).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private static async Task RunAsync(CancellationToken cancel)
        {
            while (!cancel.IsCancellationRequested)
            {
                var readings = await ReadAsync(cancel);
                Console.WriteLine(readings);
                await Task.Delay(60000, cancel);
            }
        }

        private static async Task<Reading> ReadAsync(CancellationToken cancel)
        {
            var timestamp = DateTime.UtcNow;
            var response = await _client.GetAsync("http://app.oilpal.com/Report/GetChartData?SerialNo=BB-0000-0000-3860&DeviceNo=0&NumericPeriod=2");
            Reading results = null;
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var chartData = JsonConvert.DeserializeObject<ChartData>(json);

                var readings = new OilLevel[chartData.Data.Length];
                for (var i = 0; i < readings.Length; i++)
                {
                    var data = chartData.Data[i];
                    var label = chartData.Label[i];
                    var date = DateTime.Parse(label);
                    var reading = int.Parse(data);
                    var value = new OilLevel { Day = date, Litres = reading };
                    readings[i] = value;
                }
                results = new Reading { TimestampUtc = timestamp, Levels = readings };
            }
            return results;
        }
}
}
