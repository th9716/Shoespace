using System.Text.Json.Serialization;
using System.Text.Json;

namespace dbCompanyTest.Models.LineMess.Providers
{
    public class JsonProvider
    {
        private JsonSerializerOptions serializeOption = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        public string Serialize<T>(T obj)//LineBotService
        {
            return JsonSerializer.Serialize(obj, serializeOption);
        }



        private static JsonSerializerOptions deserializeOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        public T Deserialize<T>(string str)//LineBotService
        {
            return JsonSerializer.Deserialize<T>(str, deserializeOptions);
        }
    }
}
