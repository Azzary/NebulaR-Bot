// <KeepClasses>
// DoNotObfuscate: Nebular.Zaap.ZaapAPI.ZaapConnect
// </KeepClasses>


using Nebular.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Zaap.ZaapAPI
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class ZaapConnect
    {

        private static string certificate_hash = null;
        private static string certificate_id = null;

        [Obfuscation(Exclude = true)]
        public static async Task<string> get_ApiKey(string login, string password, string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                return key;
            }

            WinHttpHandler handler = new WinHttpHandler();
            using (HttpClient cliente_web = new HttpClient(handler))
            {
                HttpRequestMessage req = new HttpRequestMessage
                {
                    RequestUri = new Uri("https://haapi.ankama.com/json/Ankama/v5/Api/CreateApiKey"),
                    Method = HttpMethod.Post,
                    Headers =
                    {
                        { "user-agent", "Zaap 3.9.6" }
                    },
                    Content = new StringContent($"login={login}&password={password}&game_id=102" +
                    $"&long_life_token=true&shop_key=ZAAP&payment_mode=OK&lang=fr" +
                    $"&certificate_id={certificate_id}" +
                    $"&certificate_hash={certificate_hash}", Encoding.UTF8, "text/plain")
                };

                 HttpResponseMessage response = await cliente_web.SendAsync(req);
                if (response.IsSuccessStatusCode)
                {
                    Dictionary<string, object> get_ApiResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<Dictionary<string, object>>(await response.Content.ReadAsStreamAsync());
                    key = get_ApiResponse?["key"].ToString();
                    //certificate_id = get_ApiResponse?["certificate_id"].ToString();
                    try
                    {
                        var file = File.ReadAllText("accounts.json");
                        var Accounts = JsonConvert.DeserializeObject<List<DofusAccount>>(file);
                        var selectedAccount = Accounts.FirstOrDefault(x => x.Email == login);
                        if (!string.IsNullOrEmpty(key) && selectedAccount != null)
                        {
                            selectedAccount.Key = key;
                            string jsonContent = JsonConvert.SerializeObject(Accounts);
                            File.WriteAllText("accounts.json", jsonContent);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    return get_ApiResponse?["key"].ToString();
                }

                return null;
            }
        }

        [Obfuscation(Exclude = true)]

        public static async Task<string> refresh_ApiKey(string apiKey)
        {
            WinHttpHandler handler = new WinHttpHandler();


            using (HttpClient cliente_web = new HttpClient(handler))
            {
                HttpRequestMessage req = new HttpRequestMessage
                {
                    RequestUri = new Uri("https://haapi.ankama.com/json/Ankama/v5/Api/RefreshApiKey"),
                    Method = HttpMethod.Post,
                    Headers =
                    {
                        { "apiKey", $"{apiKey}" },
                        { "user-agent", "Zaap 3.9.6" }
                    }
                };

                HttpResponseMessage response = await cliente_web.SendAsync(req);
                if (response.IsSuccessStatusCode)
                {
                    Dictionary<string, object> get_ApiResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<Dictionary<string, object>>(await response.Content.ReadAsStreamAsync());
                    return get_ApiResponse?["key"].ToString();
                }

                return null;
            }
        }

        [Obfuscation(Exclude = true)]
        public static async Task<string> get_Token(string apiKey, EnumClientType clientType = EnumClientType.Dofus2)
        {
            await refresh_ApiKey(apiKey);
            WinHttpHandler handler = new WinHttpHandler();

            using (HttpClient cliente_web = new HttpClient(handler))
            {
                HttpRequestMessage req = new HttpRequestMessage
                {
                    RequestUri = new Uri($"https://haapi.ankama.com/json/Ankama/v5/Account/CreateToken?game={((int)clientType)}&certificate_id={certificate_id}&certificate_hash={certificate_hash}"),
                    Method = HttpMethod.Get,
                    Headers =
                    {
                        { "apiKey", $"{apiKey}" },
                        { "user-agent", "Zaap 3.9.6" }
                    }
                };

                HttpResponseMessage response = await cliente_web.SendAsync(req);
                if (response.IsSuccessStatusCode)
                {
                    Dictionary<string, object> get_ApiResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<Dictionary<string, object>>(await response.Content.ReadAsStreamAsync());
                    return get_ApiResponse?["token"].ToString();
                }

                return null;
            }
        }
    }
}