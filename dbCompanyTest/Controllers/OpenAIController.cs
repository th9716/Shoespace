using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;

namespace dbCompanyTest.Controllers
{
    public class OpenAIController : Controller
    {
        public IActionResult Index(string question,string? OpenAIKey)
        {
            if (OpenAIKey == null)
                return Json("請提供你的OpenAI's API keys");
            string answer = callOpenAI(1000, question, "text-davinci-003", 0.7, 1, 0, 0, OpenAIKey);
            return Json(answer);
        }

        static string callOpenAI(int tokens, string input, string engine,
          double temperature, int topP, int frequencyPenalty, int presencePenalty, string OpenAIKey)
        {
            var openAiKey = OpenAIKey;
            var apiCall = "https://api.openai.com/v1/engines/" + engine + "/completions";
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), apiCall))
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + openAiKey);
                        request.Content = new StringContent("{\n  \"prompt\": \"" + input + "\",\n  \"temperature\": " +
                                                            temperature.ToString(CultureInfo.InvariantCulture) + ",\n  \"max_tokens\": " + tokens + ",\n  \"top_p\": " + topP +
                                                            ",\n  \"frequency_penalty\": " + frequencyPenalty + ",\n  \"presence_penalty\": " + presencePenalty + "\n}");
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                        var response = httpClient.SendAsync(request).Result;
                        var json = response.Content.ReadAsStringAsync().Result;
                        dynamic dynObj = JsonConvert.DeserializeObject(json);
                        if (dynObj != null)
                        {
                            return dynObj.choices[0].text.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }
    }
}
