using dbCompanyTest.Hubs;
using dbCompanyTest.Models.LineMess.Domain;
using dbCompanyTest.Models.LineMess.Dtos.Webhook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;

namespace dbCompanyTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineBot : ControllerBase
    {
        private readonly string channelAccessToken = "CiB9XbeXDnIXgfN8u7zbtIFGkaxP+VXghErm0tE/bntZJ6M9VZrIKvxUoLT2/38sLsDIXthopd+NwlcX/DT+LJKuOMUp9TJ/VlqVlcrsWjp1cjwFDzaL/2KcN3b+vNRgnP83LrM+iA6QYkFt/VqKiAdB04t89/1O/w1cDnyilFU=";//TOP>SheoseGift>ShoeseGift>Messaging API>Channel access token (long-lived)
        private readonly string channelSecret = "dd0a693282da9bd4a90aa2c837787648"; //TOP>SheoseGift>ShoeseGift>Basic settings>Channel secret

        private readonly LineBotService _lineBotService;

        public LineBot()
        {
            _lineBotService = new LineBotService();

        }
        [HttpPost("SendMessage/Broadcast")]//發送廣播訊息
        public IActionResult Broadcast([Required] string messageType, object body)
        {
            _lineBotService.BroadcastMessageHandler(messageType, body);
            return Ok();
            //{"Messages":[{"Type":"text","Text":"廣播測試"}]}
        }





        [HttpPost/*("SendMessage/send")*/]//私訊 需輸入Id
        public IActionResult SendLineMessage(string LINEID, string msg)
        {
            //string body1 = "            {\r\n                \"to\":\"U4dd930b9e38f7c7e6f8ef483c417100b\",\r\n            \"Messages\":[{ \"Type\":\"text\",\"Text\":\"私訊測試\"}]\r\n            }";

            _lineBotService.PushMessageHandler("text",
                $"{{\r\n\"to\":\"{LINEID}\",\r\n\"Messages\":[{{ \"Type\":\"text\",\"Text\":\"{msg}\"}}]\r\n}}");
            return Ok();
            //{
            //    "to":"U4dd930b9e38f7c7e6f8ef483c417100b",
            //"Messages":[{ "Type":"text","Text":"私訊測試"}]
            //}
        }





        [HttpPost("Webhook")]
        public IActionResult Webhook(WebhookRequestBodyDto body)
        {
            _lineBotService.ReceiveWebhook(body);

            return Ok();
        }
    }
}
//ngrok連線要打完整(ngrok http https://localhost:7110)
//如果遇到ERR_NGROK_108 => cmd : taskkill /f /im ngrok.exe
//2LpJcIapenfdC6h3ncC2HXAqC5p_NYh2vTL9ywErA7fMwDCG//En