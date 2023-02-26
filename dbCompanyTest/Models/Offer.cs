using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class Offer
    {
        public int 優惠編號 { get; set; }
        public string 優惠名稱 { get; set; } = null!;
        public string 客戶編號 { get; set; } = null!;
        public string? 結束時間 { get; set; }
        public string 數量 { get; set; } = null!;
        public int 折扣 { get; set; }

        public virtual TestClient 客戶編號Navigation { get; set; } = null!;
    }
}
