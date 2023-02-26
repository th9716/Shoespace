using dbCompanyTest.Models;
using dbCompanyTest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using X.PagedList;
using static dbCompanyTest.Controllers.ProductController;
using static dbCompanyTest.ViewModels.ProductDetailViewModels;
using Microsoft.AspNetCore.SignalR;
using iText.StyledXmlParser.Jsoup.Nodes;
using NPOI.OpenXmlFormats.Spreadsheet;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using NPOI.SS.Formula.Atp;
using NuGet.Protocol;

namespace dbCompanyTest.Controllers
{
    public class ProductWallController : Controller
    {
        dbCompanyTestContext _context = new dbCompanyTestContext();
        public IActionResult Index(int? id, int page = 1)
        {

            if (id == null)
                return NotFound();
            else
            {
                var datas = from c in _context.Products
                            join d in _context.ProductDetails on c.商品編號id equals d.商品編號id
                            join e in _context.ProductsTypeDetails on c.商品分類id equals e.商品分類id
                            join f in _context.圖片位置s on d.圖片位置id equals f.圖片位置id
                            join b in _context.商品鞋種s on c.商品鞋種id equals b.商品鞋種id
                            join g in _context.ProductsColorDetails on d.商品顏色id equals g.商品顏色id
                            join h in _context.ProductsSizeDetails on d.商品尺寸id equals h.商品尺寸id
                            where c.商品分類id == id
                            select new ViewModels.ProductWallViewModel
                            {
                                鞋種名稱 = b.鞋種,
                                商品id = (int)d.商品編號id,
                                商品分類id = (int)id,
                                商品鞋種id = (int)c.商品鞋種id,
                                商品名稱 = c.商品名稱,
                                商品價格 = (decimal)c.商品價格,
                                產品圖片1 = f.商品圖片1,
                                產品圖片2 = f.商品圖片2,
                                商品分類名稱 = e.商品分類名稱,
                                商品顏色id = (int)d.商品顏色id,
                                顏色名稱 = g.商品顏色種類,
                                尺寸名稱 = h.尺寸種類,
                                材質名稱 = c.商品材質
                            };

                List<ProductWallViewModel> list = datas.ToList();
                List<ProductWallViewModel> newlist = new List<ProductWallViewModel>();
                foreach (var item in list)
                    if (newlist.Count<ProductWallViewModel>(z => z.商品名稱 == item.商品名稱 && z.顏色名稱 == item.顏色名稱) == 0)
                        newlist.Add(item);
                return View(newlist.ToPagedList(page, 8));
            }
        }
        [HttpPost]
        public IActionResult pro(string word, int page = 1)
        {

            var datas = from c in _context.Products
                        join d in _context.ProductDetails on c.商品編號id equals d.商品編號id
                        join e in _context.ProductsTypeDetails on c.商品分類id equals e.商品分類id
                        join f in _context.圖片位置s on d.圖片位置id equals f.圖片位置id
                        join b in _context.商品鞋種s on c.商品鞋種id equals b.商品鞋種id
                        join g in _context.ProductsColorDetails on d.商品顏色id equals g.商品顏色id
                        join h in _context.ProductsSizeDetails on d.商品尺寸id equals h.商品尺寸id
                        where h.尺寸種類 == word || g.商品顏色種類 == word || c.商品材質 == word
                        select new ViewModels.ProductWallViewModel
                        {
                            鞋種名稱 = b.鞋種,
                            商品id = (int)d.商品編號id,
                            商品鞋種id = (int)c.商品鞋種id,
                            商品名稱 = c.商品名稱,
                            商品價格 = (decimal)c.商品價格,
                            產品圖片1 = f.商品圖片1,
                            產品圖片2 = f.商品圖片2,
                            商品分類名稱 = e.商品分類名稱,
                            商品顏色id = (int)d.商品顏色id,
                            顏色名稱 = g.商品顏色種類,
                            尺寸名稱 = h.尺寸種類,
                            材質名稱 = c.商品材質
                        };

            return PartialView(datas.ToPagedList(page, 8));

        }
        public IActionResult typeNav(int? id, string? type)
        {
            var datas = from c in _context.商品鞋種s
                        join d in _context.Products on c.商品鞋種id equals d.商品鞋種id
                        join e in _context.ProductsTypeDetails on d.商品分類id equals e.商品分類id
                        where e.商品分類id == id
                        select new ViewModels.ProductWallViewModel
                        {
                            鞋種名稱 = c.鞋種,
                            商品鞋種id = c.商品鞋種id,
                            商品分類id = (int)id,
                            商品分類名稱 = type,
                        };

            List<ProductWallViewModel> list = datas.ToList();
            List<ProductWallViewModel> newlist = new List<ProductWallViewModel>();
            foreach (var item in list)
                if (newlist.Count<ProductWallViewModel>(z => z.鞋種名稱 ==item.鞋種名稱) == 0)
                    newlist.Add(item);
            return PartialView(newlist);
        }

        public IActionResult type(int? id, int? tid, string? type, int page = 1)
        {
            if (id == null)
                return NotFound();
            else
            {
                var datas = from c in _context.Products
                            join d in _context.ProductDetails on c.商品編號id equals d.商品編號id
                            join e in _context.ProductsTypeDetails on c.商品分類id equals e.商品分類id
                            join f in _context.圖片位置s on d.圖片位置id equals f.圖片位置id
                            join b in _context.商品鞋種s on c.商品鞋種id equals b.商品鞋種id
                            join g in _context.ProductsColorDetails on d.商品顏色id equals g.商品顏色id
                            join h in _context.ProductsSizeDetails on d.商品尺寸id equals h.商品尺寸id
                            where c.商品鞋種id == id && c.商品分類id==tid
                            select new ViewModels.ProductWallViewModel
                            {
                                鞋種名稱 = b.鞋種,
                                商品id = (int)d.商品編號id,
                                商品分類id = (int)tid,
                                商品鞋種id = (int)c.商品鞋種id,
                                商品名稱 = c.商品名稱,
                                商品價格 = (decimal)c.商品價格,
                                產品圖片1 = f.商品圖片1,
                                產品圖片2 = f.商品圖片2,
                                商品分類名稱 = type,
                                商品顏色id = (int)d.商品顏色id,
                                顏色名稱 = g.商品顏色種類,
                                尺寸名稱 = h.尺寸種類,
                                材質名稱 = c.商品材質
                            };
                ViewBag.tid = tid;
                ViewBag.type = type;

                List<ProductWallViewModel> list = datas.ToList();
                List<ProductWallViewModel> newlist = new List<ProductWallViewModel>();
                foreach (var item in list)
                    if (newlist.Count<ProductWallViewModel>(z => z.商品名稱 == item.商品名稱 && z.顏色名稱 == item.顏色名稱) == 0)
                        newlist.Add(item);
                return View(newlist.ToPagedList(page, 8));

            }
        }

        public IActionResult search(string? keyword, int page = 1)
        {

            var datas = from c in _context.Products
                        join d in _context.ProductDetails on c.商品編號id equals d.商品編號id
                        join e in _context.ProductsTypeDetails on c.商品分類id equals e.商品分類id
                        join f in _context.圖片位置s on d.圖片位置id equals f.圖片位置id
                        join b in _context.商品鞋種s on c.商品鞋種id equals b.商品鞋種id
                        join g in _context.ProductsColorDetails on d.商品顏色id equals g.商品顏色id
                        where c.商品名稱.Contains(keyword)
                        select new ViewModels.ProductWallViewModel
                        {
                            商品id = (int)d.商品編號id,
                            商品鞋種id = (int)c.商品鞋種id,
                            商品名稱 = c.商品名稱,
                            商品價格 = (decimal)c.商品價格,
                            產品圖片1 = f.商品圖片1,
                            產品圖片2 = f.商品圖片2,
                            keyword = keyword,
                            商品顏色id = (int)d.商品顏色id,
                            顏色名稱 = g.商品顏色種類
                        };
            ViewBag.keyword = keyword;
            List<ProductWallViewModel> list = datas.ToList();
            List<ProductWallViewModel> newlist = new List<ProductWallViewModel>();
            foreach (var item in list)
                if (newlist.Count<ProductWallViewModel>(z => z.商品名稱 == item.商品名稱 && z.顏色名稱 == item.顏色名稱) == 0)
                    newlist.Add(item);
            return View(newlist.ToPagedList(page, 8));


        }

        public IActionResult selectview()
        {
            ProductWallViewModel pwv = new ProductWallViewModel();
            var datas = (from c in _context.Products
                         join d in _context.ProductDetails on c.商品編號id equals d.商品編號id
                         join g in _context.ProductsColorDetails on d.商品顏色id equals g.商品顏色id
                         join h in _context.ProductsSizeDetails on d.商品尺寸id equals h.商品尺寸id
                         select new ViewModels.ProductWallViewModel
                         {
                             顏色名稱 = g.商品顏色種類,
                             尺寸名稱 = h.尺寸種類,
                             材質名稱 = c.商品材質
                         }).Distinct();

            foreach (var item in datas.Distinct())
            {
                pwv.顏色名稱 = item.顏色名稱;
                pwv.尺寸名稱 = item.尺寸名稱;
                pwv.材質名稱 = item.材質名稱;
            }

            return PartialView(datas);
        }



        //---------------------- Gary產品頁 ----------------------------

        //id = 商品編號ID，colorID=商品顏色ID
        public IActionResult Details(int? id, int? colorID)
        {
            //測試用 productDetail ID
            if (id == null)
            {
                id = 1;
                colorID = 1;
            }
            //viewModels
            ProductDetailViewModels pdm = new ProductDetailViewModels();

            //透過productID找出全部的顏色圖片，商品尺寸，ProductDetail，商品顏色ID
            #region
            pdm.pro商品顏色圖片list = new List<string>();
            pdm.pro商品尺寸list = new List<string>();
            pdm.pro商品尺寸idlist = new List<int>();
            pdm.pro商品顏色idlist = new List<int>();
            pdm.pro商品分類list = new List<productrandom>();
            //ParetComment
            pdm.paretCommentslist = new List<paretCommentclass>();
            ////ChildComment
            pdm.childCommentlist = new List<childCommentclass>();

            int Key = 0;
            if (id == null)
                return NotFound();
            else
            {
                //productDetailID所找出的單一商品詳細資訊
                var productdetail = from product in _context.Products
                                    join prodetail in _context.ProductDetails on product.商品編號id equals prodetail.商品編號id
                                    join pro分類 in _context.ProductsTypeDetails on product.商品分類id equals pro分類.商品分類id
                                    join prophoto in _context.圖片位置s on prodetail.圖片位置id equals prophoto.圖片位置id
                                    join procolor in _context.ProductsColorDetails on prodetail.商品顏色id equals procolor.商品顏色id
                                    where product.商品編號id == id && prodetail.商品顏色id == colorID
                                    select new
                                    {
                                        pro商品編號 = id,
                                        pro商品商品顏色ID = colorID,
                                        pro商品金額 = product.商品價格,
                                        pro商品顏色 = procolor.商品顏色種類,
                                        pro商品分類 = pro分類.商品分類名稱,
                                        pro商品分類id = product.商品分類id,
                                        pro商品名稱 = product.商品名稱,
                                        pro商品介紹 = product.商品介紹,
                                        pro商品材質 = product.商品材質,
                                        pro商品圖片1 = prophoto.商品圖片1,
                                        pro商品圖片2 = prophoto.商品圖片2,
                                        pro商品圖片3 = prophoto.商品圖片3,

                                    };
                foreach (var item in productdetail.Distinct())
                {
                    Key = (int)item.pro商品編號;
                    pdm.pro商品編號 = (int)item.pro商品編號;
                    pdm.商品顏色ID = (int)item.pro商品商品顏色ID;
                    pdm.pro商品顏色 = item.pro商品顏色;
                    pdm.pro商品金額 = item.pro商品金額.ToString();
                    pdm.pro商品分類 = item.pro商品分類;
                    pdm.pro商品分類id = item.pro商品分類id;
                    pdm.pro商品名稱 = item.pro商品名稱;
                    pdm.pro商品介紹 = item.pro商品介紹;
                    pdm.pro商品材質 = item.pro商品材質;
                    pdm.pro商品圖片1 = item.pro商品圖片1;
                    pdm.pro商品圖片2 = item.pro商品圖片2;
                    pdm.pro商品圖片3 = item.pro商品圖片3;
                }
                #endregion
                //取出商品的顏色及顏色圖片
                #region
                var totallist = from item in _context.Products
                                join prodetail in _context.ProductDetails on item.商品編號id equals prodetail.商品編號id
                                join procolor in _context.ProductsColorDetails on prodetail.商品顏色id equals procolor.商品顏色id
                                where item.商品編號id == Key
                                select new
                                {
                                    pro商品顏色圖片list = procolor.商品顏色圖片,
                                    pro商品顏色idlist = prodetail.商品顏色id,
                                };
                foreach (var CC in totallist.Distinct())
                {

                    pdm.pro商品顏色圖片list.Add(CC.pro商品顏色圖片list);
                    pdm.pro商品顏色idlist.Add((int)(CC.pro商品顏色idlist));
                    pdm.pro商品顏色圖片list = pdm.pro商品顏色圖片list.Distinct().ToList();
                }
                //pdm.pro商品顏色idlist = pdm.pro商品顏色idlist.Distinct().ToList();

                //取出此商品顏色有幾種size
                //Comment撈取資料

                var listsize = from item in _context.Products
                               join prodetail in _context.ProductDetails on item.商品編號id equals prodetail.商品編號id
                               join prosize in _context.ProductsSizeDetails on prodetail.商品尺寸id equals prosize.商品尺寸id
                               join procolor in _context.ProductsColorDetails on prodetail.商品顏色id equals procolor.商品顏色id
                               //join childselfcomment in _context.ChildComments on childcomment.訊息id equals childselfcomment.子訊息id
                               where item.商品編號id == Key && procolor.商品顏色id == pdm.商品顏色ID
                               orderby prosize.尺寸種類
                               select new
                               {
                                   pro商品尺寸list = prosize.尺寸種類,
                                   pro商品尺寸idlist = prosize.商品尺寸id,
                               };
                foreach (var SS in listsize.Distinct())
                {

                    pdm.pro商品尺寸list.Add(SS.pro商品尺寸list);
                    pdm.pro商品尺寸list = pdm.pro商品尺寸list.Distinct().ToList();
                    pdm.pro商品尺寸idlist.Add(SS.pro商品尺寸idlist);
                }
                //pdm.pro商品尺寸idlist = pdm.pro商品尺寸idlist.Distinct().ToList();
                #endregion
                var ParetCommentList = from item in _context.ParentComments
                                       where item.商品編號id == Key && item.商品顏色id == pdm.商品顏色ID
                                       select new
                                       {
                                           paretCommentID = item.訊息id,
                                           paretCommentDateList = (DateTime)item.建立日期,
                                           paretCommentGuestIDList = item.客戶編號,
                                           paretCommentGuestNameList = item.客戶姓名,
                                           paretCommentList = item.內容,
                                       };
                foreach (var comment in ParetCommentList.Distinct())
                {
                    paretCommentclass partlist = new paretCommentclass();
                    //加入partlist
                    partlist.paretCommentID = comment.paretCommentID;
                    partlist.paretCommentDate = comment.paretCommentDateList;
                    partlist.paretComment = comment.paretCommentList;
                    partlist.paretCommentGuestID = comment.paretCommentGuestIDList;
                    partlist.paretCommentGuestName = comment.paretCommentGuestNameList;
                    pdm.paretCommentslist.Add(partlist);
                }
                //pdm.paretCommentslist = pdm.paretCommentslist.Distinct().ToList();
                var ChildCommentList = from item in _context.ParentComments
                                       join childcomment in _context.ChildComments on item.訊息id equals childcomment.父訊息id
                                       where item.商品編號id == Key && item.商品顏色id == pdm.商品顏色ID
                                       select new
                                       {
                                           childCommentID = childcomment.訊息id,
                                           childCommentDateList = (DateTime)childcomment.建立日期,
                                           childCommentList = childcomment.內容,
                                           childCommentGuestIDList = childcomment.客戶編號,
                                           childCommentGuestNameList = childcomment.客戶姓名,
                                           childCommentParet = childcomment.父訊息id,
                                           childCommentchildid = childcomment.子訊息id
                                       };
                foreach (var comment in ChildCommentList.Distinct())
                {
                    childCommentclass childlist = new childCommentclass();
                    //加入childlist
                    childlist.childCommentID = comment.childCommentID;
                    childlist.childComment = comment.childCommentList;
                    childlist.childCommentDate = comment.childCommentDateList;
                    childlist.childCommentGuestName = comment.childCommentGuestNameList;
                    childlist.childCommentGuestID = comment.childCommentGuestIDList;
                    //父訊息ID
                    childlist.childCommentParet = comment.childCommentParet;
                    //子訊息ID
                    childlist.childCommentchildid = comment.childCommentchildid;
                    pdm.childCommentlist.Add(childlist);
                }

                //隨機取出商品
                Random r = new Random();
                var redomton = (from item in _context.Products
                               join prodetail in _context.ProductDetails on item.商品編號id equals prodetail.商品編號id
                               join propictrue in _context.圖片位置s on prodetail.圖片位置id equals propictrue.圖片位置id
                               where item.商品分類id == pdm.pro商品分類id
                               group item by new {item.商品編號id,prodetail.商品顏色id,propictrue.商品圖片1} into g 
                               orderby Guid.NewGuid()
                               select new
                               {
                                   商品編號id = g.Key.商品編號id,
                                   商品顏色id = g.Key.商品顏色id,
                                   商品圖片1 = g.Key.商品圖片1,
                               }).ToList();

                foreach (var item in redomton.Take(4))
                {
                    productrandom prm = new productrandom();
                    prm.pro商品編號 = item.商品編號id;
                    prm.商品顏色ID = (int)item.商品顏色id;
                    prm.商品圖片1 = item.商品圖片1;
                    pdm.pro商品分類list.Add(prm);
                };

                if (HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION))
                {
                    string json = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION);
                    var data = JsonSerializer.Deserialize<TestClient>(json);
                    pdm.客戶編號 = data.客戶編號;
                }
                else if (HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
                {
                    string json = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
                    pdm.員工編號 = json;
                }
                return View(pdm);
            }

        }
        public IActionResult checkuser()
        {
            try
            {
                if (HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION))
                {
                    string json = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION);
                    return Content(json);

                }
                else if (HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
                {
                    string json = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
                    json = $"{{\"name\":\"{json}\"}}";//將string 組成 Json字串
                    return Json(null);
                }
                else
                {
                    return Json(null);
                }
            }
            catch {
                return Content("發送錯誤");
            }

        }
        public IActionResult CreateComment(IFormCollection data)
        {
            try
            {
                TestClient TC = JsonSerializer.Deserialize<TestClient>(data["userdata"]);
                var user = _context.TestClients.FirstOrDefault(x => x.客戶編號 == TC.客戶編號);
                if (data["comment"] == "")
                {
                    return Content("請勿輸入空白");
                }
                else
                {
                    if (user != null)
                    {
                        if (Convert.ToInt32(data["count"]) == 1)
                        {

                            ParentComment PC = new ParentComment();
                            PC.內容 = data["comment"];
                            PC.商品顏色id = Convert.ToInt32(data["colorid"]);
                            PC.商品編號id = Convert.ToInt32(data["productid"]);
                            PC.建立日期 = DateTime.Now;
                            PC.客戶編號 = user.客戶編號;
                            PC.客戶姓名 = user.客戶姓名;
                            _context.ParentComments.Add(PC);
                            _context.SaveChanges();
                            ProductDetailViewModels pdm = selectData(data);
                            return Json(pdm);
                        }
                        else if (Convert.ToInt32(data["count"]) == 2)
                        {
                            ChildComment CC = new ChildComment();
                            CC.內容 = data["comment"];
                            CC.客戶姓名 = user.客戶姓名;
                            CC.客戶編號 = user.客戶編號;
                            CC.建立日期 = DateTime.Now;
                            CC.子訊息id = null;
                            CC.父訊息id = Convert.ToInt32(data["paretID"]);
                            _context.ChildComments.Add(CC);
                            _context.SaveChanges();
                            ProductDetailViewModels pdm = selectData(data);
                            if (data["paretID"] != "undefined" && data["paretID"] != "")
                        {
                                pdm.collapseParetid = Convert.ToInt32(data["paretID"]);
                            }
                            return Json(pdm);

                        }
                        else
                        {
                            ChildComment CC = new ChildComment();
                            CC.內容 = data["comment"];
                            CC.客戶姓名 = user.客戶姓名;
                            CC.客戶編號 = user.客戶編號;
                            CC.建立日期 = DateTime.Now;
                            CC.父訊息id = Convert.ToInt32(data["paretID"]);
                            CC.子訊息id = Convert.ToInt32(data["childID"]);
                            _context.ChildComments.Add(CC);
                            _context.SaveChanges();
                            ProductDetailViewModels pdm = selectData(data);
                            if (data["paretID"] != "undefined" && data["paretID"] != "")
                            {
                                pdm.collapseParetid = Convert.ToInt32(data["paretID"]);
                            }
                            return Json(pdm);
                        }
                    }
                    else
                    {
                        return Content("請登入");
                    }
                }
            }
            catch{

                return Content("新增錯誤");
            }

        }
        private ProductDetailViewModels selectData(IFormCollection data)
        {
            ProductDetailViewModels pdm = new ProductDetailViewModels();
            pdm.paretCommentslist = new List<paretCommentclass>();
            pdm.childCommentlist = new List<childCommentclass>();

            var ParetCommentList = from item in _context.ParentComments
                                   where item.商品編號id == Convert.ToInt32(data["productid"]) && item.商品顏色id == Convert.ToInt32(data["colorid"])
                                   select new
                                   {
                                       paretCommentID = item.訊息id,
                                       paretCommentDateList = (DateTime)item.建立日期,
                                       paretCommentGuestIDList = item.客戶編號,
                                       paretCommentGuestNameList = item.客戶姓名,
                                       paretCommentList = item.內容
                                   };
            foreach (var item in ParetCommentList)
            {
                paretCommentclass partlist = new paretCommentclass();
                //加入partlist
                partlist.paretCommentID = item.paretCommentID;
                partlist.paretCommentDate = item.paretCommentDateList;
                partlist.paretComment = item.paretCommentList;
                partlist.paretCommentGuestID = item.paretCommentGuestIDList;
                partlist.paretCommentGuestName = item.paretCommentGuestNameList;
                pdm.paretCommentslist.Add(partlist);
            }
            pdm.paretCommentslist = pdm.paretCommentslist.Distinct().ToList();

            var ChildCommentList = from item in _context.ParentComments
                                   join childcomment in _context.ChildComments on item.訊息id equals childcomment.父訊息id
                                   where item.商品編號id == Convert.ToInt32(data["productid"]) && item.商品顏色id == Convert.ToInt32(data["colorid"])
                                   select new
                                   {
                                       childCommentID = childcomment.訊息id,
                                       childCommentDateList = (DateTime)childcomment.建立日期,
                                       childCommentList = childcomment.內容,
                                       childCommentGuestIDList = childcomment.客戶編號,
                                       childCommentGuestNameList = childcomment.客戶姓名,
                                       childCommentParet = childcomment.父訊息id,
                                       childCommentchildid = childcomment.子訊息id
                                   };
            foreach (var item in ChildCommentList)
            {
                childCommentclass childlist = new childCommentclass();
                //加入childlist
                childlist.childCommentID = item.childCommentID;
                childlist.childComment = item.childCommentList;
                childlist.childCommentDate = item.childCommentDateList;
                childlist.childCommentGuestName = item.childCommentGuestNameList;
                childlist.childCommentGuestID = item.childCommentGuestIDList;
                childlist.childCommentParet = item.childCommentParet;
                childlist.childCommentchildid = item.childCommentchildid;
                pdm.childCommentlist.Add(childlist);
            }
            pdm.childCommentlist = pdm.childCommentlist.Distinct().ToList();

            pdm.商品顏色ID = Convert.ToInt32(data["colorid"]);
            pdm.pro商品編號 = Convert.ToInt32(data["productid"]);
            if (HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION))
            {
                string json = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION);
                var userdata = JsonSerializer.Deserialize<TestClient>(json);
                pdm.客戶編號 = userdata.客戶編號;
            }
            else if (HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
            {
                string json = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
                pdm.員工編號 = json;
            }
            return pdm;
        }
        [HttpPost]
        public IActionResult resetdisplay(IFormCollection datas)
        {
            ProductDetailViewModels data = JsonSerializer.Deserialize<ProductDetailViewModels>(datas["response"]);
            if (HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION))
            {
                string json = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION);
                TestClient tc = JsonSerializer.Deserialize<TestClient>(json);
                data.客戶編號 = tc.客戶編號;
            }
            return PartialView(data);
        }
        public IActionResult DeleteComment(IFormCollection data)
        {
            try
            {
                if (Convert.ToInt32(data["order"]) == 1)
                {
                    var parentdata = _context.ParentComments.FirstOrDefault(x => x.訊息id == Convert.ToInt32(data["id"]));
                    var parentdatachild = _context.ChildComments.Select(x => x).Where(x => x.父訊息id == parentdata.訊息id);
                    if (parentdatachild != null)
                    {
                        _context.ChildComments.RemoveRange(parentdatachild);
                    }
                    _context.ParentComments.Remove(parentdata);
                    _context.SaveChanges();
                    ProductDetailViewModels pdm = selectData(data);

                    return Json(pdm);
                }
                else if (Convert.ToInt32(data["order"]) == 2)
                {
                    var childdata = _context.ChildComments.FirstOrDefault(x => x.訊息id == Convert.ToInt32(data["id"]));
                    var childdatachild = _context.ChildComments.Select(x => x).Where(x => x.父訊息id == childdata.父訊息id && x.子訊息id == childdata.訊息id);
                    if (childdatachild != null)
                    {
                        _context.ChildComments.RemoveRange(childdatachild);
                    }
                    _context.ChildComments.Remove(childdata);
                    _context.SaveChanges();
                    ProductDetailViewModels pdm = selectData(data);
                    return Json(pdm);
                }
                else
                {
                    return Content(null);
                }
            }
            catch {
                return Content("刪除錯誤");
            }
        }
        public IActionResult EditComment(IFormCollection data)
        {
            try
            {
                if (Convert.ToInt32(data["order"]) == 1 && data["comment"] != "")
                {
                    var parentdata = _context.ParentComments.FirstOrDefault(x => x.訊息id == Convert.ToInt32(data["id"]));
                    parentdata.內容 = data["comment"];
                    _context.Update(parentdata);
                    _context.SaveChanges();
                    ProductDetailViewModels pdm = selectData(data);
                    if (data["paretid"] != "undefined" && data["paretid"] !="")
                    {
                        pdm.collapseParetid = Convert.ToInt32(data["paretid"]);
                    }
                    return Json(pdm);
                }
                else if (Convert.ToInt32(data["order"]) == 2 && data["comment"] != "")
                {
                    var childdata = _context.ChildComments.FirstOrDefault(x => x.訊息id == Convert.ToInt32(data["id"]));
                    childdata.內容 = data["comment"];
                    _context.Update(childdata);
                    _context.SaveChanges();
                    ProductDetailViewModels pdm = selectData(data);
                    if (data["paretid"] != "undefined" && data["paretid"] != "")
                    {
                        pdm.collapseParetid = Convert.ToInt32(data["paretid"]);
                    }
                    return Json(pdm);
                }
                else
                {
                    return Content(null);
                }
            }
            catch {
                return Content("編輯錯誤");
            }

        }


    }
}

