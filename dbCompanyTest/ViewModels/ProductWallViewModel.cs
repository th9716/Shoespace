using dbCompanyTest.Models;

namespace dbCompanyTest.ViewModels
{
    public class ProductWallViewModel
    {
        public string 商品分類名稱 { get; set; }
        public string 鞋種名稱 { get; set; }
        public int 商品id { get; set; }
        public int 商品分類id { get; set; }
        public int 商品鞋種id { get; set; }
        public int 商品顏色id { get; set; }
        public string 顏色名稱 { get; set; }
        public string 尺寸名稱 { get; set; }
        public string 材質名稱 { get; set; }
        public string 商品名稱 { get; set; }
        public decimal  商品價格 { get; set; }
        public string 產品圖片1 { get; set; }
        public string 產品圖片2 { get; set; }
        public string keyword { get; set; }

    }
}
