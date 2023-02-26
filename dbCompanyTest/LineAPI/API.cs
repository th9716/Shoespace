using System.Security.Cryptography;

namespace dbCompanyTest.LineAPI
{
    public class API
    {
        public string channelSecret = "e8d9769ef718d9866e73f0888416e393";
        public string nonce = Guid.NewGuid().ToString();
        public string requestUri = "/v3/payments/request";
        public readonly string linePayBaseApiUrl = "https://sandbox-api-pay.line.me";
        public class Product
        {
            public string name { get; set; }
            public int quantity { get; set; }
            public int price { get; set; }
        }

        public class Package
        {
            public string id { get; set; }
            public int amount { get; set; }
            public string name { get; set; }
            public List<Product> products { get; set; }
        }

        public class RedirectUrls
        {
            public string confirmUrl { get; set; }
            public string cancelUrl { get; set; }
        }

        public class LineForm
        {
            public int amount { get; set; }
            public string currency { get; set; }
            public string orderId { get; set; }
            public List<Package> packages { get; set; }
            public RedirectUrls redirectUrls { get; set; }
        }
        public static string LinePayHMACSHA256(string key, string message)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);

            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);

            byte[] messageBytes = encoding.GetBytes(message);
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);

            //注意他原本的公式是直接轉為string
            return Convert.ToBase64String(hashmessage);
        }

    }
}
