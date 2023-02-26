using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace dbCompanyTest.Environment
{
    public class Environment
    {
        string apiKey = "2LU0i8A48bax1cQNKoRH5OFwfOG_43RaqcDa3yYViTPCgHtG7";
        //string apiKey = "2Lovdq4PT6vffWFYzWQayZWnIuG_3us4Mhfd1e8ik7H5kRSv";//Gary
        //string apiKey = "2LpJcIapenfdC6h3ncC2HXAqC5p_NYh2vTL9ywErA7fMwDCG";2M5SJNJZWktO80snJrUIzRWBLZY_86p28kk3BbqnRzPB6ZFEx
        //string apiKey = "2Lovdq4PT6vffWFYzWQayZWnIuG_3us4Mhfd1e8ik7H5kRSv";//EN
        //string apiKey = "2Lfh6JnTJCDuc3ES58TS4RXDREl_4GWqEQHoZZwEoKEFDTmg5";//LU
        public static bool open = true;
        public static string useEnvironment = "https://localhost:7100";
        public string getEnvironment()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("Get"), "https://api.ngrok.com/endpoints"))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
                    client.DefaultRequestHeaders.Add("Ngrok-Version", "2");
                    HttpResponseMessage response;
                    String x;
                    Root root;
                    try
                    {
                        response = client.SendAsync(request).Result;
                        x = response.Content.ReadAsStringAsync().Result;
                        root = JsonSerializer.Deserialize<Root>(x);
                        if (root.endpoints.Count == 0)
                            return "https://localhost:7100";
                        else
                        {
                            string LineBotKey = "CiB9XbeXDnIXgfN8u7zbtIFGkaxP+VXghErm0tE/bntZJ6M9VZrIKvxUoLT2/38sLsDIXthopd+NwlcX/DT+LJKuOMUp9TJ/VlqVlcrsWjp1cjwFDzaL/2KcN3b+vNRgnP83LrM+iA6QYkFt/VqKiAdB04t89/1O/w1cDnyilFU=";
                            var data = new Dictionary<string, string>()
                                {
                                    { "endpoint", root.endpoints[0].public_url+"/api/LineBot/Webhook" }
                                };
                            HttpClient Line = new HttpClient();
                            HttpRequestMessage lineRequest = new HttpRequestMessage(new HttpMethod("Put"), "https://api.line.me/v2/bot/channel/webhook/endpoint")
                            {
                                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
                            };
                            Line.DefaultRequestHeaders.Add("Authorization", "Bearer " + LineBotKey);
                            var lineResponse = Line.SendAsync(lineRequest).Result;
                            var a = lineResponse.Content.ReadAsStringAsync().Result;
                            useEnvironment = root.endpoints[0].public_url;
                            return useEnvironment;
                        }
                    }
                    catch
                    {
                        useEnvironment = "https://localhost:7100";
                        return useEnvironment;
                    }
                }
            }
        }
    }
}
