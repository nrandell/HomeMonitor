using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mios.Net;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
                RunAsync(CancellationToken.None).GetAwaiter().GetResult();

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

        private static async Task RunAsync(CancellationToken token)
        {
            var response = await _client.GetAsync("http://vera.home:3480/data_request?id=sdata");
            var json = await response.Content.ReadAsStringAsync();
            var parsed = _parser.Parse(json);
            var loadTime = parsed.LoadTime;
            var dataVersion = parsed.DataVersion;

            var names = parsed.Devices.ToDictionary(d => (string)d["id"], d => (string)d["name"]);
            var latest = parsed.Devices.ToDictionary(d => (string)d["id"]);

            while (!token.IsCancellationRequested)
            {
                json = await TryGetJsonAsync(json, loadTime, dataVersion, token);
                if (json != null)
                {
                    var received = DateTimeOffset.UtcNow;
                    parsed = _parser.Parse(json);
                    var versionShown = false;
                    if (parsed.Devices != null)
                    {
                        foreach (var device in parsed.Devices)
                        {
                            JObject changed = null;
                            var deviceId = (string)device["id"];
                            if (latest.TryGetValue(deviceId, out var current))
                            {
                                JArray changes = null;
                                var displayed = false;
                                //foreach (var property in device.GetType().GetProperties().Where(p => p.Name != "Name" && p.Name != "Category"))
                                foreach (var property in device)
                                {
                                    var previousValue = (string)current.GetValue(property.Key);
                                    var newValue = (string)property.Value;
                                    if (!newValue.Equals(previousValue))

                                    //var previousValue = property.GetValue(current);
                                    //var newValue = property.GetValue(device);
                                    //if (!object.Equals(newValue, previousValue))
                                    {
                                        if (changed == null)
                                        {
                                            changes = new JArray();
                                            changed = new JObject();
                                            changed.Add("timestamp", received);
                                            changed.Add("dataVersion", parsed.DataVersion);
                                            changed.Add("loadTime", DateTimeOffset.FromUnixTimeSeconds(parsed.LoadTime));
                                            changed.Add("changes", changes);
                                            changed.Add("device", device);
                                        }
                                        if (!versionShown)
                                        {
                                            //Console.WriteLine($"{DateTime.Now}: Gone to version {parsed.DataVersion} {parsed.LoadTime}");
                                            versionShown = true;
                                        }
                                        if (!displayed)
                                        {
                                            if (!names.TryGetValue(deviceId, out string name))
                                            {
                                                name = $"Id {deviceId}";
                                            }

                                            //Console.WriteLine($"Change in {name} ({deviceId})");
                                            displayed = true;
                                            changed.Add("name", name);
                                        }
                                        //Console.WriteLine($"{property.Name}: {previousValue} to {newValue}");
                                        //Console.WriteLine($"{property.Key}: {previousValue} to {newValue}");
                                        var change = new JObject();
                                        change.Add("property", property.Key);
                                        change.Add("from", current.GetValue(property.Key));
                                        change.Add("to", property.Value);
                                        changes.Add(change);
                                    }
                                }
                            }
                            else
                            {
                                var deviceName = (string)device["name"];
                                //Console.WriteLine($"New device {deviceId} {deviceName}");
                                if (string.IsNullOrWhiteSpace(deviceName))
                                {
                                    names[deviceId] = $"Id {deviceId}";
                                }
                                else
                                {
                                    names[deviceId] = deviceName;
                                }
                                changed = new JObject();
                                changed.Add("timestamp", received);
                                changed.Add("dataVersion", parsed.DataVersion);
                                changed.Add("loadTime", DateTimeOffset.FromUnixTimeSeconds(parsed.LoadTime));
                                changed.Add("device", device);
                                changed.Add("name", deviceName);
                            }
                            latest[deviceId] = device;
                            if (changed != null)
                            {
                                var changedJson = changed.ToString(Formatting.None);
                                Console.WriteLine(changedJson);
                            }
                        }
                        loadTime = parsed.LoadTime;
                        dataVersion = parsed.DataVersion;
                    }

                }
            }


        }

        private static async Task<string> TryGetJsonAsync(string json, long loadTime, long dataVersion, CancellationToken cancel)
        {
            try
            {
                var deltaResponse = await _client.GetAsync($"http://vera.home:3480/data_request?id=sdata&loadtime={loadTime}&dataversion={dataVersion}&timeout=30&minimumdelay=2000", cancel);
                json = await deltaResponse.Content.ReadAsStringAsync();
                return json;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await Task.Delay(2000, cancel);
                return null;
            }
        }
    }
}
