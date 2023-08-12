using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Nebular.Bot.Interface;

namespace Nebular.Bot.NebularServer
{

    public static class NebularClient
    {
        private static readonly string url = "https://www.nebularbot.com";

        public static async Task<string> PostRequest(string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var postData = new
                {
                    email = email,
                    password = password
                };
                var values = new Dictionary<string, string>
                  {
                      { "email",email },
                      { "password", password }
                  };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var content2 = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(url + "/api/users/signin", content2);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return "";
                    //throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static async void UpdateUser()
        {
            using (HttpClient client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                  {
                      { "token", LoginForm.Token },
                  };

                var content2 = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("https://www.nebularbot.com/api/users/update", content2);
                if (response.IsSuccessStatusCode)
                {
                   
                }
                else
                {
                    throw new Exception();
                }
            }
        }

    }
}
