using static dbCompanyTest.LineAPI.PaymentConfirmResponseDto;
using System.Text.Json;
using System.Text;
using static dbCompanyTest.LineAPI.API;
using LinedbCompanyTestDemo.LineAPI;

namespace dbCompanyTest.LineAPI
{
    public class LinePayService
    {
        //public LinePayService()
        //{
        private readonly string channelId = "1657887518";
        private readonly string channelSecretKey = "e8d9769ef718d9866e73f0888416e393";
        private readonly string linePayBaseApiUrl = "https://sandbox-api-pay.line.me";

        //private static HttpClient client;
        //private readonly JsonProvider _jsonProvider;
        public async Task<PaymentResponseDto> SendPaymentRequest(PaymentRequestDto dto)
        {
            HttpClient client = new HttpClient();
            var json = JsonSerializer.Serialize(dto);
            // 產生 GUID Nonce
            var nonce = Guid.NewGuid().ToString();
            // 要放入 signature 中的 requestUrl
            var requestUrl = "/v3/payments/request";

            //使用 channelSecretKey & requestUrl & jsonBody & nonce 做簽章
            var signature = SignatureProvider.HMACSHA256(channelSecretKey, channelSecretKey + requestUrl + json + nonce);

            var request = new HttpRequestMessage(HttpMethod.Post, linePayBaseApiUrl + requestUrl)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            // 帶入 Headers
            client.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
            client.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
            client.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);
            var response = await client.SendAsync(request);//
            var linePayResponse = JsonSerializer.Deserialize<PaymentResponseDto>(await response.Content.ReadAsStringAsync());

            return linePayResponse;
        }

        public async Task<PaymentConfirmResponseDto> ConfirmPayment(string transactionId, string orderId, PaymentConfirmDto dto) //加上 OrderId 去找資料
        {
            HttpClient client = new HttpClient();

            var json = JsonSerializer.Serialize(dto);
            var nonce = Guid.NewGuid().ToString();
            var requestUrl = string.Format("/v3/payments/{0}/confirm", transactionId);
            var signature = SignatureProvider.HMACSHA256(channelSecretKey, channelSecretKey + requestUrl + json + nonce);

            var request = new HttpRequestMessage(HttpMethod.Post, String.Format(linePayBaseApiUrl + requestUrl, transactionId))
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            client.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
            client.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
            client.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);

            var response = await client.SendAsync(request);
            var responseDto = JsonSerializer.Deserialize<PaymentConfirmResponseDto>(await response.Content.ReadAsStringAsync());
            return responseDto;
        }
    }
}

