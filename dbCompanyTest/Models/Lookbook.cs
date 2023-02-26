using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class Lookbook
    {
        public int 文章編號 { get; set; }
        public string? 類別名稱 { get; set; }
        public string? 標題 { get; set; }
        public string? 內文 { get; set; }
        public byte[]? 主圖 { get; set; }
        public byte[]? 內圖 { get; set; }
    }
}
