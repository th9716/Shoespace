using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public string 訂單編號 { get; set; } = null!;
        public string? 客戶編號 { get; set; }
        public string? 付款方式 { get; set; }
        public string? 送貨地址 { get; set; }
        public decimal? 總金額 { get; set; }
        public string? 下單時間 { get; set; }
        public string? 訂單狀態 { get; set; }
        public string? 付款狀態 { get; set; }
        public string? 收件人名稱 { get; set; }
        public string? 收件人電話 { get; set; }
        public string? 收件人email { get; set; }

        public virtual TestClient? 客戶編號Navigation { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
