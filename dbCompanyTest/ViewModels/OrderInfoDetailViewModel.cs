namespace dbCompanyTest.ViewModels
{
    public class OrderInfoDetailViewModel
    {
        public string 訂單編號 { get; set; }
        public string 商品名 { get; set; }
        public string 圖片 { get; set; }
        public string 規格 { get; set; }
        public int? 數量 { get; set; }
        public decimal? 價格 { get; set; }
        public string 付款方式 { get; set; }
        public string 送貨地址 { get; set; }
        public decimal? 總金額 { get; set; }
        public int 商品id { get; set; }
        public int 商品顏色id { get; set; }

    }
}
