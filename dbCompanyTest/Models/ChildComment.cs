using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class ChildComment
    {
        public int 訊息id { get; set; }
        public string? 客戶編號 { get; set; }
        public string? 客戶姓名 { get; set; }
        public string? 內容 { get; set; }
        public DateTime? 建立日期 { get; set; }
        public int? 父訊息id { get; set; }
        public int? 子訊息id { get; set; }

        public virtual ParentComment? 父訊息 { get; set; }
    }
}
