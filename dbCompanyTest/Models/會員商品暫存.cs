using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class 會員商品暫存
    {
        public int Id { get; set; }
        public string? 客戶編號 { get; set; }
        public int? 商品編號 { get; set; }
        public string? 商品名稱 { get; set; }
        public string? 尺寸種類 { get; set; }
        public string? 商品顏色種類 { get; set; }
        public int? 訂單數量 { get; set; }
        public decimal? 商品價格 { get; set; }
        public bool? 購物車或我的最愛 { get; set; }
        public string? 圖片1檔名 { get; set; }

        public virtual TestClient? 客戶編號Navigation { get; set; }
    }
}
