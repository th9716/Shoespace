namespace dbCompanyTest.Models.LineMess.Dtos.Messages.Request
{
    public class ReplyMessageRequestDto<T>
    {
        public string ReplyToken { get; set; }
        public List<T> Messages { get; set; }
        public bool? NotificationDisabled { get; set; } //用戶在發送消息時收到通知,非必要項預設會收到
    }
}
