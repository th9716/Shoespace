using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class ParentComment
    {
        public ParentComment()
        {
            ChildComments = new HashSet<ChildComment>();
        }

        public int 訊息id { get; set; }
        public string? 客戶編號 { get; set; }
        public string? 客戶姓名 { get; set; }
        public int? 商品編號id { get; set; }
        public int? 商品顏色id { get; set; }
        public string? 內容 { get; set; }
        public DateTime? 建立日期 { get; set; }

        public virtual ICollection<ChildComment> ChildComments { get; set; }
    }
}
