using dbCompanyTest.Models;
using dbCompanyTest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.Util;
using System.Data;
using System.Xml.Linq;
using System.ComponentModel;
using System.Text.Json;
using System.Text;
using NPOI.OpenXmlFormats.Dml;
using static NPOI.HSSF.UserModel.HeaderFooter;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.HPSF;
using Org.BouncyCastle.Crypto.Macs;
using System.Collections.Generic;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using Microsoft.VisualBasic;

namespace dbCompanyTest.Controllers
{
    public class ProductController : Controller
    {
        dbCompanyTestContext db = new dbCompanyTestContext();
        private IWebHostEnvironment _eviroment; //宣告取域 環境變數
        private readonly ILogger<ProductController> _logger;  //設定成紀錄類型

        public ProductController(IWebHostEnvironment eviroment, ILogger<ProductController> logger)  //建構子，將環境變數注入
        {
            _eviroment = eviroment;
            _logger = logger; //注入，才能用session
        }
        //存放Product的資料，搜尋請改用Session
        //static List<Product> searchData = new List<Product>();

        //powerBI

        public IActionResult Pro_PowerBI_Inventory_profit()
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
                return RedirectToAction("login", "Staff_Home");
            else
            return View();
        }
        public IActionResult ProSell_PowerBI_show()
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
                return RedirectToAction("login", "Staff_Home");
            else
                return View();

        }

        public IActionResult ProSell_PowerBI_member_Sell()
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
                return RedirectToAction("login", "Staff_Home");
            else
                return View();
        }



        #region 圖表相關
        //圖表使用
        //前五產品 Pie 圖
        public class data
        {
            public string name { get; set; }
            public decimal y { get; set; }
        }

        public class Pie_sellData
        {
            public List<data> PieData { get; set; }
            public List<decimal> T5Sell { get; set; }
            public decimal allSell { get; set; }
            public string year { get; set; }
        }

        public IActionResult Pei_ProSell()
        {
            List<data> AllData = new List<data>();

            //當前的日期
            DateTime thisDay = DateTime.Today;
            //當前年份
            string thisyear = thisDay.Year.ToString();
            // oder 與 orderDetail  先join 可取得時間過濾後的訂單資料
            var _tempOD = db.OrderDetails.Join(db.Orders, od => od.訂單編號, o => o.訂單編號, (od, o) => new
            {
                無用id = od.無用id,
                訂單編號 = od.訂單編號,
                Id = od.Id,
                商品價格 = od.商品價格,
                商品數量 = od.商品數量,
                總金額 = od.總金額,
                下單時間 = o.下單時間
            });
            //加入商品編號id
            var _tempOD2 = _tempOD.Join(db.ProductDetails, od => od.Id, p => p.Id, (od, p) => new
            {
                無用id = od.無用id,
                訂單編號 = od.訂單編號,
                Id = od.Id,
                商品價格 = od.商品價格,
                商品數量 = od.商品數量,
                總金額 = od.總金額,
                下單時間 = od.下單時間,
                商品編號id = p.商品編號id
            });


            //沒有2023資料,只能先用2022
            var tempOD2 = _tempOD2.Where(t => t.下單時間.Substring(0, 4).Contains("2023"));
            //計算總收益     
            decimal totalAll = _tempOD2.Sum(od => od.總金額).Value;
            //Left Join 要改成ProductDetail 的 ID 
            var tempD = from p in db.Products
                        join od in tempOD2 on p.商品編號id equals od.商品編號id
                       into EmployeeAddressGroup
                        from address in EmployeeAddressGroup.DefaultIfEmpty()
                        group address by new { p.商品編號id, p.商品名稱 } into g
                        select new data
                        {
                            name = g.Key.商品名稱.ToString(),
                            y = g.Sum(od => od.總金額).Value
                        };
            AllData = tempD.ToList();
            //取得前5筆加總資料

            var ALLsellTop5 = AllData.OrderByDescending(o => o.y).Take(5).ToList();

            List<decimal> _T5Sell = ALLsellTop5.Select(a => a.y).ToList();
            //將前五名減去-得到其他收益
            decimal total = ALLsellTop5.Sum(a => a.y);
            decimal othersell = totalAll - total;

            //將data 的y轉為 百分比
            //var top5 = ALLsellTop5.Select(a => new data { name = a.name, y =(a.y)  }).ToList();
            var top5 = ALLsellTop5.Select(a => new data { name = a.name, y = Math.Round(((a.y) / totalAll) * 100, 0) }).ToList();
            //因Pei 的比例 一定要=100,故須做預防處裡,用最大的值減去誤差
            if (top5.Sum(d => d.y) > 100)
            {
                decimal max = top5.Max(t => t.y);
                var model = top5.Where(t => t.y == max).FirstOrDefault();
                if (model != null)
                    model.y = model.y - (top5.Sum(d => d.y) - 100);
            }
            //將其他加入TOP5的資料
            //top5.Add(new data { name = "其他", y = othersell  });
            top5.Add(new data { name = "其他", y = Math.Round((othersell / totalAll) * 100, 0) });
            Pie_sellData Pie_D = new Pie_sellData()
            {
                PieData = top5,
                T5Sell = _T5Sell,
                allSell = totalAll,
                year = "2023"
            };

            return Json(Pie_D);
        }
        //立體柱狀圖 前10名商品
        public class column_sellData
        {
            public List<string> categories { get; set; }
            public List<decimal> datas { get; set; }
            public decimal allSell { get; set; }
            public string year { get; set; }
        }
        public IActionResult Top10ProSellCol()
        {
            //先取得訂單時間 2022年資料
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
                           明細商品名 = $"{p.商品名稱}_{z.尺寸種類}_{c.商品顏色種類}"
                       };
            var tempD = from p in data
                        join o in _tempOD on p.Id equals o.Id
                        into EmployeeAddressGroup
                        from address in EmployeeAddressGroup.DefaultIfEmpty()
                        group address by new { p.Id, p.明細商品名 } into g
                        select new
                        {
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
            var ALLsellTop10 = tempD.OrderByDescending(o => o.sell).Take(10).ToList();
            List<string> _categories = ALLsellTop10.Select(t => t.name.ToString()).ToList();
            List<decimal> _datas = ALLsellTop10.Select(t => t.sell).ToList();
            column_sellData colData = new column_sellData()
            {
                categories = _categories,
                datas = _datas,
                allSell = _tempOD.Sum(od => od.總金額).Value,
                year = "2023"
            };
            return Json(colData);
        }


        #endregion 圖表相關

        public IActionResult Index()
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
                return RedirectToAction("login", "Staff_Home");
            else
            return View();
        }

        #region 下載與批次上傳所有事件

        [HttpPost]
        public IActionResult Upload(BACK_ProductViewModels vc)
        {
            if (vc.excel != null)
            {
                string excelname = vc.excel.Name;
                string oldPath = _eviroment.WebRootPath + "/Datas/" + excelname;
                if (string.IsNullOrEmpty(vc.File_type))
                    return Json("錯誤_遺失存入資料表代號");


                System.IO.File.Delete(oldPath);   //刪掉原本的檔案

                //string photoName = $"{Guid.NewGuid().ToString()}.jpg";                   

                string path01 = _eviroment.WebRootPath + "/Datas/" + excelname;  //用環境變數取得 IIS路徑(wwwroot)
                using (FileStream fs = new FileStream(path01, FileMode.Create))
                {
                    vc.excel.CopyTo(fs);   //將檔案寫入指定路徑      
                }
                using (FileStream fs = new FileStream(path01, FileMode.Open))
                {
                    method_uploadEx(fs, vc.excel,vc.File_type);
                }
            }
            return RedirectToAction("Index");
        }

        //將檔案與資料流轉成DataTable並存入資料庫
        public void method_uploadEx(Stream stream, IFormFile formFile,string FileType)
        {
            DataTable dataTable = new DataTable();
            IWorkbook wb;
            ISheet sheet;
            IRow headerRow;
            int cellCount; //紀錄共有幾欄
            try
            {
                //依excel版本，NPOI載入檔案
                if (formFile.FileName.ToUpper().EndsWith("XLSX"))
                    wb = new XSSFWorkbook(stream); // excel版本(.xlsx)
                else
                    wb = new HSSFWorkbook(stream); // excel版本(.xls)

                //取第一個頁籤   
                sheet = wb.GetSheetAt(0);

                //取第一個頁籤的第一列
                headerRow = sheet.GetRow(0);

                //計算出第一列共有多少欄位
                cellCount = headerRow.LastCellNum;

                //迴圈執行第一列的第一個欄位到最後一個欄位，將抓到的值塞進DataTable做完欄位名稱
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    dataTable.Columns.Add(new DataColumn(headerRow.GetCell(i).StringCellValue));
                }

                //int j; //計算每一列讀到第幾個欄位
                int column = 1; //計算每一列讀到第幾個欄位

                // 略過第零列(標題列)，一直處理至最後一列
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    //取目前的列(row)
                    IRow row = sheet.GetRow(i);

                    //若該列的第一個欄位無資料，break跳出
                    if (string.IsNullOrEmpty(row.Cells[0].ToString().Trim()))
                    {
                        break;
                    }

                    //宣告DataRow
                    DataRow dataRow = dataTable.NewRow();
                    //宣告ICell
                    ICell cell;

                    try
                    {
                        //依先前取得，依每一列的欄位數，逐一設定欄位內容
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            //計算每一列讀到第幾個欄位(秀在錯誤訊息上)
                            column = j + 1;

                            //設定cell為目前第j欄位
                            cell = row.GetCell(j);

                            if (cell != null) //若cell有值
                            {
                                //用cell.CellType判斷資料的型別
                                //再依照欄位屬性，用StringCellValue、DateCellValue、NumericCellValue、DateCellValue取值
                                switch (cell.CellType)
                                {
                                    //字串型態欄位
                                    case NPOI.SS.UserModel.CellType.String:
                                        //設定dataRow第j欄位的值，cell以字串型態取值
                                        dataRow[j] = cell.StringCellValue;
                                        break;

                                    //數字型態欄位
                                    case NPOI.SS.UserModel.CellType.Numeric:

                                        if (HSSFDateUtil.IsCellDateFormatted(cell)) //日期格式
                                        {
                                            //設定dataRow第j欄位的值，cell以日期格式取值
                                            dataRow[j] = DateTime.FromOADate(cell.NumericCellValue).ToString("yyyy/MM/dd HH:mm");
                                        }
                                        else //非日期格式
                                        {
                                            //設定dataRow第j欄位的值，cell以數字型態取值
                                            dataRow[j] = cell.NumericCellValue;
                                        }
                                        break;

                                    //布林值
                                    case NPOI.SS.UserModel.CellType.Boolean:

                                        //設定dataRow第j欄位的值，cell以布林型態取值
                                        dataRow[j] = cell.BooleanCellValue;
                                        break;

                                    //空值
                                    case NPOI.SS.UserModel.CellType.Blank:

                                        dataRow[j] = "";
                                        break;

                                    // 預設
                                    default:

                                        dataRow[j] = cell.StringCellValue;
                                        break;
                                }
                            }
                        }
                        //DataTable加入dataRow
                        dataTable.Rows.Add(dataRow);
                    }
                    catch (Exception ex)
                    {
                        //錯誤訊息
                        //throw new Exception("第 " + i + "列第" + column + "欄，資料格式有誤:\r\r" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            finally
            {
                //釋放資源
                sheet = null;
                wb = null;
                stream.Dispose();
                stream.Close();
            }
            //將資料存入資料庫
            DataTable gg = dataTable;
            switch (FileType)
            {
                case "Pro":
                    method_save_In_Product(dataTable);
                    break;
                case "ProDe":
                    method_save_In_ProDetail(dataTable);
                    break;
            }
        }

        public string method_save_In_ProDetail(DataTable data)
        {
            //dataTable跑回圈，insert資料至DB
            foreach (DataRow dr in data.Rows)
            {
                int _商品尺寸id = 0;
                int _商品顏色id = 0;
                //dr[2] 與 dr[3] 查詢相應table 回傳可存入的數值
                if (!string.IsNullOrEmpty(dr[2].ToString()))
                {
                    var temp = db.ProductsSizeDetails.FirstOrDefault(s => s.尺寸種類 == dr[2].ToString());
                    if (temp != null)
                    {
                        _商品尺寸id= temp.商品尺寸id;
                    }
                }

                if (!string.IsNullOrEmpty(dr[3].ToString()))
                {
                    var temp = db.ProductsColorDetails.FirstOrDefault(s => s.商品顏色種類 == dr[3].ToString());
                    if (temp != null)
                    {
                        _商品顏色id = temp.商品顏色id;
                    }
                }
                bool In_stock = false;
                bool put_on = false;
                //[10]商品是否有貨 [11]商品是否上架
                if (!string.IsNullOrEmpty(dr[10].ToString()))
                {
                    string _In_stock = dr[10].ToString();
                    if (_In_stock == "有貨")
                    {
                        In_stock = true;
                    }
                    else if (_In_stock == "沒貨")
                    {                     
                    In_stock = false ;
                    }
                }
                if (!string.IsNullOrEmpty(dr[11].ToString()))
                {
                    string _put_on =  dr[11].ToString();
                    if (_put_on == "上架")
                    {
                        put_on = true;
                    }
                    else if (_put_on == "下架")
                    {
                        put_on = false;
                    }
                }


                ProductDetail x = new ProductDetail();
                x.商品編號id = Int32.TryParse(dr[1].ToString(), out int _Id) ? _Id : 1;
                x.商品尺寸id = _商品尺寸id;
                x.商品顏色id = _商品顏色id;
                x.商品數量 = Int32.TryParse(dr[4].ToString(),out int _count)? _count : 0;           
                x.圖片位置id = Int32.TryParse(dr[9].ToString(), out int _Location) ? _Location : 1;            
                x.商品是否有貨 = In_stock;
                x.商品是否上架 = put_on;

                try
                {
                    db.ProductDetails.Add(x);
                   
                    //Response.BodyWriter("<script language=javascript>alert('檔案匯入成功');</" + "script>");
                    
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    return ex.Message;
                }
            }
            db.SaveChanges();
            return "";
        }

        public string method_save_In_Product(DataTable data)
        {
            //dataTable跑回圈，insert資料至DB
            foreach (DataRow dr in data.Rows)
            {
                int _分類id = 0;
                int _鞋種id = 0;
                //dr[9] 與 dr[10] 查詢相應table 回傳可存入的數值
                if (!string.IsNullOrEmpty(dr[9].ToString()))
                {
                    var temp = db.ProductsTypeDetails.FirstOrDefault(pd => pd.商品分類名稱 == dr[9].ToString());
                    if (temp != null)
                    {
                        _分類id = temp.商品分類id;
                    }
                }

                if (!string.IsNullOrEmpty(dr[10].ToString()))
                {
                    var temp = db.商品鞋種s.FirstOrDefault(s => s.鞋種 == dr[10].ToString());
                    if (temp != null)
                    {
                        _鞋種id = temp.商品鞋種id;
                    }
                }


                Product x = new Product();                
                x.上架時間 = dr[1].ToString();
                x.商品名稱 = dr[2].ToString();
                x.商品價格 = Convert.ToDecimal(double.TryParse(dr[3].ToString(), out double _price) ? _price : 0);
                x.商品介紹 = dr[4].ToString();
                x.商品材質 = dr[5].ToString();
                x.商品重量 = Int32.TryParse(dr[6].ToString(), out int _weight) ? _weight : 0;
                x.商品成本 = Convert.ToDecimal(double.TryParse(dr[7].ToString(), out double _cost) ? _cost : 0);
                x.商品分類id = _分類id;
                x.商品鞋種id = _鞋種id;
                x.商品是否有貨 = bool.TryParse(dr[8].ToString(), out bool _instock) ? _instock : false;
                x.商品是否上架 = bool.TryParse(dr[11].ToString(), out bool _onshelves) ? _onshelves : false;

                try
                {
                    db.Products.Add(x);
                    db.SaveChanges();
                    //Response.BodyWriter("<script language=javascript>alert('檔案匯入成功');</" + "script>");
                    return "檔案匯入成功";
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    return ex.Message;
                }
            }
            return "";
        }

        //下載
        public IActionResult Downloads(string filename,string key)
        {
            DataTable dt = null;
            //把 Session 到回ListData
            string json;
            switch (key)
            {
                case "Pro":
                    List<BACK_ProductViewModels> searchData = null;
                    if (HttpContext.Session.Keys.Contains(Product_CDictionary.SK_SEARCH_PRODUCTS_LIST)) //判斷Session 的key是否含有SK_PURCHASED_PRODUCTS_LIST
                    {
                        json = HttpContext.Session.GetString(Product_CDictionary.SK_SEARCH_PRODUCTS_LIST);   //將Session 轉為字串
                       searchData = JsonSerializer.Deserialize<List<BACK_ProductViewModels>>(json);
                    }
                    else
                         searchData = new List<BACK_ProductViewModels>();

                    dt = ConvertToDataTable(searchData.ToArray());
                    break;
                case "PD":
                    List<Back_ProducDetail> searchData01 = null;
                    if (HttpContext.Session.Keys.Contains(Product_CDictionary.SK_SEARCH_PRO_DETAIL_LIST)) //判斷Session 的key是否含有SK_SEARCH_PRO_DETAIL_LIST
                    {
                        json = HttpContext.Session.GetString(Product_CDictionary.SK_SEARCH_PRO_DETAIL_LIST);   //將Session 轉為字串
                        searchData01 = JsonSerializer.Deserialize<List<Back_ProducDetail>>(json);
                    }
                    else
                        searchData01 = new List<Back_ProducDetail>();

                    dt = ConvertToDataTable(searchData01.ToArray());
                    break;
            }

            if (dt.Rows.Count == 0)          
                return Json("沒有可輸出資料!!");
          
            //取得欄位名稱          
            string path = DataTableToExcelFile(dt);

            //string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
             string contentType = "application/vnd.ms-excel";
            //此寫法會強制將檔案另存，依不同的檔案型別傳回不同的ContentType
            //string contentType = "application/octet-stream";
            string fileName = $"{filename}.xls";
            //寫入檔案

            //FileStream fs = new FileStream(path, FileMode.Open);     
            var stream = System.IO.File.OpenRead(path);  //創建資料流
            return File(stream, contentType, fileName);  //資料流是否要關閉
        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        private string DataTableToExcelFile(DataTable dt)
        {
            //建立Excel 2003檔案
            IWorkbook wb = new HSSFWorkbook();
            ISheet ws;

            ////建立Excel 2007檔案
            //IWorkbook wb = new XSSFWorkbook();
            //ISheet ws;

            if (dt.TableName != string.Empty)
            {
                ws = wb.CreateSheet(dt.TableName);
            }
            else
            {
                ws = wb.CreateSheet("Sheet1");
            }

            ws.CreateRow(0);//第一行為欄位名稱
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ws.GetRow(0).CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ws.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ws.GetRow(i + 1).CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
                }
            }
            //string filename = DateTime.Now.ToString("yyyyMMddHHmmss");
            string filename = "TempUp";
            string path = _eviroment.WebRootPath + "/Datas/" + $"{filename}.xls";  //用環境變數取得 IIS路徑(wwwroot)
            System.IO.File.Delete(path);   //刪掉原本的檔案           
            FileStream file = new FileStream(path, FileMode.Create);//產生檔案
            wb.Write(file);
            file.Close();
            return path;
        }

        #endregion


        #region Product所有事件
        public IActionResult DeleteProduct(string? id)
        {
            int _id = 0;
            if (string.IsNullOrEmpty(id) || !(Int32.TryParse(id, out _id)))
                return Json("錯誤_傳輸id資料異常");

            IEnumerable<ProductDetail> data = db.ProductDetails.Where(pd => pd.商品編號id == _id).ToList();
            if (data.Count() != 0)
                return Json("警告_商品尚有明細,不能刪除");

            var Pro = db.Products.FirstOrDefault(p => p.商品編號id == _id);
            if (Pro == null)
                return Json("錯誤_沒有此項商品");
            string name = Pro.商品名稱;
            db.Products.Remove(Pro);
            db.SaveChanges();

            return Json($"商品{name}刪除成功");
        }


        public IActionResult EditProduct(string id, string time, string name, string price, string introduce, string material, string weight, string cost, string typeid, string shoeid, string instock, string onshelves)
        {
            //可以先做後端檢查(time、價格、成本不能亂填)


            //檢查後可以寫入Product Model內存入資料庫
            int _id = 0;

            if (string.IsNullOrEmpty(id) || !(Int32.TryParse(id, out _id)))
                return Json("失敗!商品id沒資料!!");

            try
            {
                var pro = db.Products.FirstOrDefault(p => p.商品編號id == _id);
                if (pro == null)
                    return Json("失敗!找不到資料");

                pro.上架時間 = time;
                pro.商品名稱 = name;
                pro.商品價格 = Convert.ToDecimal(double.TryParse(price, out double _price) ? _price : 0);
                pro.商品介紹 = introduce;
                pro.商品材質 = material;
                pro.商品重量 = Int32.TryParse(weight, out int _weight) ? _weight : 0;
                pro.商品成本 = Convert.ToDecimal(double.TryParse(cost, out double _cost) ? _cost : 0);
                pro.商品分類id = Int32.TryParse(typeid, out int _typeid) ? _typeid : 0;
                pro.商品鞋種id = Int32.TryParse(shoeid, out int _shoeid) ? _shoeid : 0;
                pro.商品是否有貨 = bool.TryParse(instock, out bool _instock) ? _instock : false;
                pro.商品是否上架 = bool.TryParse(onshelves, out bool _onshelves) ? _onshelves : false;
                db.SaveChanges();
                return Json("修改成功!");
            }
            catch
            {
                return Json("失敗!資料庫寫入異常");
            }
        }

        public IActionResult CreateProduct(string id, string time, string name, string price, string introduce, string material, string weight, string cost, string typeid, string shoeid, string instock, string onshelves)
        {
            //可以先做後端檢查(time、價格、成本不能亂填)
            //檢查後可以寫入Product Model內存入資料庫
            try
            {
                Product pro = new Product();
                pro.上架時間 = time;
                pro.商品名稱 = name;
                pro.商品價格 = Convert.ToDecimal(double.TryParse(price, out double _price) ? _price : 0);
                pro.商品介紹 = introduce;
                pro.商品材質 = material;
                pro.商品重量 = Int32.TryParse(weight, out int _weight) ? _weight : 0;
                pro.商品成本 = Convert.ToDecimal(double.TryParse(cost, out double _cost) ? _cost : 0);
                pro.商品分類id = Int32.TryParse(typeid, out int _typeid) ? _typeid : 0;
                pro.商品鞋種id = Int32.TryParse(shoeid, out int _shoeid) ? _shoeid : 0;
                pro.商品是否有貨 = bool.TryParse(instock, out bool _instock) ? _instock : false;
                pro.商品是否上架 = bool.TryParse(onshelves, out bool _onshelves) ? _onshelves : false;

                db.Products.Add(pro);
                db.SaveChanges();
                return Json("新增成功");
            }
            catch
            {
                return Json("失敗");
            }
        }

        public IActionResult Pro_Edit(string id) {
            if (!string.IsNullOrEmpty(id))
            {
                var pro = db.Products.Where(p => p.商品編號id == Int32.Parse(id)).FirstOrDefault();
                if (pro != null)
                {
                    return Json(pro);
                }
                return Json(null);
            }
            return Json(null);
        }

        [HttpGet]
        public IActionResult showlist(string id)
        {
            string key = id;
            IEnumerable<Product> tmepdata = null;

            //結合相關表單產生有意義的資料
            var data = from p in db.Products.ToList()
                       join pt in db.ProductsTypeDetails on p.商品分類id equals pt.商品分類id
                       join s in db.商品鞋種s on p.商品鞋種id equals s.商品鞋種id
                       select new BACK_ProductViewModels
                       {
                           商品編號id = p.商品編號id,
                           上架時間 = p.上架時間,
                           商品名稱 = p.商品名稱,
                           商品價格 = p.商品價格,
                           商品介紹 = p.商品介紹,
                           商品材質 = p.商品材質,
                           商品重量 = p.商品重量,
                           商品成本 = p.商品成本,
                           商品是否有貨 = p.商品是否有貨,
                           商品分類 = pt.商品分類名稱,
                           商品鞋種 = s.鞋種,
                           商品是否上架 = p.商品是否上架
                       };

            if (!string.IsNullOrEmpty(key))
            {
                data = data.Where(d => d.商品名稱.Contains(key) || d.商品鞋種.Contains(key) || d.商品介紹.Contains(key) || d.商品分類.Contains(key) || d.商品材質.Contains(key)).ToList();
            }
            //存入輸出暫存          
            //List<Product> searchData = new List<Product>();   //初始化
            //foreach (var item in data)
            //{
            //    foreach (var p in db.Products)
            //    {
            //        if (item.商品編號id == p.商品編號id)
            //        {
            //            searchData.Add(p);
            //        }
            //    }
            //}
            //將得到的值存入Session
            if (data.Count() != 0)
            {
                string json = JsonSerializer.Serialize(data);
                HttpContext.Session.SetString(Product_CDictionary.SK_SEARCH_PRODUCTS_LIST, json);
            }

            return Json(new { data });
        }

        #endregion

        #region   提取select 所需的資料  

        //提取圖片位置s的jpg圖檔名稱
        public IActionResult GetImg(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Json("錯誤_資料傳輸錯誤");
            int _id = 0;
            if (!Int32.TryParse(id, out _id))
                return Json("錯誤_數值有誤");
            var data = db.圖片位置s.FirstOrDefault(im => im.圖片位置id == _id);
            return Json(data);
        }

        //提取商品分類
        public IActionResult GetTyteD()
        {
            IEnumerable<ProductsTypeDetail> data = db.ProductsTypeDetails.ToList();
            return Json(data);
        }
        //提取鞋種
        public IActionResult GetShoe()
        {
            IEnumerable<商品鞋種> data = db.商品鞋種s.ToList();
            return Json(data);
        }

        //提取商品名稱
        public IActionResult GetProName()
        {
            IEnumerable<Back_GetProName> data = from p in db.Products.ToList()
                                                select new Back_GetProName {
                                                    商品編號id = p.商品編號id,
                                                    商品名稱 = p.商品名稱
                                                };
            return Json(data);
        }
        //提取圖片id
        //抓取圖片id建立select
        public IActionResult Get_img_select()
        {
            IEnumerable<圖片位置> data = db.圖片位置s.ToList();
            return Json(data);
        }

        //提取顏色
        public IActionResult GetColor()
        {
            IEnumerable<ProductsColorDetail> data = db.ProductsColorDetails.ToList();
            return Json(data);
        }
        //提取尺寸
        public IActionResult GetSize()
        {
            IEnumerable<ProductsSizeDetail> data = db.ProductsSizeDetails.ToList();
            return Json(data);
        }
        #endregion


        #region ProductDetail所有事件
        public class createkeydata
       {
           public string? id { get; set; }

           public string? imgid { get; set; }

            public string? colid { get; set; }
       }

        public IActionResult _CreateDetail(createkeydata key)
        {
            if (string.IsNullOrEmpty(key.id))
            {
                ViewBag.id = db.Products.FirstOrDefault().商品編號id;
            }
            else
            {
                ViewBag.id = key.id;
            }           
            if (string.IsNullOrEmpty(key.imgid))
            {
                ViewBag.imgid = "";
            }
            else
            {
                ViewBag.imgid = key.imgid;
            }
            if (string.IsNullOrEmpty(key.colid))
            {
                ViewBag.colid = "";
            }
            else
            {
                ViewBag.colid = key.colid;
            }
            
            return PartialView();
        }

        public IActionResult _EditDetial(string id)
        {
            //因用javascript ajax傳輸 請用 Content("錯誤_請輸入圖片!", "text/plain", Encoding.UTF8);
          
            if (string.IsNullOrEmpty(id))
                return Content("錯誤_資料傳輸錯誤", "text/plain", Encoding.UTF8);
            int _id = Int32.TryParse(id, out int temp)?   temp : -1;
            if(_id==-1)
                return Content("錯誤_資料格式錯誤", "text/plain", Encoding.UTF8);
            var data = db.ProductDetails.FirstOrDefault(pd => pd.Id == _id);
            return PartialView(data);
        }
        [HttpPost]
        public IActionResult _EditDetial(ProductDetail data, bool? 新建圖片, IFormFile photo1, IFormFile photo2, IFormFile photo3)
        {
            
            var FL = db.ProductDetails.FirstOrDefault(pd => pd.Id == data.Id);
            if(FL==null) return Content($"錯誤_沒有此筆資料", "text/plain", Encoding.UTF8);

            //先更改圖片位置
            //建立8位數亂碼
            string cold = (new Back_Product_library()).RandomString(8);
            string imgeName = $"{data.商品編號id}_{cold}";
            //判斷此類商品尺寸是否重複
            //判斷尺寸是否更改
            int? thisSizeid = db.ProductDetails.FirstOrDefault(pd => pd.Id == data.Id).商品尺寸id;
            if (data.商品尺寸id != thisSizeid)
            {
                int PDCount = db.ProductDetails.Where(pd => pd.商品編號id == Convert.ToInt32(data.商品編號id) &pd.商品顏色id == Convert.ToInt32(data.商品顏色id)& pd.商品尺寸id == Convert.ToInt32(data.商品尺寸id)).Count();
                if (PDCount > 0)
                    return Content("錯誤_此商品已有此尺寸!", "text/plain", Encoding.UTF8);
            }          

            var imgdata = db.圖片位置s.FirstOrDefault(im => im.圖片位置id ==  data.圖片位置id);
            if(imgdata==null)
                return Content("錯誤_圖片位置錯誤!", "text/plain", Encoding.UTF8);
            //先建立圖片資料
            if (新建圖片 == true)
            {
                if (photo1 == null || photo2 == null || photo3 == null)
                    return Content("錯誤_請輸入圖片!", "text/plain", Encoding.UTF8);
                //photo包成List<IFormFile>傳給建檔方法
                List<IFormFile> photo = new List<IFormFile>() { photo1, photo2, photo3 };
                Method_Create_Img(imgeName, photo);
                //新建商品得到圖片位置id
                var pd = db.圖片位置s.FirstOrDefault(p => p.商品圖片1.Contains(imgeName));
                FL.圖片位置id = pd.圖片位置id;
            }
            else
            {
                if (photo1 != null)
                {
                    SavePhotoMethod(photo1, $"{imgeName}_1", imgdata.商品圖片1);
                    imgdata.商品圖片1 = $"{imgeName}_1.jpg";
                }

                if (photo2 != null)
                {
                    SavePhotoMethod(photo2, $"{imgeName}_2", imgdata.商品圖片2);
                    imgdata.商品圖片2 = $"{imgeName}_2.jpg";
                }

                if (photo3 != null)
                {
                    SavePhotoMethod(photo3, $"{imgeName}_3", imgdata.商品圖片3);
                    imgdata.商品圖片3 = $"{imgeName}_3.jpg";
                }
                FL.圖片位置id = data.圖片位置id;
            }

            FL.Id = data.Id;
            FL.商品編號id = data.商品編號id;
            FL.商品尺寸id = data.商品尺寸id;
            FL.商品顏色id = data.商品顏色id;
            FL.商品數量 = data.商品數量;
            FL.商品編號 = data.商品編號;
           
            FL.商品是否有貨 = data.商品是否有貨;
            FL.商品是否上架 = data.商品是否上架;
            db.SaveChanges();
           
            return Content($"明細編號 {data.Id} 修改成功", "text/plain", Encoding.UTF8);
        }

        
        //新建圖片方法
        public void SavePhotoMethod(IFormFile p,string proname,string oldname)
        {
            string photoName = $"{proname}.jpg";
            string oldPath = _eviroment.WebRootPath + "/images/" + oldname;
            if (oldname != "")
            {//要先確認這Path是否有檔案    
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);   //刪掉圖片
                    //string photoName = $"{Guid.NewGuid().ToString()}.jpg";      
                }
            }
            string path01 = _eviroment.WebRootPath + "/images/" + photoName;  //用環境變數取得 IIS路徑(wwwroot)
            using (FileStream fs = new FileStream(path01, FileMode.Create))
            {
                p.CopyTo(fs);   //將圖片寫入指定路徑      
            }
        }

     
        //新增ProductDetail
        public IActionResult CreateProDetail(Back_ProducDetail Pro, IFormFile photo1, IFormFile photo2, IFormFile photo3)
        {
            if (Pro == null)
                return Content("錯誤_沒有資料!", "text/plain", Encoding.UTF8);
            //先建立圖片位置
            //建立8位數亂碼
            string cold = (new Back_Product_library()).RandomString(8);
            string imgeName = $"{Pro.商品編號id}_{cold}";
            //判斷此商品是否已有細項有重複的尺寸
            int PDCount = db.ProductDetails.Where(pd => pd.商品編號id == Convert.ToInt32(Pro.商品編號id) & pd.商品尺寸id == Convert.ToInt32(Pro.明細尺寸)).Count();
            if (PDCount > 0)
                return Content("錯誤_已有此尺寸!", "text/plain", Encoding.UTF8);
            //判斷是否新建圖片
            if (Pro.新建圖片==true)
            {               
                if (photo1 == null || photo2 == null || photo3 == null)
                    return Content("錯誤_請輸入圖片!", "text/plain", Encoding.UTF8);
                //photo包成List<IFormFile>傳給建檔方法
                List<IFormFile> photo = new List<IFormFile>() { photo1, photo2, photo3 };
                Method_Create_Img(imgeName, photo);
                //新建商品得到圖片位置id
                var pd = db.圖片位置s.FirstOrDefault(p => p.商品圖片1.Contains(imgeName));
                Pro.圖片位置id = pd.圖片位置id.ToString();
            }

            ProductDetail data = new ProductDetail();
            data.商品編號id = Convert.ToInt32(Pro.商品編號id) ;
            data.商品尺寸id = Convert.ToInt32(Pro.明細尺寸);
            data.商品顏色id = Convert.ToInt32(Pro.顏色);
            data.商品數量 = Convert.ToInt32(Pro.數量);
            data.商品編號 = Pro.商品編號id;
            data.圖片位置id = Convert.ToInt32(Pro.圖片位置id);
            data.商品是否上架 = bool.Parse(Pro.商品是否上架);
            data.商品是否有貨 = bool.Parse(Pro.商品是否有貨);
            db.ProductDetails.Add(data);
            db.SaveChanges();
            return Content("商品明細建檔成功!", "text/plain", Encoding.UTF8);
        }

        //新建圖片位置s的方法
        public int Method_Create_Img(string imgeName, List<IFormFile> photo)
        {
            int id = 0;
            //先建立圖片資料
            for (int i = 0; i < photo.Count(); i++)
            {
                SavePhotoMethod(photo[i], $"{imgeName}_{i+1}", "");
            }
            圖片位置 img = new 圖片位置();
            img.商品圖片1 = $"{imgeName}_1.jpg";
            img.商品圖片2 = $"{imgeName}_2.jpg";
            img.商品圖片3 = $"{imgeName}_3.jpg";
            db.圖片位置s.Add(img);
            db.SaveChanges();
            return id; 
        }

        //刪除圖片位置資料表的相應圖片
        public string DeleteImg(string imgName)
        {
            if (imgName == null)
                return "錯誤_沒有此圖片資料!";
                string oldPath1 = _eviroment.WebRootPath + "/images/" + imgName;
                System.IO.File.Delete(oldPath1); //刪掉原本的圖片

            return $"{imgName} 圖片刪除成功";

        }

        //刪除ProductDetail
        public IActionResult DeleteProDetail(string id)
        {
            if(string.IsNullOrEmpty(id))
                return Json( "錯誤_傳輸id資料異常");

            string[] idgrop = id.Split(',');
            string errmessage = "";
            for (int i = 0; i < idgrop.Length; i++)
            {
                errmessage += $"{method_DeleteProDetail(idgrop[i])}";
            }
            if (errmessage.Length != 0)
                return Json($"{errmessage}");
            return Json($"商品明細刪除成功");
        }

        public string method_DeleteProDetail(string id) {
            int _id = 0;
            //判斷同時用TryPase回傳已轉為int的id給_id
            if (string.IsNullOrEmpty(id) || !(Int32.TryParse(id, out _id)))
                return $"編號{id}錯誤_傳輸id資料異常 ";
            //查詢是否有訂單在使用此細項商品
            int odCount = db.OrderDetails.ToList().Where(od => od.Id == _id).Count();
            if (odCount > 0)
                return $"編號{id}錯誤_有訂單明細在使用此商品細項 ";
            //查詢圖檔位置     
            var ProDdata = db.ProductDetails.ToList().FirstOrDefault(pd => pd.Id == _id);
            if (ProDdata == null)
                return $"編號{id}錯誤_沒有此項商品 ";
            var imgData = db.圖片位置s.FirstOrDefault(i => i.圖片位置id == ProDdata.圖片位置id);

            int ProCount = db.ProductDetails.Where(g => g.圖片位置id == ProDdata.圖片位置id).Count();
            //判斷使用此圖片位置的ProductDetail是否只剩一筆
            if (ProCount == 1)
            {
                //刪除images內的相關圖片
                if (imgData == null)
                    return "錯誤_沒有此筆圖片位置資料 ";
                //刪除相應圖片
                DeleteImg(imgData.商品圖片1);
                DeleteImg(imgData.商品圖片2);
                DeleteImg(imgData.商品圖片3);

                //刪除圖片位置s內此筆資料
                db.圖片位置s.Remove(imgData);
            }
            db.ProductDetails.Remove(ProDdata);
            db.SaveChanges();
            return "";

        }


        //顯示ProductDetail
        public class Back_Pro_Detail_data_ColorImg
        {
           public string? name { get; set; }
           public string? img1 { get; set; }
          public  string? img2 { get; set; }
         public  string? img3 { get; set; }
         public string? Proid { get; set; }
          public string? colid { get; set; }
         public string? imgid { get; set; }
          public List<Back_ProducDetail>? thisdata { get; set; }
        }

        //輸入key 取得 Product的商品編號id 與 ProductDetail的Id
        public class Search_id
        {
            public int 商品編號id { get; set; }
            public int id { get; set; }
        }
        [HttpGet]
        public IActionResult showDetail(string id) 
        {
            if (!string.IsNullOrEmpty(id))
            {
                int PDid = Convert.ToInt32(id);
                var _data = from pd in db.ProductDetails.ToList()
                            join z in db.ProductsSizeDetails on pd.商品尺寸id equals z.商品尺寸id
                            join c in db.ProductsColorDetails on pd.商品顏色id equals c.商品顏色id
                            join i in db.圖片位置s on pd.圖片位置id equals i.圖片位置id
                            where pd.商品編號id == PDid
                            select new Back_ProducDetail
                           {                               
                               明細編號 = pd.Id.ToString(),
                                商品編號id = pd.商品編號id.ToString(),
                               明細尺寸 = z.尺寸種類,
                               顏色 = c.商品顏色種類,
                               數量 = (pd.商品數量).ToString(),
                               圖片位置id = i.圖片位置id.ToString(),
                               商品圖片1 = i.商品圖片1,
                               商品圖片2 = i.商品圖片2,
                               商品圖片3 = i.商品圖片3,
                               商品是否上架 = pd.商品是否上架.ToString(),
                               商品是否有貨 = pd.商品是否有貨.ToString()
                           };
                //將data資料做group by
                var data = from d in _data.ToList()
                              group d by new { d.顏色 , d.商品編號id } into g
                              select new Back_Pro_Detail_data_ColorImg
                              {
                                  Proid = g.Key.商品編號id,
                                  name = g.Key.顏色,
                                  colid =  (from p1 in db.ProductsColorDetails where p1.商品顏色種類 == g.Key.顏色 select p1).FirstOrDefault().商品顏色id.ToString(),
                                  imgid = (from p1 in _data where p1.顏色 == g.Key.顏色 select p1).FirstOrDefault().圖片位置id,
                                  img1 = (from p1 in _data where p1.顏色 == g.Key.顏色 select p1).FirstOrDefault().商品圖片1,
                                  img2 = (from p1 in _data where p1.顏色 == g.Key.顏色 select p1).FirstOrDefault().商品圖片2,
                                  img3 = (from p1 in _data where p1.顏色 == g.Key.顏色 select p1).FirstOrDefault().商品圖片3,
                                  thisdata = (from p1 in _data where p1.顏色 == g.Key.顏色 select p1).ToList()
                              };
                
                return Json(data);
            }
            return View();
        }



        #endregion ProductDetail所有事件

        #region 商品細項搜尋設定
        public IActionResult showSearchDetail(string id)
        {
            string key = id;
            var data = from pd in db.ProductDetails.ToList()
                       join p in db.Products on pd.商品編號id equals p.商品編號id
                       join s in db.商品鞋種s on p.商品鞋種id equals s.商品鞋種id
                       join t in db.ProductsTypeDetails on p.商品分類id equals t.商品分類id
                       join z in db.ProductsSizeDetails on pd.商品尺寸id equals z.商品尺寸id
                       join c in db.ProductsColorDetails on pd.商品顏色id equals c.商品顏色id
                       join i in db.圖片位置s on pd.圖片位置id equals i.圖片位置id
                       select new Back_ProducDetail
                       {
                           id = pd.Id.ToString(),
                           明細編號 = pd.Id.ToString(),
                           商品編號id = pd.商品編號id.ToString(),
                           分類 = t.商品分類名稱.ToString(),
                           鞋種 = s.鞋種.ToString(),
                           商品名稱 = p.商品名稱,
                           明細尺寸 = z.尺寸種類,
                           顏色 = c.商品顏色種類,
                           數量 = (pd.商品數量).ToString(),
                           圖片位置id = i.圖片位置id.ToString(),
                           商品圖片1 = i.商品圖片1,
                           商品圖片2 = i.商品圖片2,
                           商品圖片3 = i.商品圖片3,
                           商品是否上架 = pd.商品是否上架 == true ? "上架" : "下架",
                           商品是否有貨 = pd.商品是否有貨 == true ? "有貨" : "沒貨"
                       };
            if (!string.IsNullOrEmpty(key))
            {
                data = data.Where(d => d.商品名稱.Contains(key)|| d.分類.Contains(key) || d.鞋種.Contains(key) || d.明細尺寸 == key || d.顏色.Contains(key) || d.商品是否上架.Contains(key) || d.商品是否有貨.Contains(key)).ToList();
            }
            if (data.Count() != 0)
            {
                string json = JsonSerializer.Serialize(data);
                HttpContext.Session.SetString(Product_CDictionary.SK_SEARCH_PRO_DETAIL_LIST, json);
            }
            return Json(new { data });

        }


        #endregion 商品細項搜尋設定
    }
}
