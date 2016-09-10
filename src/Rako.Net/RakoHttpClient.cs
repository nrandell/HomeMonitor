using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rako.Net
{
    public class RakoHttpClient
    {
        private string BridgeAddress {  get; }
        private readonly HttpClient _client = new HttpClient();

        public RakoHttpClient(string bridgeAddress)
        {
            BridgeAddress = bridgeAddress;
        }

        public Task<bool> SendCommand(int room, int channel, int command)
        {
            var url = $"http://{BridgeAddress}/rako.cgi?room={room}&ch={channel}&com={command}";
            return SendCommand(url);

        }

        public Task<bool> SendCommand(int room, int command)
        {
            var url = $"http://{BridgeAddress}/rako.cgi?room={room}&com={command}";
            return SendCommand(url);
        }

        private async Task<bool> SendCommand(string url)
        {
            var response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
                return true;

            } else
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed: {response.StatusCode}: {response.ReasonPhrase}: {result}");
                return false;
            }
        }
    }
}
