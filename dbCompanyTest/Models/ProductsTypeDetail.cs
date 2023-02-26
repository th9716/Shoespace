using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class ProductsTypeDetail
    {
        public ProductsTypeDetail()
        {
            Products = new HashSet<Product>();
        }

        public int 商品分類id { get; set; }
        public string? 商品分類名稱 { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
