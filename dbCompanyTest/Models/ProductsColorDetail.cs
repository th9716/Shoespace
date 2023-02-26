using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class ProductsColorDetail
    {
        public ProductsColorDetail()
        {
            ProductDetails = new HashSet<ProductDetail>();
        }

        public int 商品顏色id { get; set; }
        public string? 商品顏色種類 { get; set; }
        public string? 色碼 { get; set; }
        public string? 商品顏色圖片 { get; set; }

        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}
