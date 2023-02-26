using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class 圖片位置
    {
        public 圖片位置()
        {
            ProductDetails = new HashSet<ProductDetail>();
        }

        public int 圖片位置id { get; set; }
        public string? 商品圖片1 { get; set; }
        public string? 商品圖片2 { get; set; }
        public string? 商品圖片3 { get; set; }

        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}
