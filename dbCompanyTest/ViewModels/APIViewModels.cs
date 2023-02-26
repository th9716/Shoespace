using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace dbCompanyTest.ViewModels
{
    public class APIViewModels
    {
        public string GetCheckMacValueForOrder(Dictionary<string, string> order)
        {
            return GetCheckMacValue_Order(order);
        }
        public string GetCheckMacValuecon(Dictionary<string, string> order) 
        {
            return GetCheckMacValue(order);
        }
        private string GetCheckMacValue(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();

            var checkValue = string.Join("&", param);

            //測試用的 HashKey
            var hashKey = "5294y06JbISpM5x9";

            //測試用的 HashIV
            var HashIV = "v77hoKGq4kWxNNIS";

            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";

            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();

            checkValue = GetSHA256(checkValue);

            return checkValue.ToUpper();
        }

        private string GetSHA256(string value)
        {
            var result = new StringBuilder();
            var sha256 = SHA256.Create();
            var bts = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            return result.ToString();
        }
        //public ActionResult PayInfo(string id)
        //{
        //    //var cache = MemoryCache.Default;
        //    //var cacheData = cache.Get(id);
        //    var dataStr = JsonConvert.SerializeObject(id);
        //    var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataStr);
        //    return View("EcpayView", data);
        //}

        private string GetCheckMacValue_Order(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();

            var checkValue = string.Join("&", param);

            //測試用的 HashKey
            var hashKey = "5294y06JbISpM5x9";

            //測試用的 HashIV
            var HashIV = "v77hoKGq4kWxNNIS";

            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";

            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();

            checkValue = GetMD5(checkValue);

            return checkValue.ToUpper();
        }

        private string GetMD5(string value)
        {
            var result = new StringBuilder();
            var sha256 = MD5.Create();
            var bts = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            return result.ToString();
        }
    }
}
