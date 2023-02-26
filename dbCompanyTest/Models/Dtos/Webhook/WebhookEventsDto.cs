namespace dbCompanyTest.Models.LineMess.Dtos.Webhook
{
    public class WebhookEventDto //(Common properties =>https://developers.line.biz/en/reference/messaging-api/#common-properties)
    {//定義出對應到各種情境的參數模型,之後可用物件裏面的屬性來直接對應到使用者輸入的資料(參數)
        public string Type { get; set; } // 事件類型的Identity
        public string Mode { get; set; } // Channel state : active | standby
        public long Timestamp { get; set; } // 事件發生時間 : event occurred time in milliseconds
        public SourceDto Source { get; set; } // 事件來源 : user | group chat | multi-person chat的對象
        public string WebhookEventId { get; set; } // webhook event id - ULID format(唯一標識 Webhook 事件的 ID。是 ULID 格式的字符串)
        public DeliverycontextDto DeliveryContext { get; set; } // 是否為重新傳送之事件 DeliveryContext.IsRedelivery : true(重新傳遞的 webhook 事件) | false(發送第一個 webhook 事件)

        public string? ReplyToken { get; set; } // 回覆此事件所使用的 token,每個token只能使用一次, 收到 webhook 後一分鐘內使用。使用超過一分鐘不保證有效。
        public MessageEventDto? Message { get; set; } // 收到訊息的事件，可收到 text、sticker、image、file、video、audio、location 訊息
        public UnsendEventObjectDto? Unsend { get; set; } //使用者“收回”訊息事件

        //加為好友/解除封鎖屬性跟之前的共用

        public class MemberEventDto
        {
            public List<SourceDto> Members { get; set; }  //Member Join Event //Member Leave Event
        }
        public MemberEventDto? Joined { get; set; } // Memmber Joined Event
        public MemberEventDto? Left { get; set; } // Member Leave Event
        public PostbackEventDto? Postback { get; set; } // Postback Event
        public VideoViewingCompleteEventDto? VideoPlayComplete { get; set; } // Video viewing complete event

    }




    public class UnsendEventObjectDto //收回先前傳送的訊息 for class WebhookEventDto/Message
    {
        public string messageId { get; set; }
    }

    public class DeliverycontextDto  //for class WebhookEventDto/Message
    {
        public bool IsRedelivery { get; set; }// DeliverycontextDto DeliveryContext

    }
    public class SourceDto  //群聊
    {
        public string Type { get; set; }
        public string? UserId { get; set; }//可能只包含 LINE for iOS 和 LINE for Android 的用戶userId
        public string? GroupId { get; set; }
        public string? RoomId { get; set; }
    }

    public class VideoViewingCompleteEventDto //當好友看完這部設定過的影片時，就會產生此事件回傳給我們伺服器
    {
        public string TrackingId { get; set; }
    }
























































    //以下宣告事件類型
    public class MessageEventDto
    {
        public string Id { get; set; }
        public string Type { get; set; }

        // Text Message Event
        public string? Text { get; set; }
        public List<TextMessageEventEmojiDto>? Emojis { get; set; }
        public TextMessageEventMentionDto? Mention { get; set; }


        //圖片
        public ContentProviderDto? ContentProvider { get; set; }
        public ImageMessageEventImageSetDto? ImageSet { get; set; }
        //影片
        public int? Duration { get; set; } // 影片 or 音檔時長(單位：豪秒)//圖片共用

        //註:Audio 用到的屬性跟 Video 重複，直接共用

        //檔案
        public string? FileName { get; set; }
        public int? FileSize { get; set; }

        //Location
        public string? Title { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        //貼圖
        public string? PackageId { get; set; }
        public string? StickerId { get; set; }
        public string? StickerResourceType { get; set; }
        public List<string>? Keywords { get; set; }

    }

    public class ContentProviderDto   //for  class MessageEventDto/ContentProvider 
    {
        public string Type { get; set; }
        public string? OriginalContentUrl { get; set; }
        public string? PreviewImageUrl { get; set; }
    }

    public class ImageMessageEventImageSetDto  //for  class MessageEventDto/ImageSet 
    {
        public string Id { get; set; }
        public string Index { get; set; }
        public string Total { get; set; }
    }











    public class TextMessageEventEmojiDto //Text裡的Emojis使用
    {
        public int Index { get; set; }
        public int Length { get; set; }
        public string ProductId { get; set; }
        public string EmojiId { get; set; }
    }
    //"emojis": [
    //            {
    //                "index": 0,
    //                "length": 7,
    //                "productId": "5fc9d9f650fe1f79ba04f46e",
    //                "emojiId": "023"
    //            }
    //        ]
    //    },













    public class TextMessageEventMentionDto
    {
        public List<TextMessageEventMentioneeDto> Mentionees { get; set; }
    }
    public class TextMessageEventMentioneeDto //訊息標註某人
    {
        public int Index { get; set; }
        public int Length { get; set; } //mentioned user的長度
        public string UserId { get; set; }//標註到的用戶 ID
    }









    public class PostbackEventDto
    {
        public string? Data { get; set; }
        public PostbackEventParamDto? Params { get; set; }
    }

    public class PostbackEventParamDto  //Postback event for date-time selection action and rich menu switch action using this same class togethor
    {
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? Datetime { get; set; }
        public string? NewRichMenuAliasId { get; set; }
        public string? Status { get; set; }
    }











}
