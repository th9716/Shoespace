using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class OrderDetail
    {
        public int 無用id { get; set; }
        public string? 訂單編號 { get; set; }
        public int? Id { get; set; }
        public decimal? 商品價格 { get; set; }
        public int? 商品數量 { get; set; }
        public decimal? 總金額 { get; set; }

        public virtual ProductDetail? IdNavigation { get; set; }
        public virtual Order? 訂單編號Navigation { get; set; }
    }
}
