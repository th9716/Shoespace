using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class ToDoList
    {
        public int 交辦事項id { get; set; }
        public string 員工編號 { get; set; } = null!;
        public string? 表單類型 { get; set; }
        public string 表單內容 { get; set; } = null!;
        public string? 回覆 { get; set; }
        public string 表單狀態 { get; set; } = null!;
        public string? 起單時間 { get; set; }
        public string? 起單人 { get; set; }
        public string? 部門主管 { get; set; }
        public string? 部門主管簽核 { get; set; }
        public string? 部門主管簽核意見 { get; set; }
        public string? 部門主管簽核時間 { get; set; }
        public string? 協辦部門 { get; set; }
        public string? 協辦部門簽核 { get; set; }
        public string? 協辦部門簽核人員 { get; set; }
        public string? 協辦部門簽核意見 { get; set; }
        public string? 協辦部門簽核時間 { get; set; }
        public string? 老闆簽核 { get; set; }
        public string? 老闆簽核意見 { get; set; }
        public string? 老闆簽核時間 { get; set; }
        public string? 執行人 { get; set; }
        public string? 執行時間 { get; set; }
        public string? 執行人簽核 { get; set; }
        public string? 附件 { get; set; }
        public string? 附件path { get; set; }

        public virtual TestStaff? 員工編號Navigation { get; set; }
    }
}
