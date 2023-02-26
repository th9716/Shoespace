namespace dbCompanyTest.ViewModels
{
    public class BACK_ProductViewModels
    {
        public int 商品編號id { get; set; }
        public string? 上架時間 { get; set; }
        public string? 商品名稱 { get; set; }
        public decimal? 商品價格 { get; set; }
        public string? 商品介紹 { get; set; }
        public string? 商品材質 { get; set; }
        public int? 商品重量 { get; set; }
        public decimal? 商品成本 { get; set; }
        public bool? 商品是否有貨 { get; set; }
        public string? 商品分類 { get; set; }
        public string? 商品鞋種 { get; set; }
        public bool? 商品是否上架 { get; set; }

        public IFormFile excel { get; set; }

        public string? File_type { get; set; }

        public string? fileName { get; set; }
    }

    public class Back_GetProName
    {
        public int 商品編號id { get; set; }

        public string? 商品名稱 { get; set; }
    }

    public class Back_ProducDetail
    {
        public string? id { get; set; }
        public string? 商品編號id { get; set; }
        public string? 明細尺寸 { get; set; }
        public string? 顏色 { get; set; }
        public string? 數量 { get; set; }
        public string? 明細編號 { get; set; }
        public string? 商品圖片1 { get; set; }
        public string? 商品圖片2 { get; set; }
        public string? 商品圖片3 { get; set; }
        public string? 圖片位置id { get; set; }
        public string? 商品是否有貨 { get; set; }
        public string? 商品是否上架 { get; set; }

        public string? 商品名稱 { get; set; }

        public string? 分類 { get; set; }
        public string? 鞋種 { get; set; }
        public bool? 新建圖片 { get; set; }


    }

    public class Product_CDictionary
    {        
        public static readonly string SK_SEARCH_PRODUCTS_LIST = "SK_SEARCH_PRODUCTS_LIST";
        public static readonly string SK_SEARCH_PRO_DETAIL_LIST = "SK_SEARCH_PRO_DETAIL_LIST";
    }

    public class Back_Product_library
    {
        //刪除相應圖片
        //刪除圖片位置資料表的相應圖片
        public string DeleteImg(string Path)
        {
           
            string oldPath1 = Path;
            System.IO.File.Delete(oldPath1); //刪掉原本的圖片

            return $"圖片刪除成功";

        }

        //圖片存入方法
        public void SavePhotoMethod(IFormFile p, string proname, string oldname ,string Path)
        {
            string photoName = $"{proname}.jpg";
            string oldPath = Path  + oldname;
            if (oldname != "")
                System.IO.File.Delete(oldPath);   //刪掉圖片
            //string photoName = $"{Guid.NewGuid().ToString()}.jpg";                   

            string path01 = Path  + photoName;  //用環境變數取得 IIS路徑(wwwroot)
            using (FileStream fs = new FileStream(path01, FileMode.Create))
            {
                p.CopyTo(fs);   //將圖片寫入指定路徑      
            }
        }

        //亂數產生器
        public  String RandomString(int length)
        {
            //少了英文的IO和數字10，要避免使用者判斷問題時會使用到
            string allChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            //string allChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";//26個英文字母
            char[] chars = new char[length];
            Random rd = new Random(Guid.NewGuid().GetHashCode());
            rd.Next();
            for (int i = 0; i < length; i++)
            {
                chars[i] = allChars[rd.Next(0, allChars.Length)];
            }

            return new string(chars);
        }

    }


    #region ProductRelevant 使用類別
    //ProductRelevant 圖表使用的類別
    //用來回傳細項商品的銷售額
    public class Back_SellData_s
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal sell { get; set; }
    }
    //回傳資訊較多的細項銷售資料
    public class Back_PD_sumSell
    {
        public int Id { get; set; }
        public string name { get; set; }
        public decimal sell { get; set; }
        public int? 商品編號id { get; set; }
        public int? 商品顏色id { get; set; }
        public int? 商品尺寸id { get; set; }
    }
    //氣泡圖專用類別
    public class Back_proselldata
    {
        public string name { get; set; }
        public decimal value { get; set; }
    }
    public class Back_clsseries
    {
        public string name { get; set; }
        public List<Back_proselldata> data { get; set; }
    }
    public class bubble_infor
    {
        public string title_text { get; set; }
        public string pontformat { get; set; }
        public int filter_value { get; set; }
        public List<Back_clsseries> series { get; set; }
    }

    //Bar圖使用的類別
    public class Bar_chart_info
    {
        public string topcolor { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public List<string> categories { get; set; }
        public string yAxis_title_text { get; set; }
        public string tooltip_valueSuffix { get; set; }
        public int plotOptions_bar_pontwidth { get; set; }

        public Bar_series series { get; set; }
    }
    public class Bar_series_data
    {
        public decimal y { get; set; }
        public string color { get; set; }
    }

    public class Bar_series
    {
        public string name { get; set; }
        public List<Bar_series_data> data { get; set; }
    }

    public class Bar_chart_tempSell
    {
        public string name { get; set; }
        public decimal sellsum { get; set; }

    }

    #endregion ProductRelevant 使用類別


}
