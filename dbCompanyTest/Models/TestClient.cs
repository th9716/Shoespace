using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class TestClient
    {
        public TestClient()
        {
            Offers = new HashSet<Offer>();
            Orders = new HashSet<Order>();
            會員商品暫存s = new HashSet<會員商品暫存>();
        }

        public string 客戶編號 { get; set; } = null!;
        public string? 客戶姓名 { get; set; }
        public string? 客戶電話 { get; set; }
        public string? 身分證字號 { get; set; }
        public string? 縣市 { get; set; }
        public string? 區 { get; set; }
        public string? 地址 { get; set; }
        public string? Email { get; set; }
        public string? 密碼 { get; set; }
        public string? 性別 { get; set; }
        public string? 生日 { get; set; }

        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<會員商品暫存> 會員商品暫存s { get; set; }
    }
}
