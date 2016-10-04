using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mios.Net;
using System.Reflection;

namespace MiosMonitor
{
    public class Program
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly Parser _parser = new Parser();

        public static void Main(string[] args)
        {
            try
            {
                Run(CancellationToken.None).GetAwaiter().GetResult();

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

        private static async Task Run(CancellationToken token)
        {
            var response = await _client.GetAsync("http://vera.home:3480/data_request?id=sdata");
            var json = await response.Content.ReadAsStringAsync();
            var parsed = _parser.Parse(json);
            var loadTime = parsed.LoadTime;
            var dataVersion = parsed.DataVersion;

            var names = parsed.Devices.ToDictionary(d => d.Id, d => d.Name);
            var latest = parsed.Devices.ToDictionary(d => d.Id);

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(2000, token);
                json = await TryGetJson(json, loadTime, dataVersion);
                if (json != null)
                {
                    parsed = _parser.Parse(json);
                    var versionShown = false;
                    if (parsed.Devices != null)
                    {
                        foreach (var device in parsed.Devices)
                        {
                            Device current;
                            if (latest.TryGetValue(device.Id, out current))
                            {
                                var displayed = false;
                                foreach (var property in device.GetType().GetProperties().Where(p => p.Name != "Name" && p.Name != "Category"))
                                {

                                    var previousValue = property.GetValue(current);
                                    var newValue = property.GetValue(device);
                                    if (!object.Equals(newValue, previousValue))
                                    {
                                        if (!versionShown)
                                        {
                                            Console.WriteLine($"{DateTime.Now}: Gone to version {parsed.DataVersion} {parsed.LoadTime}");
                                            versionShown = true;
                                        }
                                        if (!displayed)
                                        {
                                            string name;
                                            if (!names.TryGetValue(device.Id, out name))
                                            {
                                                name = $"Id {device.Id}";
                                            }

                                            Console.WriteLine($"Change in {name} ({device.Id})");
                                            displayed = true;
                                        }
                                        Console.WriteLine($"{property.Name}: {previousValue} to {newValue}");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"New device {device.Id} {device.Name}");
                                if (string.IsNullOrWhiteSpace(device.Name))
                                {
                                    names[device.Id] = $"Id {device.Id}";
                                }
                                else
                                {
                                    names[device.Id] = device.Name;
                                }
                            }
                            latest[device.Id] = device;
                        }
                        loadTime = parsed.LoadTime;
                        dataVersion = parsed.DataVersion;
                    }

                }
            }


        }

        private static async Task<string> TryGetJson(string json, long loadTime, long dataVersion)
        {
            try
            {
                var deltaResponse = await _client.GetAsync($"http://vera.home:3480/data_request?id=sdata&loadtime={loadTime}&dataversion={dataVersion}&timeout=30&minimumdelay=2000");
                json = await deltaResponse.Content.ReadAsStringAsync();
                return json;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}
