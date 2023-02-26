using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class ProductsSizeDetail
    {
        public ProductsSizeDetail()
        {
            ProductDetails = new HashSet<ProductDetail>();
        }

        public int 商品尺寸id { get; set; }
        public string? 尺寸種類 { get; set; }

        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}
