namespace dbCompanyTest.Models.LineMess.Dtos.Webhook
{
    public class WebhookRequestBodyDto //對應到LINE訊息的完整結構，json最外層
    {//對應到LINE訊息的完整結構，同時也是最外層的結構，接收時上面最外層的兩筆資料
        public string? Destination { get; set; }
        public List<WebhookEventDto> Events { get; set; }
    }
}
