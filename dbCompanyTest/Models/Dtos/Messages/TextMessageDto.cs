using dbCompanyTest.Models.LineMess.Enum;

namespace dbCompanyTest.Models.LineMess.Dtos.Messages
{
    public class TextMessageDto : BaseMessageDto
    {
        public TextMessageDto()
        {
            Type = MessageTypeEnum.Text;
        }
        public string Text { get; set; }
    }
}

