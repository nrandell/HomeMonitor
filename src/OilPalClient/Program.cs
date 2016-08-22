using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;

namespace OilPalClient
{
    public class Program
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly HtmlParser _parser = new HtmlParser();

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
                if (readings != null)
                {
                    foreach (var reading in readings)
                    {
                        Console.WriteLine($"Got {reading}");
                    }
                }
                await Task.Delay(60000, cancel);
            }
        }

        private static async Task<List<Reading>> ReadAsync(CancellationToken cancel)
        {
            var response = await _client.GetAsync("http://192.168.100.78/diag.htm");
            List<Reading> results = null;
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var dom = _parser.Parse(data);
                var content = dom.GetElementById("content");
                var tables = content.GetElementsByTagName("table");
                var table = tables[1];
                var rows = table.GetElementsByTagName("tr");
                Dictionary<string,int> heading = null;;
                foreach (var row in rows)
                {
                    var contents = ReadRow(row, heading == null);
                    if (heading == null)
                    {
                        heading = contents.Select((s,i) => Tuple.Create(s, i)).ToDictionary(t => t.Item1, t => t.Item2, StringComparer.OrdinalIgnoreCase);
                    } else
                    {
                        var reading = ParseRow(heading, contents);
                        if (reading != null)
                        {
                            if (results == null)
                            {
                                results = new List<Reading>();
                            }
                            results.Add(reading);

                        }
                    }
                }

            }
            return results;
        }

        private static string[] ReadRow(IElement row, bool isHeading)
        {
            var cols = row.GetElementsByTagName(isHeading ? "th" : "td");
            var results = new string[cols.Length];
            for (var i = 0; i < cols.Length; i++)
            {
                results[i] = cols[i].TextContent;
            }
            return results;
        }

        private static Reading ParseRow(Dictionary<string, int> heading, string[] row)
        {
            var device = row[heading["Device #"]];
            var rtAddress = row[heading["RF Address"]];
            var receiveCount = row[heading["#Rx"]];
            var rxTime = row[heading["Rx Time (h)"]];
            var data = row[heading["Data Aux Bat [Cache]"]];
            var flow = row[heading["[Fl Hi Lo Dif]"]];
            if (data != "No Data")
            {
                var reading = new Reading(device, rtAddress, receiveCount, rxTime, data, flow);
                return reading;
            } else
            {
                return null;
            }

        }
    }


}
