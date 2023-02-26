using dbCompanyTest.Models;

namespace dbCompanyTest.ViewModels
{
    public class ProductDetailViewModels
    {
        public int pro商品編號 { get; set; }
        public int prodetailID { get; set; }
        public int 商品顏色ID { get; set; }
        public int 商品尺寸ID { get; set; }
        public int? collapseParetid { get; set; }
        public string? pro商品名稱 { get; set; }
        public string? pro商品金額 { get; set; }
        public string? pro商品顏色 { get; set; }
        public string? pro商品顏色圖片 { get; set; }
        public string? pro商品分類 { get; set; }
        public int? pro商品分類id { get; set; }
        public string? pro商品材質 { get; set; }
        public string? pro商品介紹 { get; set; }
        public string? pro商品圖片1 { get; set; }
        public string? pro商品圖片2 { get; set; }
        public string? pro商品圖片3 { get; set; }
        public string? 客戶編號 { get; set; }
        public string? 員工編號 { get; set; }
        public List<string>? pro商品顏色圖片list { get; set; }
        public List<string>? pro商品尺寸list { get; set; }
        public List<int>? pro商品DetailIDlist { get; set; }
        public List<int>? pro商品顏色idlist { get; set; }
        public List<int>? pro商品尺寸idlist { get; set; }
        public List<productrandom>? pro商品分類list { get; set; }
        public List<paretCommentclass>? paretCommentslist { get; set; }
        public List<childCommentclass>? childCommentlist { get; set; }
        //ParetComment訊息
        public class paretCommentclass
        {
            public int paretCommentID { get; set; }
            public string? paretCommentGuestID { get; set; }
            public string? paretCommentGuestName { get; set; }
            public string? paretComment { get; set; }
            public DateTime paretCommentDate { get; set; }
        }
        //ChildComment訊息
        public class childCommentclass
        {
            public int childCommentID { get; set; }
            public DateTime childCommentDate { get; set; }
            public string? childCommentGuestID { get; set; }
            public string? childCommentGuestName { get; set; }
            public string? childComment { get; set; }
            public int? childCommentParet { get; set; }
            public int? childCommentchildid { get; set; }
        }
        public class productrandom {

            public int pro商品編號 { get; set; }
            public int 商品顏色ID { get; set; }
            public string? 商品圖片1 { get; set; }
        }
    }
}
