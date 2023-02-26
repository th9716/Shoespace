using dbCompanyTest.Hubs;
using dbCompanyTest.Models;
using dbCompanyTest.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NPOI.SS.Formula.Atp;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace dbCompanyTest.Controllers
{
    public class MyLoveController : Controller
    {
        dbCompanyTestContext _context = new dbCompanyTestContext();
        public IActionResult Index()
        {
            //判斷是否登入
            if (HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION))
            {
                //取得Login Session
                string login = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION);
                var user = JsonSerializer.Deserialize<TestClient>(login);
                //判斷之前是否有Session
                if (!HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_MYLOVE_SESSION))
                {
                    //開啟MyloveSession
                    var data = _context.會員商品暫存s.Select(x => x).Where(y => y.購物車或我的最愛 == false && y.客戶編號.Contains(user.客戶編號)).ToList();
                    if (data.Count != 0)
                    {
                        string json = JsonSerializer.Serialize(data);
                        HttpContext.Session.SetString(CDittionary.SK_USE_FOR_MYLOVE_SESSION, json);
                        return View(data);
                    }
                    return View(data);
                }
                else
                {
                    var json = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_MYLOVE_SESSION);
                    var data = JsonSerializer.Deserialize<List<會員商品暫存>>(json);
                    return View(data);
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        //待新增功能加入購物車之前判斷商品總數量是否超過10樣
        public IActionResult Create(會員商品暫存 prod)
        {
            if (HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION))
            {
                //判斷是否登入
                string userjson = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION);
                var userinfo = JsonSerializer.Deserialize<TestClient>(userjson);

                if (prod.購物車或我的最愛 == false)
                {
                    //加入我的最愛
                    if (HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_MYLOVE_SESSION))
                    {
                        string json = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_MYLOVE_SESSION);
                        var datas = JsonSerializer.Deserialize<List<會員商品暫存>>(json);
                        foreach (var item in datas)
                        {
                            if (item.商品編號 == prod.商品編號 && item.商品顏色種類.Equals(prod.商品顏色種類) && item.尺寸種類.Equals(prod.尺寸種類))
                            {
                                return Content("收藏清單已有相同商品");
                            }
                        }
                        
                        會員商品暫存 data = prod;
                        data.Id = datas.Count+1;
                        data.客戶編號 = userinfo.客戶編號;
                        data.訂單數量 = 1;
                        data.購物車或我的最愛 = false;
                        datas.Add(data);
                        json = JsonSerializer.Serialize(datas);
                        HttpContext.Session.SetString(CDittionary.SK_USE_FOR_MYLOVE_SESSION, json);
                        return Content("加入收藏成功");
                    }
                    else
                    {
                        var list = _context.會員商品暫存s.Select(x => x).Where(y => y.購物車或我的最愛 == false && y.客戶編號.Contains(userinfo.客戶編號)).ToList();
                        foreach (var item in list)
                        {
                            if (item.商品編號 == prod.商品編號 && item.商品顏色種類.Equals(prod.商品顏色種類) && item.尺寸種類.Equals(prod.尺寸種類))
                            {
                                return Content("收藏清單已有相同商品");
                            }
                        }
                        List<會員商品暫存> datas = new List<會員商品暫存>();
                        會員商品暫存 data = prod;
                        data.客戶編號 = userinfo.客戶編號;
                        data.Id = datas.Count + 1;
                        data.訂單數量 = 1;
                        data.購物車或我的最愛 = false;
                        datas.Add(data);
                        datas.AddRange(list);
                        string json = JsonSerializer.Serialize(datas);
                        HttpContext.Session.SetString(CDittionary.SK_USE_FOR_MYLOVE_SESSION, json);
                        return Content("加入收藏成功");
                    }

                }

                else
                {
                    //加入購物車
                    if (HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION))
                    {
                        string cartjson = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION);
                        List<會員商品暫存> shoppingcart = new List<會員商品暫存>();
                        shoppingcart.AddRange((JsonSerializer.Deserialize<List<會員商品暫存>>(cartjson)).ToArray());
                        foreach (var item in shoppingcart)
                        {
                            if (item.商品編號 == prod.商品編號 && item.商品顏色種類.Equals(prod.商品顏色種類) && item.尺寸種類.Equals(prod.尺寸種類))
                            {
                                item.訂單數量 = item.訂單數量 + prod.訂單數量;
                                cartjson = JsonSerializer.Serialize(shoppingcart);
                                HttpContext.Session.SetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION, cartjson);
                                return Content("加入購物車成功");
                            }
                        }
                        會員商品暫存 data = prod;
                        data.客戶編號 = userinfo.客戶編號;
                        data.訂單數量 = prod.訂單數量;
                        shoppingcart.Add(data);
                        cartjson = JsonSerializer.Serialize(shoppingcart);
                        HttpContext.Session.SetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION, cartjson);
                        return Content("加入購物車成功");
                    }
                    else
                    {
                        string cartjson = "";
                        List<會員商品暫存> shoppingcart = new List<會員商品暫存>();
                        shoppingcart.AddRange((_context.會員商品暫存s.Select(x => x).Where(y => y.購物車或我的最愛 == true && y.客戶編號.Contains(userinfo.客戶編號))).ToArray());
                        foreach (var item in shoppingcart)
                        {
                            if (item.商品編號 == prod.商品編號 && item.商品顏色種類.Equals(prod.商品顏色種類) && item.尺寸種類.Equals(prod.尺寸種類))
                            {
                                item.訂單數量 = item.訂單數量 + prod.訂單數量;
                                cartjson = JsonSerializer.Serialize(shoppingcart);
                                HttpContext.Session.SetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION, cartjson);
                                return Content("加入購物車成功");
                            }
                        }
                        會員商品暫存 data = prod;
                        data.客戶編號 = userinfo.客戶編號;
                        data.訂單數量 = prod.訂單數量;
                        shoppingcart.Add(data);
                        cartjson = JsonSerializer.Serialize(shoppingcart);
                        HttpContext.Session.SetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION, cartjson);
                        return Content("加入購物車成功");
                    }
                }
            }
            //未登入創建CartSession
            else if (!(HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION)) && prod.購物車或我的最愛 == true)
            {

                //加入購物車
                if (HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION))
                {
                    string cartjson = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION);
                    List<會員商品暫存> shoppingcart = new List<會員商品暫存>();
                    shoppingcart.AddRange((JsonSerializer.Deserialize<List<會員商品暫存>>(cartjson)).ToArray());
                    foreach (var item in shoppingcart)
                    {
                        if (item.商品編號 == prod.商品編號 && item.商品顏色種類.Equals(prod.商品顏色種類) && item.尺寸種類.Equals(prod.尺寸種類))
                        {
                            item.訂單數量 = item.訂單數量 + prod.訂單數量;
                            cartjson = JsonSerializer.Serialize(shoppingcart);
                            HttpContext.Session.SetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION, cartjson);
                            return Content("加入購物車成功");
                        }
                    }
                    會員商品暫存 data = prod;
                    data.訂單數量 = prod.訂單數量;
                    shoppingcart.Add(data);
                    cartjson = JsonSerializer.Serialize(shoppingcart);
                    HttpContext.Session.SetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION, cartjson);
                    return Content("加入購物車成功");
                }
                else
                {
                    string cartjson = "";
                    List<會員商品暫存> shoppingcart = new List<會員商品暫存>();
                    會員商品暫存 data = prod;
                    data.訂單數量 = prod.訂單數量;
                    shoppingcart.Add(data);
                    cartjson = JsonSerializer.Serialize(shoppingcart);
                    HttpContext.Session.SetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION, cartjson);
                    return Content("加入購物車成功");
                }
            }
            else
            {
                return Content("請先登入會員");
            }
        }
        public IActionResult Delete(int? id)
        {
            if (id != null)
            {
                string json = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_MYLOVE_SESSION);
                var datas = JsonSerializer.Deserialize<List<會員商品暫存>>(json);
                var data = datas.FirstOrDefault(x => x.Id == id);
                datas.Remove(data);
                json = JsonSerializer.Serialize(datas);
                HttpContext.Session.SetString(CDittionary.SK_USE_FOR_MYLOVE_SESSION, json);
                return Content("刪除成功");
            }
            else
            {
                return Content("沒有這筆資料");
            }
        }
        public IActionResult JoinCart(int? id)
        {
            if (HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION))
            {
                List<會員商品暫存> shoppingcart = new List<會員商品暫存>();
                //讀出購物車session
                string cartjson = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION);
                shoppingcart.AddRange((JsonSerializer.Deserialize<List<會員商品暫存>>(cartjson)).ToArray());
                //讀出我的最愛session
                string lovejson = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_MYLOVE_SESSION);
                var datas = JsonSerializer.Deserialize<List<會員商品暫存>>(lovejson);
                會員商品暫存 data = null;
                data = datas.FirstOrDefault(x => x.Id == id);
                data.購物車或我的最愛 = true;

                foreach (var item in shoppingcart)
                {
                    if (item.商品編號 == data.商品編號 && item.商品顏色種類.Equals(data.商品顏色種類) && item.尺寸種類.Equals(data.尺寸種類))
                    {
                        item.訂單數量++;
                        cartjson = JsonSerializer.Serialize(shoppingcart);
                        HttpContext.Session.SetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION, cartjson);
                        return Content("加入購物車成功");
                    }
                }
                shoppingcart.Add(data);
                cartjson = JsonSerializer.Serialize(shoppingcart);
                HttpContext.Session.SetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION, cartjson);
                return Content("加入購物車成功");
            }
            else
            {
                List<會員商品暫存> shoppingcart = new List<會員商品暫存>();
                string cartjson = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION);
                var user = JsonSerializer.Deserialize<TestClient>(cartjson);
                shoppingcart.AddRange(_context.會員商品暫存s.Select(x => x).Where(y => y.購物車或我的最愛 == true && y.客戶編號.Contains(user.客戶編號)).ToArray());
                string lovejson = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_MYLOVE_SESSION);
                var datas = JsonSerializer.Deserialize<List<會員商品暫存>>(lovejson);
                會員商品暫存 data = null;
                data = datas.FirstOrDefault(x => x.Id == id);
                data.購物車或我的最愛 = true;
                foreach (var item in shoppingcart)
                {
                    if (item.商品編號 == data.商品編號 && item.商品顏色種類.Equals(data.商品顏色種類) && item.尺寸種類.Equals(data.尺寸種類))
                    {
                        item.訂單數量++;
                        cartjson = JsonSerializer.Serialize(shoppingcart);
                        HttpContext.Session.SetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION, cartjson);
                        return Content("加入購物車成功");
                    }
                }
                shoppingcart.Add(data);
                cartjson = JsonSerializer.Serialize(shoppingcart);
                HttpContext.Session.SetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION, cartjson);
                return Content("加入購物車成功");
            }
        }
        public IActionResult checkCount(int? productid, int? colorid, int? sizeid, int? count)
        {
            var data = _context.ProductDetails.FirstOrDefault(x => x.商品編號id == productid && x.商品顏色id == colorid && x.商品尺寸id == sizeid);
            if (HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION))
            {
                //代處理是否合併到create
                string cartjson = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION);
                var shoppingcart = JsonSerializer.Deserialize<List<會員商品暫存>>(cartjson);
                var list = from cc in _context.ProductDetails
                           join procolor in _context.ProductsColorDetails on cc.商品顏色id equals procolor.商品顏色id
                           join prosize in _context.ProductsSizeDetails on cc.商品尺寸id equals prosize.商品尺寸id
                           where cc.Id == data.Id
                           select new
                           {
                               productsize = prosize.尺寸種類,
                               productcolor = procolor.商品顏色種類
                           };
                foreach (var item in shoppingcart)
                {
                    foreach (var item2 in list)
                    {
                        if (item.商品編號 == productid && item.商品顏色種類.Equals(item2.productcolor) && item.尺寸種類.Equals(item2.productsize))
                        {
                            if ((item.訂單數量 + count) > data.商品數量)
                            {
                                return Content("false");
                            }
                        }


                    }
                }
                return Content("true");

            }
            if (data.商品數量 < count)
                return Content("false");
            else
                return Content("true");
        }
    }
}
