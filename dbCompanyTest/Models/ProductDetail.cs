using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class ProductDetail
    {
        public ProductDetail()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int? 商品編號id { get; set; }
        public int? 商品尺寸id { get; set; }
        public int? 商品顏色id { get; set; }
        public int? 商品數量 { get; set; }
        public string? 商品編號 { get; set; }
        public int? 圖片位置id { get; set; }
        public bool? 商品是否有貨 { get; set; }
        public bool? 商品是否上架 { get; set; }

        public virtual ProductsSizeDetail? 商品尺寸 { get; set; }
        public virtual Product? 商品編號Navigation { get; set; }
        public virtual ProductsColorDetail? 商品顏色 { get; set; }
        public virtual 圖片位置? 圖片位置 { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
