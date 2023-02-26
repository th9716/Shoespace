using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dbCompanyTest.Models;
using dbCompanyTest.Hubs;
using NPOI.XWPF.UserModel;

namespace dbCompanyTest.Controllers
{
    public class OrdersController : Controller
    {
        dbCompanyTestContext _context = new dbCompanyTestContext();
        //private readonly dbCompanyTestContext _context;

        //public OrdersController(dbCompanyTestContext context)
        //{
        //    _context = context;
        //}

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var dbCompanyTestContext = _context.Orders.Include(o => o.客戶編號Navigation).OrderByDescending(o =>o.訂單編號);
            return View(await dbCompanyTestContext.ToListAsync());
        }

        public IActionResult Search(string keyPoint)
        {
            List<Order> queryList = _context.Orders.ToList();
            if (keyPoint == null)
            {
                return Json(queryList);
            }
            else
            {
                List<Order> clients = queryList.Where(x => x.下單時間.Contains(keyPoint)||x.收件人名稱.Contains(keyPoint)||x.訂單編號.Contains(keyPoint)||x.客戶編號.Contains(keyPoint)||x.送貨地址.Contains(keyPoint)||x.付款狀態.Contains(keyPoint)).ToList();
                if (clients.Count() == 0)
                {
                    return Json("沒有找到資料");
                }
                else
                {
                    return Json(clients);
                }
            }
        }

        public IActionResult getOrderDetails()
        {
            var datas = from c in _context.Orders
                        join o in _context.OrderDetails on c.訂單編號 equals o.訂單編號
                        join a in _context.ProductDetails on o.Id equals a.Id
                        join b in _context.ProductsSizeDetails on a.商品尺寸id equals b.商品尺寸id
                        join d in _context.ProductsColorDetails on a.商品顏色id equals d.商品顏色id
                        join e in _context.Products on a.商品編號id equals e.商品編號id
                        orderby o.無用id
                        select new ViewModels.OrderDetail_List
                        {
                            無用ID=o.無用id,
                            Id = o.Id,
                            訂單編號 = c.訂單編號,
                            送貨地址 = c.送貨地址,
                            商品名稱 = e.商品名稱,
                            尺寸種類 = b.尺寸種類,
                            色碼 = d.色碼,
                            商品數量 = (int)o.商品數量,
                            商品價格=(int)o.商品價格,
                            總金額= (int)o.商品價格* (int)o.商品數量,
                        };
            var test = datas.ToList();
            return Json(datas);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["客戶編號"] = new SelectList(_context.TestClients, "客戶編號", "客戶編號");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("訂單編號,客戶編號,付款方式,送貨地址,總金額,下單時間,訂單狀態,付款狀態,收件人名稱,收件人電話,收件人email")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["客戶編號"] = new SelectList(_context.TestClients, "客戶編號", "客戶編號", order.客戶編號);
            return View(order);
        }

        public async Task<IActionResult> EditDetail(int id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            var ee=  ViewData["aa"] = new SelectList(_context.ProductDetails, "Id", "Id", orderDetail.Id);
            ViewData["訂單編號"] = new SelectList(_context.Orders, "訂單編號", "訂單編號", orderDetail.訂單編號);
            
            return View(orderDetail);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDetail(int id, [Bind("無用id,訂單編號,Id,商品價格,商品數量,總金額l")] OrderDetail orderDetail)
        {
            if (id != orderDetail.無用id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.無用id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id"] = new SelectList(_context.ProductDetails, "Id", "Id", orderDetail.Id);
            ViewData["訂單編號"] = new SelectList(_context.Orders, "訂單編號", "訂單編號", orderDetail.訂單編號);
            return View(orderDetail);
        }

        public IActionResult EditOrderDetailToSQL(OrderDetail orderDetail)
        {
            dbCompanyTestContext _context = new dbCompanyTestContext();
            OrderDetail data = orderDetail;
            _context.Update(data);
            _context.SaveChanges();
            return Content("OK");
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["客戶編號"] = new SelectList(_context.TestClients, "客戶編號", "客戶編號", order.客戶編號);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("訂單編號,客戶編號,付款方式,送貨地址,總金額,下單時間,訂單狀態,付款狀態,收件人名稱,收件人電話,收件人email")] Order order)
        {
            if (id != order.訂單編號)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.訂單編號))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["客戶編號"] = new SelectList(_context.TestClients, "客戶編號", "客戶編號", order.客戶編號);
            return View(order);
        }

        public IActionResult DeleteOrder(string id)
        {
            var order = _context.Orders.Select(x => x).Where(c => c.訂單編號 == id).ToList();
            var orderDitail = _context.OrderDetails.Select(x => x).Where(c => c.訂單編號 == id).ToList();
            _context.Orders.RemoveRange(order);
            _context.OrderDetails.RemoveRange(orderDitail);
            _context.SaveChanges();
            return Content("ok");
            //return Json(order);
        }
        public IActionResult DeleteOrderDitail(int id)
        {
            var orderDitail = _context.OrderDetails.Select(x => x).Where(c => c.無用id == id).ToList();
            _context.OrderDetails.RemoveRange(orderDitail);
            _context.SaveChanges();
            return Content("ok");
            //return Json(order);
        }
        // GET: Orders/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null || _context.Orders == null)
        //    {
        //        return NotFound();
        //    }

        //    //var order = await _context.Orders
        //    //    .Include(o => o.客戶編號Navigation)
        //    //    .FirstOrDefaultAsync(m => m.訂單編號 == id);
        //    Order a = null;
        //    var order = _context.Orders.Select(x => x).Where(c => c.訂單編號 == id);/*.ToList();*/
        //    var orderDitail = _context.OrderDetails.Select(x => x).Where(c => c.訂單編號 == id).ToList();
        //    a = (Order)order;
        //    //var a = "";
        //    //if (orderDitail.Any())
        //    //a = orderDitail;
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    //return View(order);
        //    return View(a);
        //}

        //// POST: Orders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    if (_context.Orders == null)
        //    {
        //        return Problem("Entity set 'dbCompanyTestContext.Orders'  is null.");
        //    }
        //    var order = await _context.Orders.FindAsync(id);
        //    if (order != null)
        //    {
        //        _context.Orders.Remove(order);
        //    }
            
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool OrderExists(string id)
        {
          return _context.Orders.Any(e => e.訂單編號 == id);
        }
        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.無用id == id);
        }
    }
}
