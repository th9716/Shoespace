using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductDetails = new HashSet<ProductDetail>();
        }

        public int 商品編號id { get; set; }
        public string? 上架時間 { get; set; }
        public string? 商品名稱 { get; set; }
        public decimal? 商品價格 { get; set; }
        public string? 商品介紹 { get; set; }
        public string? 商品材質 { get; set; }
        public int? 商品重量 { get; set; }
        public decimal? 商品成本 { get; set; }
        public bool? 商品是否有貨 { get; set; }
        public int? 商品分類id { get; set; }
        public int? 商品鞋種id { get; set; }
        public bool? 商品是否上架 { get; set; }

        public virtual ProductsTypeDetail? 商品分類 { get; set; }
        public virtual 商品鞋種? 商品鞋種 { get; set; }
        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}
