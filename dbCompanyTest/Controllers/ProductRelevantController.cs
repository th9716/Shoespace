using dbCompanyTest.Models;
using dbCompanyTest.ViewModels;
using MessagePack.Formatters;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.EventUserModel.DummyRecord;
using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.UserModel;
using System.Text;
using static dbCompanyTest.Controllers.ProductController;

namespace dbCompanyTest.Controllers
{
    public class ProductRelevantController : Controller
    {
        dbCompanyTestContext db = new dbCompanyTestContext();
        private IWebHostEnvironment _eviroment; //宣告取域 環境變數
        private readonly ILogger<ProductController> _logger;  //設定成紀錄類型
        Back_Product_library BP = new Back_Product_library();
        public ProductRelevantController(IWebHostEnvironment eviroment, ILogger<ProductController> logger)  //建構子，將環境變數注入
        {
            _eviroment = eviroment;
            _logger = logger; //注入，才能用session
        }
        //存放Product的資料，搜尋請改用Session
        //static List<Product> searchData = new List<Product>();

        public IActionResult Index()
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
                return RedirectToAction("login", "Staff_Home");
            else
            return View();
        }

        //圖表
        #region 圖表專用
        //明細商品總銷售
        public List<Back_SellData_s> ProductDetail_selll()
        {
            var _tempOD = db.OrderDetails.Join(db.Orders, od => od.訂單編號, o => o.訂單編號, (od, o) => new
            {
                無用id = od.無用id,
                訂單編號 = od.訂單編號,
                Id = od.Id,
                商品價格 = od.商品價格,
                商品數量 = od.商品數量,
                總金額 = od.總金額,
                下單時間 = o.下單時間
            }).Where(t => t.下單時間.Substring(0, 4).Contains("2023"));
            //將PrductDetail 轉為文字
            var data = from pd in db.ProductDetails.ToList()
                       join p in db.Products on pd.商品編號id equals p.商品編號id
                       join z in db.ProductsSizeDetails on pd.商品尺寸id equals z.商品尺寸id
                       join c in db.ProductsColorDetails on pd.商品顏色id equals c.商品顏色id
                       select new
                       {
                           Id = pd.Id,
                           pid = p.商品編號id,
                           明細商品名 = $"{p.商品名稱}_{z.尺寸種類}_{c.商品顏色種類}"
                       };
            var tempD = from p in data
                        join o in _tempOD on p.Id equals o.Id
                        into EmployeeAddressGroup
                        from address in EmployeeAddressGroup.DefaultIfEmpty()
                        group address by new { p.Id,p.明細商品名 } into g
                        select new Back_SellData_s
                        {
                            id = g.Key.Id,                           
                            name = g.Key.明細商品名,
                            sell = g.Sum(o =>
                            {
                                if (o is null)
                                {
                                    return 0;
                                }
                                return o.總金額;
                            }).Value
                        };
            return tempD.ToList();
        }
        //銷售資料與productDetail join
        public List<Back_PD_sumSell> SellJoinProDetail()
        {
            //銷售資料與productDetail join
            List<Back_SellData_s> ProSell = ProductDetail_selll();
            var temp = from s in ProSell
                       join pd in db.ProductDetails on s.id equals pd.Id
                       select new Back_PD_sumSell
                       {
                           Id = s.id,
                           name = s.name,
                           sell = s.sell,
                           商品編號id = pd.商品編號id,
                           商品顏色id = pd.商品顏色id,
                           商品尺寸id = pd.商品尺寸id

                       };
            return temp.ToList();
        }

        //將前10的Bar資料組成Bar圖所需的完整資料
        public Bar_chart_info Build_Bar_chart_info_Top10(List<Bar_chart_tempSell> _temp_sell,string _title,string _subtitle)
        {
            //找出前10名銷售顏色
            var temp_colorSell_Top10 = _temp_sell.OrderByDescending(o => o.sellsum).Take(10).ToList();
            //組出Bar_chart所需的資料
            List<Bar_series_data> _data = temp_colorSell_Top10.Select(t => new Bar_series_data { y = t.sellsum, color = "" }).ToList();
            for (int i = 0; i < temp_colorSell_Top10.Count; i++)
            {
                _data[i].color = colorlist[i];
            }
            Bar_series _series = new Bar_series()
            {
                name = "銷售金額",
                data = _data
            };
            List<string> _categories = temp_colorSell_Top10.Select(t => t.name).ToList();
            Bar_chart_info chart_Info = new Bar_chart_info()
            {
                topcolor = colorlist[0],
                title = _title,
                subtitle = _subtitle,
                categories = _categories,
                yAxis_title_text = "元整 (新台幣 )",
                tooltip_valueSuffix = "元整 (新台幣 )",
                plotOptions_bar_pontwidth = 30,
                series = _series
            };

            return chart_Info;

        }

        //組成Bar圖所需的資料
        string[] colorlist = { "#C8ECFA", "#A9A9AE", "#DCFAC9", "#FFFFC2", "#E6EBFF", "#FFC2E6", "#F8F4A0", "#91F6F5", "#FFC1C1", "#F7FFFF" };
        
        //建構Bar圖
        //Bar_顏色
        public IActionResult color_Barchart()
        {
            var temp = SellJoinProDetail();
            //group by 取得顏色總價
            var temp_colorSell = from t in temp
                                 join c in db.ProductsColorDetails
                                 on t.商品顏色id equals c.商品顏色id
                                 group t by new { t.商品顏色id, c.商品顏色種類 } into g
                                 select new Bar_chart_tempSell
                                 {
                                     name=g.Key.商品顏色種類,
                                     sellsum = g.Sum(g => g.sell)
                                 };
            return Json(Build_Bar_chart_info_Top10(temp_colorSell.ToList(), "Top10 色彩銷售表", "受歡迎的色彩Top10"));
        }
        //Bar_尺寸
        public IActionResult size_Barchart()
        {
            var temp = SellJoinProDetail();
            //group by 取得尺寸總價
            var temp_colorSell = from t in temp
                                 join c in db.ProductsSizeDetails
                                 on t.商品尺寸id equals c.商品尺寸id
                                 group t by new { t.商品尺寸id, c.尺寸種類 } into g
                                 select new Bar_chart_tempSell
                                 {
                                     name = g.Key.尺寸種類,
                                     sellsum = g.Sum(g => g.sell)
                                 };
            return Json(Build_Bar_chart_info_Top10(temp_colorSell.ToList(), "Top10 尺寸銷售表", "受歡迎的尺寸Top10"));
        }

        public IActionResult type_Barchart()
        {
            var temp = SellJoinProDetail();
            //要先與商品編號id group by 取得 每項商品的總銷售
            var ProSumsell = temp.GroupBy(t => t.商品編號id).Select(g => new { 商品編號id = g.Key, sellsum = g.Sum(g => g.sell) });
            //再與Product join 才能與 種類表單group by 
            var tempPro = from t in ProSumsell
                          join c in db.Products
                          on t.商品編號id equals c.商品編號id
                          select new
                          {
                              商品編號id = t.商品編號id,
                              sell = t.sellsum,
                              name = c.商品名稱,
                              商品分類id = c.商品分類id,
                              商品鞋種id = c.商品鞋種id
                          };
            //group by 取得類別總價
            var temp_colorSell = from t in tempPro
                                 join c in db.ProductsTypeDetails
                                 on t.商品分類id equals c.商品分類id
                                 group t by new { t.商品分類id, c.商品分類名稱 } into g
                                 select new Bar_chart_tempSell
                                 {
                                     name = g.Key.商品分類名稱,
                                     sellsum = g.Sum(g => g.sell)
                                 };
            return Json(Build_Bar_chart_info_Top10(temp_colorSell.ToList(), "Top10 類別銷售表", "受歡迎的類別Top10"));
        }

        public IActionResult shose_Barchart()
        {
            var temp = SellJoinProDetail();
            //要先與商品編號id group by 取得 每項商品的總銷售
            var ProSumsell = temp.GroupBy(t => t.商品編號id).Select(g => new { 商品編號id = g.Key, sellsum = g.Sum(g => g.sell) });
            //再與Product join 才能與 種類表單group by 
            var tempPro = from t in ProSumsell
                          join c in db.Products
                          on t.商品編號id equals c.商品編號id
                          select new
                          {
                              商品編號id = t.商品編號id,
                              sell = t.sellsum,
                              name = c.商品名稱,
                              商品分類id = c.商品分類id,
                              商品鞋種id = c.商品鞋種id
                          };
            //group by 取得類別總價
            var temp_colorSell = from t in tempPro
                                 join c in db.商品鞋種s
                                 on t.商品鞋種id equals c.商品鞋種id
                                 group t by new { t.商品鞋種id, c.鞋種 } into g
                                 select new Bar_chart_tempSell
                                 {
                                     name = g.Key.鞋種,
                                     sellsum = g.Sum(g => g.sell)
                                 };
            return Json(Build_Bar_chart_info_Top10(temp_colorSell.ToList(), "Top10 鞋種銷售表", "受歡迎的鞋種Top10"));
        }


        //組成氣泡圖所需資料

        public class bubble_chart_temp_sell
        { 
            public int _id { get; set; }
            public string _name { get; set; }
            public decimal sellsum { get; set; }
            public List<Back_proselldata> _data { get; set; }
           
        }
        public bubble_infor build_bubble_info(List<bubble_chart_temp_sell> _temp,string _title_text)
        {
            //找出前10名銷售顏色
            var temp_colorSell_Top10 = _temp.OrderByDescending(o => o.sellsum).Take(10).ToList();
            //建立series
            List<Back_clsseries> series = new List<Back_clsseries>();
            foreach (var p in temp_colorSell_Top10)
            {
                series.Add(new Back_clsseries() { name = p._name, data = p._data.ToList() });
            }
            //if (series.Count() < 0)
            //    return Json("錯誤_沒有前五名銷售資料!");
            //建立氣泡圖所需完整資料
            bubble_infor bubinf = new bubble_infor()
            {
                title_text = _title_text,
                pontformat = " 元(台幣)",
                filter_value = 10000,
                series = series
            };
            return bubinf;
        }
        //建構氣泡圖所需完整資料
        public IActionResult color_bubblechart()
        {
            var temp = SellJoinProDetail();
            //group by 取得顏色總價
            var temp_colorSell = from t in temp 
                                                            join c in db.ProductsColorDetails 
                                                            on t.商品顏色id equals c.商品顏色id
                                      group t by new { t.商品顏色id,c.商品顏色種類 } into g
                                      select new bubble_chart_temp_sell
                                      {
                                          _id =  g.Key.商品顏色id.Value,
                                          _name= g.Key.商品顏色種類,
                                          sellsum = g.Sum(g=> g.sell),
                                          _data =(from t in temp
                                                      where t.商品顏色id == g.Key.商品顏色id
                                                      select new Back_proselldata
                                                      { 
                                                             name = t.name,
                                                             value = t.sell
                                                      }).ToList()
                                      };

            return Json(build_bubble_info(temp_colorSell.ToList(),"Top 10色彩銷售統計"));
        }

        public IActionResult size_bubblechart()
        {
            var temp = SellJoinProDetail();
            //group by 取得顏色總價
            var temp_colorSell = from t in temp
                                 join c in db.ProductsSizeDetails
                                 on t.商品尺寸id equals c.商品尺寸id
                                 group t by new { t.商品尺寸id, c.尺寸種類 } into g
                                 select new bubble_chart_temp_sell
                                 {
                                     _id = g.Key.商品尺寸id.Value,
                                     _name = g.Key.尺寸種類,
                                     sellsum = g.Sum(g => g.sell),
                                     _data = (from t in temp
                                              where t.商品尺寸id == g.Key.商品尺寸id
                                              select new Back_proselldata
                                              {
                                                  name = t.name,
                                                  value = t.sell
                                              }).ToList()
                                 };

            return Json(build_bubble_info(temp_colorSell.ToList(), "Top 10尺寸銷售統計"));
        }

        public IActionResult type_bubblechart()
        {
            var temp = SellJoinProDetail();
            //要先與商品編號id group by 取得 每項商品的總銷售
            var ProSumsell = temp.GroupBy(t => t.商品編號id).Select(g => new { 商品編號id = g.Key, sellsum = g.Sum(g => g.sell) }) ;
            //再與Product join 才能與 種類表單group by 
            var tempPro = from t in ProSumsell
                          join c in db.Products
                          on t.商品編號id equals c.商品編號id
                          select new
                          {
                              商品編號id = t.商品編號id,
                              sellsum = t.sellsum,
                              name = c.商品名稱,
                              商品分類id = c.商品分類id,
                              商品鞋種id = c.商品鞋種id
                          };
            //group by 取得顏色總價
            var temp_colorSell = from t in tempPro
                                 join c in db.ProductsTypeDetails
                                 on t.商品分類id equals c.商品分類id
                                 group t by new { t.商品分類id, c.商品分類名稱 } into g
                                 select new bubble_chart_temp_sell
                                 {
                                     _id = g.Key.商品分類id.Value,
                                     _name = g.Key.商品分類名稱,
                                     sellsum = g.Sum(g => g.sellsum),
                                     _data = (from t in tempPro
                                              where t.商品分類id == g.Key.商品分類id
                                              select new Back_proselldata
                                              {
                                                  name = t.name,
                                                  value = t.sellsum
                                              }).ToList()
                                 };

            return Json(build_bubble_info(temp_colorSell.ToList(), "Top 10商品分類銷售統計"));
        }

        public IActionResult shoes_bubblechart()
        {
            var temp = SellJoinProDetail();
            //要先與商品編號id group by 取得 每項商品的總銷售
            var ProSumsell = temp.GroupBy(t => t.商品編號id).Select(g => new { 商品編號id = g.Key, sellsum = g.Sum(g => g.sell) });
            //再與Product join 才能與 種類表單group by 
            var tempPro = from t in ProSumsell
                          join c in db.Products
                          on t.商品編號id equals c.商品編號id
                          select new
                          {
                              商品編號id = t.商品編號id,
                              sellsum = t.sellsum,
                              name = c.商品名稱,
                              商品分類id = c.商品分類id,
                              商品鞋種id = c.商品鞋種id
                          };
            //group by 取得顏色總價
            var temp_colorSell = from t in tempPro
                                 join c in db.商品鞋種s
                                 on t.商品鞋種id equals c.商品鞋種id
                                 group t by new { t.商品鞋種id, c.鞋種 } into g
                                 select new bubble_chart_temp_sell
                                 {
                                     _id = g.Key.商品鞋種id.Value,
                                     _name = g.Key.鞋種,
                                     sellsum = g.Sum(g => g.sellsum),
                                     _data = (from t in tempPro
                                              where t.商品鞋種id == g.Key.商品鞋種id
                                              select new Back_proselldata
                                              {
                                                  name = t.name,
                                                  value = t.sellsum
                                              }).ToList()
                                 };

            return Json(build_bubble_info(temp_colorSell.ToList(), "Top 10鞋種銷售統計"));
        }




        #endregion 圖表專用

        //顏色
        #region 顏色
        public IActionResult ColorList()
    {
        var data = db.ProductsColorDetails.ToList();
        return Json(new { data });
    }

    public IActionResult ColorDelete(string id)
    {
        if (string.IsNullOrEmpty(id))
            return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
        int _id = Int32.TryParse(id, out int temp) ? temp : -1;
        if (_id == -1)
            return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);            
        var data = db.ProductsColorDetails.ToList().FirstOrDefault(pd => pd.商品顏色id == _id);
        if (data == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);
        var ProData = db.ProductDetails.ToList().Where(p => p.商品顏色id == _id);
        if(ProData.Count()>0)return Content($"錯誤_還有ProductDetail使用此顏色，不能刪除", "text/plain", Encoding.UTF8);
        //刪除圖片
        if (data.商品顏色圖片 == null)return Content($"錯誤_沒有此圖片資料!", "text/plain", Encoding.UTF8);

        string Path = _eviroment.WebRootPath + "/images/colorimg/" + data.商品顏色圖片;
        (new Back_Product_library()).DeleteImg(Path);

        db.ProductsColorDetails.Remove(data);
        db.SaveChanges();
        return Content($"編號 {data.商品顏色id} 刪除成功", "text/plain", Encoding.UTF8);
    }



    public IActionResult _ColorEdit(string id)
    {
        //因用javascript ajax傳輸 請用 Content("錯誤_請輸入圖片!", "text/plain", Encoding.UTF8);          
        if (string.IsNullOrEmpty(id))
            return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);                 
        int _id = Int32.TryParse(id, out int temp) ? temp : -1;
        if (_id == -1)
            return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);          
        var data = db.ProductsColorDetails.FirstOrDefault(pd => pd.商品顏色id == Int32.Parse(id));
        return PartialView(data);
    }
    [HttpPost]
    public IActionResult ColorEdit(ProductsColorDetail data, IFormFile 商品顏色圖片)
    {

            var PC = db.ProductsColorDetails.FirstOrDefault(pd => pd.商品顏色id == data.商品顏色id);
        if (PC == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);

        //先更改圖片位置
        //建立8位數亂碼
        string cold = (new Back_Product_library()).RandomString(8);
        string imgeName = $"{data.商品顏色id}_{cold}";

        //先建立圖片資料
        if (商品顏色圖片 != null)
        {
            BP.SavePhotoMethod(商品顏色圖片, $"{imgeName}", PC.商品顏色圖片, _eviroment.WebRootPath + "/images/colorimg/");
            PC.商品顏色圖片 = $"{imgeName}.jpg";
        }
        PC.商品顏色種類 = data.商品顏色種類;
        PC.色碼 = data.色碼;
        db.SaveChanges();

        return Content($"編號 {data.商品顏色id} 修改成功", "text/plain", Encoding.UTF8);
    }

    public IActionResult _ColorCreate()
    {
        return PartialView();
    }

    public IActionResult ColorCreate(ProductsColorDetail data, IFormFile 商品顏色圖片)
    {
        //先更改圖片位置
        //建立8位數亂碼           
        string cold = (new Back_Product_library()).RandomString(8);
        int num = db.ProductsColorDetails.Count();
        var N_id = db.ProductsColorDetails.Take(num).Skip(num - 1).FirstOrDefault().商品顏色id;     
        if(N_id==null)
            return Content($"資料讀取錯誤", "text/plain", Encoding.UTF8);
        string imgeName = $"{N_id+1}_{cold}";
        ProductsColorDetail CC = new ProductsColorDetail();
        //先建立圖片資料
        if (商品顏色圖片 != null)
        {
            BP.SavePhotoMethod(商品顏色圖片, $"{imgeName}", "", _eviroment.WebRootPath + "/images/colorimg/");
            CC.商品顏色圖片 = $"{imgeName}.jpg";
        }
        CC.商品顏色id = data.商品顏色id;
        CC.商品顏色種類 = data.商品顏色種類;
        CC.色碼 = data.色碼;
        db.ProductsColorDetails.Add(CC);
        db.SaveChanges();
        // return PartialView();
        return Content($"新增成功", "text/plain", Encoding.UTF8);
    }
    #endregion

        //尺寸
        #region 尺寸
        public IActionResult SizeList()
        {
            var data = db.ProductsSizeDetails.ToList();
            return Json(new { data });
        }

        public IActionResult SizeDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);
            var data = db.ProductsSizeDetails.FirstOrDefault(pd => pd.商品尺寸id == _id);
            if (data == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);
            var PDdata = db.ProductDetails.Where(p => p.商品尺寸id == _id);
            if(PDdata.Count()>0) return Content($"錯誤_還有ProductDetail使用此尺寸，不能刪除", "text/plain", Encoding.UTF8);
            db.ProductsSizeDetails.Remove(data);
            db.SaveChanges();
            return Content($"編號 {data.商品尺寸id} 刪除成功", "text/plain", Encoding.UTF8);
        }

       
        public IActionResult _SizeEdit(string id)
        {
            //因用javascript ajax傳輸 請用 Content("錯誤_請輸入圖片!", "text/plain", Encoding.UTF8);          
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);
            var data = db.ProductsSizeDetails.FirstOrDefault(pd => pd.商品尺寸id == Int32.Parse(id));
            return PartialView(data);
        }
        public IActionResult SizeEdit(ProductsSizeDetail data)
        {
            
            var SE = db.ProductsSizeDetails.FirstOrDefault(pd => pd.商品尺寸id == data.商品尺寸id);
            if (SE == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);

            SE.商品尺寸id = data.商品尺寸id;
            SE.尺寸種類 = data.尺寸種類;
            db.SaveChanges();

            return Content($"編號 {data.商品尺寸id} 修改成功", "text/plain", Encoding.UTF8);
        }

        public IActionResult _SizeCreate()
        {
            return PartialView();
        }

        public IActionResult SizeCreate(ProductsSizeDetail data)
        {
            ProductsSizeDetail SE = new ProductsSizeDetail();            
            SE.尺寸種類 = data.尺寸種類;
            db.ProductsSizeDetails.Add(SE);
            db.SaveChanges();

            return Content($"新建成功", "text/plain", Encoding.UTF8);
        }

        #endregion

        //類別
        #region 類別
        public IActionResult TypeList()
        {
            var data = db.ProductsTypeDetails.ToList();
            return Json(new { data });
        }

        public IActionResult TypeDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);
            var data = db.ProductsTypeDetails.FirstOrDefault(pd => pd.商品分類id == _id);
            if (data == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);
            var Tdata = db.Products.Where(p => p.商品分類id == _id);
            if (Tdata.Count() > 0) return Content($"錯誤_還有Product使用此分類，不能刪除", "text/plain", Encoding.UTF8);
            db.ProductsTypeDetails.Remove(data);
            db.SaveChanges();
            return Content($"編號 {data.商品分類id} 刪除成功", "text/plain", Encoding.UTF8);
        }

        public IActionResult _TypeEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);
            var data = db.ProductsTypeDetails.FirstOrDefault(pd => pd.商品分類id == _id);
            return PartialView(data);
        }

        public IActionResult TypeEdit(ProductsTypeDetail data)
        {

            var PT = db.ProductsTypeDetails.FirstOrDefault(pd => pd.商品分類id == data.商品分類id);
            if (PT == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);

            //PT.商品分類id = data.商品分類id;
            PT.商品分類名稱 = data.商品分類名稱;
            db.SaveChanges();

            return Content($"編號 {data.商品分類id} 修改成功", "text/plain", Encoding.UTF8);
        }

        public IActionResult _TypeCreate()
        {
            return PartialView();
        }

        public IActionResult TypeCreate(ProductsTypeDetail data)
        {

            ProductsTypeDetail PT = new ProductsTypeDetail();

            PT.商品分類id = data.商品分類id;
            PT.商品分類名稱 = data.商品分類名稱;
            db.ProductsTypeDetails.Add(PT);
            db.SaveChanges();

            return Content($"新建成功", "text/plain", Encoding.UTF8);
        }
        #endregion

        //鞋種
        #region 鞋種
        public IActionResult ShoesList()
        {
            var data = db.商品鞋種s.ToList();
            return Json(new { data });
        }

        public IActionResult _ShoesCreate()
        {
            return PartialView();
        }

        public IActionResult ShoesCreate(商品鞋種 data)
        {

            商品鞋種 SD= new 商品鞋種();

            //SD.商品鞋種id = data.商品鞋種id;
            SD.鞋種 = data.鞋種;
            db.商品鞋種s.Add(SD);
            db.SaveChanges();

            return Content($"新建成功", "text/plain", Encoding.UTF8);
        }


        public IActionResult _ShoesEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);
            var data = db.商品鞋種s.FirstOrDefault(pd => pd.商品鞋種id == _id);
            return PartialView(data);
        }

        public IActionResult ShoesEdit(商品鞋種 data)
        {

            var SD = db.商品鞋種s.FirstOrDefault(pd => pd.商品鞋種id == data.商品鞋種id);
            if (SD == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);

            //SD.商品鞋種id = data.商品鞋種id;
            SD.鞋種 = data.鞋種;
            db.SaveChanges();

            return Content($"編號 {data.商品鞋種id} 修改成功", "text/plain", Encoding.UTF8);
        }

        public IActionResult ShoesDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp) ? temp : -1;
            if (_id == -1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);
            var data = db.商品鞋種s.FirstOrDefault(pd => pd.商品鞋種id == _id);
            if (data == null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);
            var SHdata = db.Products.Where(p => p.商品鞋種id == _id);
            if (SHdata.Count() > 0) return Content($"錯誤_還有Product使用此鞋種，不能刪除", "text/plain", Encoding.UTF8);
            db.商品鞋種s.Remove(data);
            db.SaveChanges();
            return Content($"編號 {data.商品鞋種id} 刪除成功", "text/plain", Encoding.UTF8);
        }

        #endregion
    }
}
