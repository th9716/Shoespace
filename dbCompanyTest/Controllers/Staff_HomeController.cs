using dbCompanyTest.Models;
using dbCompanyTest.ViewModels;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Pdf;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Net.Mail;
using System.Runtime.Intrinsics.Arm;
using System.Security.Policy;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using static dbCompanyTest.Controllers.ProductController;
using static NuGet.Packaging.PackagingConstants;

namespace dbCompanyTest.Controllers
{
    public class Staff_HomeController : Controller
    {
        dbCompanyTestContext _context = new dbCompanyTestContext();
        private IWebHostEnvironment _environment;
        public Staff_HomeController(IWebHostEnvironment p)
        {
            _environment = p;
        }
        public IActionResult Staff_Information()
        {
            string json = HttpContext.Session.GetString(CDittionary.SK_STAFF_INFO_SESSION);
            if (json == null)
            {
                return RedirectToAction("login");
            }
            var data = JsonSerializer.Deserialize<TestStaff>(json);
            CStaffInfo info = new CStaffInfo();
            info.stfNum = data.員工編號;
            info.stfName = data.員工姓名;
            info.stfDep = data.部門;
            ViewBag.inf = info;

            string stfNum = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);
            ViewBag.acc = $"{stfNum} {stf.員工姓名} {stf.部門}";
            ViewBag.dep = stf.部門;
            return Ok();
        }
        public IActionResult Index()
        {
            Staff_Information();
            string stfNum = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);
            ViewBag.HR = stf;
            if (stf == null)
            {
                return RedirectToAction("login");
            }
            if (stf.部門 == "行政" || stf.部門 == "執行長室")
                return View();
            else
                return RedirectToAction("Index_HR");
        }
        public async Task<IActionResult> Index_HR()
        {
            Staff_Information();
            string stfNum = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);
            ViewBag.HR = stf.員工編號;
            if (stf.部門 == "人事")
                return View(await _context.TestStaffs.ToListAsync());
            else return RedirectToAction("Index");
        }

        public IActionResult login()
        {
            //HttpContext.Session.Remove("Account");
            string stfNum = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
            if (HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
            {
                TestStaff x = _context.TestStaffs.FirstOrDefault(T => T.員工編號 == stfNum);
                if (x.部門 == "行政" || x.部門 == "執行長室")
                {
                    return RedirectToAction("Index");
                }
                else if (x.部門 == "人事")
                {
                    return RedirectToAction("Index_HR");
                }
            }
            return View();
        }
        [HttpPost]
        public IActionResult login(/*CLoginViewModels vm, */string account, string password)
        {
            TestStaff x = _context.TestStaffs.FirstOrDefault(T => T.員工編號.Equals(account) && T.密碼.Equals(password));
            if (x != null)
            {
                if (x.密碼.Equals(password) && x.員工編號.Equals(account))
                {
                    string json = System.Text.Json.JsonSerializer.Serialize(x);
                    if (!HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
                    {
                        HttpContext.Session.SetString(CDittionary.SK_STAFF_NUMBER_SESSION, account);
                        HttpContext.Session.SetString(CDittionary.SK_STAFF_INFO_SESSION, json);
                    }
                    return Content("success");
                }
            }
            else
            {
                if (account == null || password == null)
                {
                    return Content("CantNull");
                }
                return Content("false");
            }
            return View();
        }
        public IActionResult logout()
        {
            HttpContext.Session.Remove(CDittionary.SK_STAFF_NUMBER_SESSION);
            HttpContext.Session.Remove(CDittionary.SK_STAFF_INFO_SESSION);
            return RedirectToAction("login");
        }
        public IActionResult PartialSheeplist()
        {
            return PartialView();
        }
        public IActionResult PartialToDoList()
        {
            return PartialView();
        }
        public IActionResult Create_TDL()
        {
            Staff_Information();
            return View();
        }

        public IActionResult DT_TDL(int listNum, string listType)
        {

            Staff_Information();
            if (listType == "人事表單")
            {
                return RedirectToAction("DT_TDL_HR", new { listNum = listNum });
            }
            else
            {
                var data = _context.ToDoLists.FirstOrDefault(c => c.交辦事項id == listNum);
                //return RedirectToAction("DT_TDL_RG", new { listNum = listNum });
                return View(data);
            }

        }
        public IActionResult DT_TDL_HR(int listNum)
        {
            CToDoListViewModels cToDoListViewModels = new CToDoListViewModels();
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
            {
                return RedirectToAction("login");
            }
            string stfNum = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
            //var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);
            var data = _context.ToDoLists.FirstOrDefault(c => c.交辦事項id == listNum);

            var datas = from c in _context.TestStaffs select c;
            var str_dep = datas.FirstOrDefault(c => c.員工編號 == stfNum).部門;/*from t in datas where t.員工編號 == stfNum select t.部門*/;
            var bos_dep = datas.FirstOrDefault(c => c.員工編號 == "ST2-0010170").部門;/*from b in datas where b.員工編號 == "ST2-0010170" select b.部門*/;
            var exc_dep = datas.FirstOrDefault(c => c.員工編號 == data.執行人).部門;/*from d in datas where d.員工編號 == data.執行人 select d.部門;*/
            var strName = datas.FirstOrDefault(c => c.員工編號 == data.起單人).員工姓名;
            var spvName = datas.FirstOrDefault(c => c.員工編號 == data.部門主管).員工姓名;
            var codName = datas.FirstOrDefault(c => c.員工編號 == data.協辦部門簽核人員).員工姓名;
            var bosName = datas.FirstOrDefault(c => c.員工編號 == "ST2-0010170").員工姓名;
            var ecuName = datas.FirstOrDefault(c => c.員工編號 == data.執行人).員工姓名;
            var stf = datas.FirstOrDefault(c => c.員工編號 == stfNum);

            cToDoListViewModels.交辦事項id = listNum;
            cToDoListViewModels.員工編號 = data.員工編號;

            cToDoListViewModels.起單人 = $"{data.起單人} {strName}";
            cToDoListViewModels.部門主管 = $"{data.部門主管} {spvName}";
            cToDoListViewModels.部門主管簽核 = data.部門主管簽核;
            cToDoListViewModels.部門主管簽核意見 = data.部門主管簽核意見;
            cToDoListViewModels.部門主管簽核時間 = data.部門主管簽核時間;

            cToDoListViewModels.協辦部門簽核人員 = $"{data.協辦部門簽核人員} {codName}";
            cToDoListViewModels.協辦部門簽核 = data.協辦部門簽核;
            cToDoListViewModels.協辦部門簽核意見 = data.協辦部門簽核意見;
            cToDoListViewModels.協辦部門簽核時間 = data.協辦部門簽核時間;

            cToDoListViewModels.老闆 = $"ST2-0010170 {bosName}";
            cToDoListViewModels.老闆簽核 = data.老闆簽核;
            cToDoListViewModels.老闆簽核意見 = data.老闆簽核意見;
            cToDoListViewModels.老闆簽核時間 = data.老闆簽核時間;

            cToDoListViewModels.執行人 = $"{data.執行人} {ecuName}";
            cToDoListViewModels.執行人簽核 = data.執行人簽核;
            cToDoListViewModels.回覆 = data.回覆;
            cToDoListViewModels.執行時間 = data.執行時間;

            cToDoListViewModels.表單狀態 = data.表單狀態;
            cToDoListViewModels.表單內容 = data.表單內容;
            cToDoListViewModels.起單時間 = data.起單時間;
            cToDoListViewModels.表單類型 = data.表單類型;

            cToDoListViewModels.起單部門 = str_dep.ToString();
            cToDoListViewModels.老闆部門 = bos_dep.ToString();
            cToDoListViewModels.執行部門 = exc_dep.ToString();
            cToDoListViewModels.協辦部門 = data.協辦部門;

            cToDoListViewModels.附件 = data.附件;
            cToDoListViewModels.附件path = data.附件path;

            ViewBag.acc = $"{stf.部門} {stfNum} {stf.員工姓名}";
            ViewBag.dep = stf.部門;
            return View(cToDoListViewModels);
        }
        public IActionResult DT_TDL_RG(int listNum)
        {
            Staff_Information();
            var data = _context.ToDoLists.FirstOrDefault(c => c.交辦事項id == listNum);
            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DT_TDL(CToDoListViewModels cToDoListViewModels/*int id, [Bind("交辦事項id,員工編號,表單類型,表單內容,回覆,表單狀態,起單時間,起單人,部門主管,部門主管簽核,部門主管簽核意見,部門主管簽核時間,協辦部門,協辦部門簽核,協辦部門簽核人員,協辦部門簽核意見,協辦部門簽核時間,老闆簽核,老闆簽核意見,老闆簽核時間,執行人,執行時間,執行人簽核,附件,附件path")] *//*, ToDoList toDoList*/)
        {
            var thislist = _context.ToDoLists.FirstOrDefault(t => t.交辦事項id == cToDoListViewModels.交辦事項id);

            if (ModelState.IsValid)
            {
                try
                {
                    if (cToDoListViewModels.File != null/* && thislist != null*/)
                    {
                        if (thislist != null)
                        {
                            string oldPath = _environment.WebRootPath + "/File/" + thislist.附件path;
                            if (System.IO.File.Exists(oldPath))
                                System.IO.File.Delete(oldPath);
                        }
                        string FileNameSub = cToDoListViewModels.File.FileName;
                        string[] words = FileNameSub.Split('.');
                        int FileTypeIndex = words.Length;
                        string FileType = words[FileTypeIndex - 1];

                        string FileName = $"{Guid.NewGuid().ToString()}.{FileType}";
                        string path = _environment.WebRootPath + "/File/" + FileName;
                        thislist.附件path = FileName;

                        using (FileStream file = new FileStream(path, FileMode.Create))
                        {
                            cToDoListViewModels.File.CopyTo(file);
                        }
                    }
                    if (thislist != null && cToDoListViewModels.表單類型 == "人事表單")
                    {
                        if (System.IO.File.Exists(_environment.WebRootPath + "/File/" + thislist.附件))
                        {
                            string oldpath = _environment.WebRootPath + "/File/" + thislist.附件;
                            string NewFileName = $"{Guid.NewGuid().ToString()}.pdf";
                            string NewFileNamePath = $"{_environment.WebRootPath}/File/{NewFileName}";


                            if (cToDoListViewModels.部門主管簽核 != thislist.部門主管簽核 && cToDoListViewModels.部門主管簽核 == "敬陳")
                            {
                                AddImg(oldpath, NewFileNamePath, $"{_environment.WebRootPath}/Sign/{thislist.部門主管}.jpg", 158, 545);
                                thislist.附件 = NewFileName;
                                if (System.IO.File.Exists(oldpath))
                                    System.IO.File.Delete(oldpath);
                            }
                            if (cToDoListViewModels.協辦部門簽核 != thislist.協辦部門簽核 && cToDoListViewModels.協辦部門簽核 == "敬陳")
                            {
                                AddImg(oldpath, NewFileNamePath, $"{_environment.WebRootPath}/Sign/{thislist.協辦部門簽核人員}.jpg", 262, 545);
                                thislist.附件 = NewFileName;
                                if (System.IO.File.Exists(oldpath))
                                    System.IO.File.Delete(oldpath);
                            }
                            if (cToDoListViewModels.老闆簽核 != thislist.老闆簽核 && cToDoListViewModels.老闆簽核 == "敬陳")
                            {
                                AddImg(oldpath, NewFileNamePath, $"{_environment.WebRootPath}/Sign/ST2-0010170.jpg", 366, 545);
                                thislist.附件 = NewFileName;
                                if (System.IO.File.Exists(oldpath))
                                    System.IO.File.Delete(oldpath);
                            }
                            if (cToDoListViewModels.執行人簽核 != thislist.執行人簽核 && cToDoListViewModels.執行人簽核 == "完成")
                            {
                                AddImg(oldpath, NewFileNamePath, $"{_environment.WebRootPath}/Sign/{thislist.執行人}.jpg", 470, 545);
                                thislist.附件 = NewFileName;
                                if (System.IO.File.Exists(oldpath))
                                    System.IO.File.Delete(oldpath);
                            }
                            if (cToDoListViewModels.表單狀態 == "退回起單人")
                            {
                                AddImgAllBack(oldpath, NewFileNamePath, $"{_environment.WebRootPath}/Sign/空白.jpg");
                                thislist.附件 = NewFileName;
                                if (System.IO.File.Exists(oldpath))
                                    System.IO.File.Delete(oldpath);
                            }
                        }
                    }
                    //_context.Update(toDoList/*cToDoListViewModels.toDoList*/);
                    thislist.部門主管簽核 = cToDoListViewModels.部門主管簽核;
                    thislist.部門主管簽核意見 = cToDoListViewModels.部門主管簽核意見;
                    thislist.部門主管簽核時間 = cToDoListViewModels.部門主管簽核時間;

                    thislist.協辦部門簽核 = cToDoListViewModels.協辦部門簽核;
                    thislist.協辦部門簽核意見 = cToDoListViewModels.協辦部門簽核意見;
                    thislist.協辦部門簽核時間 = cToDoListViewModels.協辦部門簽核時間;

                    thislist.老闆簽核 = cToDoListViewModels.老闆簽核;
                    thislist.老闆簽核意見 = cToDoListViewModels.老闆簽核意見;
                    thislist.老闆簽核時間 = cToDoListViewModels.老闆簽核時間;

                    thislist.執行人簽核 = cToDoListViewModels.執行人簽核;
                    thislist.回覆 = cToDoListViewModels.回覆;
                    thislist.執行時間 = cToDoListViewModels.執行時間;

                    thislist.表單狀態 = cToDoListViewModels.表單狀態;
                    thislist.表單內容 = cToDoListViewModels.表單內容;
                    thislist.起單時間 = cToDoListViewModels.起單時間;


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoListExists(cToDoListViewModels.交辦事項id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("DT_TDL_HR", new { listNum = thislist.交辦事項id });
            }
            return RedirectToAction("Index_HR");
        }
        private bool ToDoListExists(int id)
        {
            return _context.ToDoLists.Any(e => e.交辦事項id == id);
        }

        public IActionResult PartialSchedule_HR()
        {
            return PartialView();
        }
        public IActionResult PartialSchedule_RG()
        {
            return PartialView();
        }


        //==================================================
        public IActionResult LoadSheeplist()
        {
            var datas = from c in _context.Orders
                        join o in _context.OrderDetails on c.訂單編號 equals o.訂單編號
                        join a in _context.ProductDetails on o.Id equals a.Id
                        join b in _context.ProductsSizeDetails on a.商品尺寸id equals b.商品尺寸id
                        join d in _context.ProductsColorDetails on a.商品顏色id equals d.商品顏色id
                        join e in _context.Products on a.商品編號id equals e.商品編號id
                        orderby c.訂單編號 descending
                        where c.訂單狀態 == "待出貨"
                        select new ViewModels.Cback_order_list
                        {
                            訂單編號 = c.訂單編號,
                            客戶編號 = c.客戶編號,
                            送貨地址 = c.送貨地址,
                            商品名稱 = e.商品名稱,
                            尺寸種類 = b.尺寸種類,
                            色碼 = d.色碼,
                            商品數量 = (int)o.商品數量,
                            付款方式 = c.付款方式
                        };
            var test = datas.ToList();
            return Json(datas);
        }

        public IActionResult LoadToDoList(string stf)
        {
            if (stf == "ST2-0010170")
            {
                var datas = from c in _context.ToDoLists
                            where c.員工編號 == stf || c.起單人 == stf || c.執行人 == stf || c.表單類型 == "人事表單"
                            select c;
                var data = from c in datas
                           where c.表單狀態 != "完成" && c.表單狀態 != "作廢"
                           select c;
                return Json(data);
            }
            else
            {
                var datas = from c in _context.ToDoLists
                            where c.員工編號 == stf || c.協辦部門簽核人員 == stf || c.部門主管 == stf || c.起單人 == stf || c.執行人 == stf
                            select c;
                var data = from c in datas
                           where c.表單狀態 != "完成" && c.表單狀態 != "作廢"
                           select c;
                return Json(data);
            }


        }
        public IActionResult stf_info(string stf)
        {
            TestStaff datas = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stf);
            return Json(datas);
        }
        public IActionResult all_stf_info()
        {
            var datas = from c in _context.TestStaffs select c;
            return Json(datas);
        }
        public IActionResult stf_info_dep(string dep)
        {
            var datas = from c in _context.TestStaffs
                        where c.部門 == dep
                        select c;
            var data = (from t in datas
                        orderby Guid.NewGuid()
                        select t).Take(2);
            return Json(data);
        }

        public IActionResult stf_info1(string i_whostart, string s_executor, string i_spvs, string i_co, string i_boss)
        {
            List<TestStaff> data = new List<TestStaff>();
            var datas = from c in _context.TestStaffs select c;
            var data1 = from t in datas where t.員工編號 == i_whostart select t;
            var data2 = from b in datas where b.員工編號 == i_spvs select b;
            var data3 = from d in datas where d.員工編號 == i_co select d;
            var data4 = from f in datas where f.員工編號 == i_boss select f;
            var data5 = from a in datas where a.員工編號 == s_executor select a;
            data.AddRange(data1); data.AddRange(data2); data.AddRange(data3);
            data.AddRange(data4); data.AddRange(data5);


            return Json(data);
        }
        [HttpPost]
        public IActionResult forgetPassword(string account)
        {
            //var x = from t in _context.TestStaffs where t.員工編號 == account select t; 
            TestStaff x = _context.TestStaffs.FirstOrDefault(T => T.員工編號 == account);
            if (x != null)
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                msg.To.Add(x.Email);
                msg.From = new MailAddress("msit145finalpj@gmail.com", "Shoespace", System.Text.Encoding.UTF8);
                msg.Subject = "員工忘記密碼";
                msg.SubjectEncoding = System.Text.Encoding.UTF8;//主旨編碼
                msg.Body = $"<h5 id=\"stf_info\">{x.員工編號} {x.員工姓名} 您好!</h5>";
                msg.Body += $"<a href=`{Environment.Environment.useEnvironment}/Staff_Home/ResetPassword?account={account}`>點選此連結變更密碼</a>";
                msg.BodyEncoding = System.Text.Encoding.UTF8;//內文編碼
                msg.IsBodyHtml = true; //!!!

                SmtpClient smtp = new SmtpClient();
                smtp.Credentials = new System.Net.NetworkCredential("msit145finalpj@gmail.com", "zlazqafpmuwxkxvo");
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(msg);
                msg.Dispose();

                return Json("請至信箱接收密碼更改信件");
            }
            else
                return Json("沒有這個帳號");
        }
        public IActionResult ResetPassword(string account)
        {
            ViewBag.account = account;
            return View();
        }
        [HttpPost]
        public IActionResult ResetPassword(string stf_info, string Password_F)
        {
            TestStaff datas = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stf_info);
            if (datas.密碼 == Password_F)
            {
                return Content("repeat");
            }
            else
            {
                datas.密碼 = Password_F;
                _context.SaveChanges();
                return Content("success");
            }
        }


        public IActionResult StaffNum()
        {
            string stfNum = "";
            if (HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
                stfNum = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
            else
                stfNum = "fales";
            return Content(stfNum);
        }

        public IActionResult TDLCount()
        {
            var datas = (from c in _context.ToDoLists select c).OrderByDescending(d => d.交辦事項id).FirstOrDefault().交辦事項id;
            var count = Convert.ToInt32(datas) + 1;

            return Content(count.ToString());
        }



        public IActionResult stfNum_and_inf0()
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_INFO_SESSION))
            {
                return RedirectToAction("login");
            }
            string json = HttpContext.Session.GetString(CDittionary.SK_STAFF_INFO_SESSION);
            var data = JsonSerializer.Deserialize<TestStaff>(json);
            CStaffInfo info = new CStaffInfo();
            info.stfNum = data.員工編號;
            info.stfName = data.員工姓名;
            info.stfDep = data.部門;
            info.stfpos = data.職稱;
            //ViewBag.acc = $"{stf.部門} {stfNum} {stf.員工姓名}";
            //ViewBag.dep = stf.部門;

            return Json(info);
        }
        public IActionResult ListWaiting()
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
            {
                return RedirectToAction("login");
            }
            string stfNum = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);
            ViewBag.acc = $"{stf.部門} {stfNum} {stf.員工姓名}";
            ViewBag.dep = stf.部門;
            IEnumerable<dbCompanyTest.Models.ToDoList> datas;
            if (stfNum == "ST2-0010170")
            {
                datas = from c in _context.ToDoLists
                        where (c.老闆簽核 == "待簽")
                        select c;
            }
            else
            {
                datas = from c in _context.ToDoLists
                        where (c.起單人 == stfNum && c.表單狀態 == "退回起單人") ||
                        (c.部門主管 == stfNum && c.部門主管簽核 == "待簽") ||
                        (c.協辦部門簽核人員 == stfNum && c.協辦部門簽核 == "待簽") ||
                        (c.執行人 == stfNum && c.執行人簽核 == "待簽")
                        select c;
            }

            return View(datas);
        }
        public IActionResult MyList()
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
            {
                return RedirectToAction("login");
            }
            string stfNum = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);
            ViewBag.acc = $"{stf.部門} {stfNum} {stf.員工姓名}";
            ViewBag.dep = stf.部門;

            var datas = from c in _context.ToDoLists
                        where c.起單人 == stfNum && c.表單狀態 != "完成" && c.表單狀態 != "作廢"
                        select c;
            return View(datas);
        }
        public IActionResult ListByMe()
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
            {
                return RedirectToAction("login");
            }
            string stfNum = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);
            ViewBag.acc = $"{stf.部門} {stfNum} {stf.員工姓名}";
            ViewBag.dep = stf.部門;
            IEnumerable<dbCompanyTest.Models.ToDoList> data;

            if (stfNum == "ST2-0010170")
            {
                var datas1 = from c in _context.ToDoLists
                             select c;
                data = from c in datas1
                       where c.表單狀態 != "完成" && c.表單狀態 != "作廢"
                       select c;
            }
            else
            {
                var datas = from c in _context.ToDoLists
                            where c.協辦部門簽核人員 == stfNum || c.部門主管 == stfNum || c.執行人 == stfNum
                            select c;
                data = from c in datas
                       where c.表單狀態 != "完成" && c.表單狀態 != "作廢"
                       select c;
            }


            return View(data);
        }

        public IActionResult ListDone()
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
            {
                return RedirectToAction("login");
            }
            string stfNum = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);
            ViewBag.acc = $"{stf.部門} {stfNum} {stf.員工姓名}";
            ViewBag.dep = stf.部門;
            IEnumerable<dbCompanyTest.Models.ToDoList> data;

            if (stfNum == "ST2-0010170")
            {
                var datas = from c in _context.ToDoLists select c;
                data = from c in datas
                       where c.表單狀態 == "完成"
                       select c;
            }
            else
            {
                var datas = from c in _context.ToDoLists
                            where c.員工編號 == stfNum || c.協辦部門簽核人員 == stfNum || c.部門主管 == stfNum || c.起單人 == stfNum || c.執行人 == stfNum
                            select c;
                data = from c in datas
                       where c.表單狀態 == "完成"
                       select c;
            }

            return View(data);
        }
        public IActionResult ListDis()
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
            {
                return RedirectToAction("login");
            }
            string stfNum = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);
            ViewBag.acc = $"{stf.部門} {stfNum} {stf.員工姓名}";
            ViewBag.dep = stf.部門;
            IEnumerable<dbCompanyTest.Models.ToDoList> data;

            if (stfNum == "ST2-0010170")
            {
                var datas = from c in _context.ToDoLists select c;
                data = from c in datas
                       where c.表單狀態 == "作廢"
                       select c;
            }
            else
            {
                var datas = from c in _context.ToDoLists
                            where c.員工編號 == stfNum || c.協辦部門簽核人員 == stfNum || c.部門主管 == stfNum || c.起單人 == stfNum || c.執行人 == stfNum
                            select c;
                data = from c in datas
                       where c.表單狀態 == "作廢"
                       select c;
            }

            return View(data);
        }

        public IActionResult Partial_List()
        {
            return PartialView();
        }
        protected void AddImg(string oldP, string newP, string imP, int x, int y)
        {
            using (Stream inputPdfStream = new FileStream(oldP, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream inputImageStream = new FileStream(imP, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream outputPdfStream = new FileStream(newP, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var reader = new iTextSharp.text.pdf.PdfReader(inputPdfStream);//讀取原有pdf
                var stamper = new PdfStamper(reader, outputPdfStream);
                var pdfContentByte = stamper.GetOverContent(1);//取內容
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(inputImageStream);//取圖片
                image.ScalePercent(75);//設置圖片比例

                image.SetAbsolutePosition(x, y);//圖片位置
                pdfContentByte.AddImage(image);
                stamper.Close();
            }
        }
        protected void AddImgAllBack(string oldP, string newP, string imP)
        {
            using (Stream inputPdfStream = new FileStream(oldP, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream inputImageStream = new FileStream(imP, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream outputPdfStream = new FileStream(newP, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var reader = new iTextSharp.text.pdf.PdfReader(inputPdfStream);
                var stamper = new PdfStamper(reader, outputPdfStream);
                var pdfContentByte = stamper.GetOverContent(1);
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(inputImageStream);
                image.ScalePercent(35);

                image.SetAbsolutePosition(158, 545);
                pdfContentByte.AddImage(image);
                image.SetAbsolutePosition(158, 545);
                pdfContentByte.AddImage(image);
                image.SetAbsolutePosition(262, 545);
                pdfContentByte.AddImage(image);
                image.SetAbsolutePosition(366, 545);
                pdfContentByte.AddImage(image);
                image.SetAbsolutePosition(470, 545);
                pdfContentByte.AddImage(image);
                stamper.Close();
            }
        }


        public async Task<IActionResult> PonitSheeplist(string SheepLish)//Staff_Home/PonitSheeplist
        {
            var datas = from c in _context.Orders
                        join o in _context.OrderDetails on c.訂單編號 equals o.訂單編號
                        join a in _context.ProductDetails on o.Id equals a.Id
                        join b in _context.ProductsSizeDetails on a.商品尺寸id equals b.商品尺寸id
                        join d in _context.ProductsColorDetails on a.商品顏色id equals d.商品顏色id
                        join e in _context.Products on a.商品編號id equals e.商品編號id
                        where c.訂單編號 == SheepLish
                        select new ViewModels.Cback_order_list
                        {
                            訂單編號 = c.訂單編號,
                            客戶編號 = c.客戶編號,
                            送貨地址 = c.送貨地址,
                            商品名稱 = e.商品名稱,
                            尺寸種類 = b.尺寸種類,
                            色碼 = d.色碼,
                            商品數量 = (int)o.商品數量
                        };
            var test = datas.ToList();

            string SheepList = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title></title>\r\n</head>\r\n<body>\r\n    <div style=\"width:80%;margin:200px auto; border: 2px;\">";
            SheepList += $"<h1 style=\"text-align: center;\">訂單編號:{test[0].訂單編號}</h1>\r\n        <h3>客戶編號:{test[0].客戶編號}</h3>\r\n        <h3>送貨地址:{test[0].送貨地址}</h3>\r\n        <hr />";
            SheepList += " <div id=\"Grid\" style=\"border-style:solid;\">\r\n\r\n            <table cellpadding=\"5\" cellspacing=\"0\" style=\"border: 1px solid #ccc;font-size: 9pt; width: 100%;\">\r\n\r\n                <tr>\r\n\r\n                    <th style=\"background-color: #B8DBFD;border: 1px solid #ccc\">商品名稱</th>\r\n\r\n                    <th style=\"background-color: #B8DBFD;border: 1px solid #ccc\">尺寸種類</th>\r\n\r\n                    <th style=\"background-color: #B8DBFD;border: 1px solid #ccc\">色碼</th>\r\n\r\n                    <th style=\"background-color: #B8DBFD;border: 1px solid #ccc\">商品數量</th>\r\n\r\n                </tr>\r\n";
            for (int i = 0; i < test.Count(); i++)
            {
                SheepList += $" <tr>\r\n                    <td style=\"width:120px; height: 20px; border: 1px solid #ccc\">{test[i].商品名稱}</td>\r\n                    <td style=\"width:120px; height: 20px;border: 1px solid #ccc\">{test[i].尺寸種類}</td>\r\n                    <td style=\"width:120px; height: 20px;border: 1px solid #ccc\">{test[i].色碼}</td>\r\n                   <td style=\"width:width:120px; height: 20px;border: 1px solid #ccc\">{test[i].商品數量}</td>\r\n                </tr>";
            }
            SheepList += "</table>\r\n        </div><br /><br />\r\n    </div>\r\n</body>\r\n</html>";


            string SheepListPDF = SheepList.ToString();



            string pdfName = $"貨單{test[0].訂單編號}.pdf";


            var properties = new ConverterProperties();
            properties.SetBaseUri(_environment.WebRootPath + "\\File\\");
            properties.SetCharset("utf-8");

            var provider = new DefaultFontProvider(true, true, true);//系統字體 中文
            properties.SetFontProvider(provider);

            //await using (MemoryStream stream = new MemoryStream())
            //{
            //    HtmlConverter.ConvertToPdf(SheepListPDF, stream, properties);
            //    File(stream.ToArray(), "application/pdf", pdfName);
            //    //byte[] bytes = stream.ToArray();
            //    //string byt = bytes.ToString();
            //    //return Content(byt);
            //    return Json(File(stream.ToArray(), "application/pdf", pdfName));
            //}
            await using (FileStream file1 = new FileStream(_environment.WebRootPath + "\\File\\" + pdfName, FileMode.Create))
            {
                HtmlConverter.ConvertToPdf(SheepListPDF, file1, properties);
                return Json(/*_environment.WebRootPath + "\\File\\" + */pdfName);
            }

        }
        public async Task<IActionResult> DeleteSheeplist(string SheepLish)
        {
            if (SheepLish != null)
            {
                string oldPath = _environment.WebRootPath + "/File/" + SheepLish;
                if (System.IO.File.Exists(oldPath))
                     System.IO.File.Delete(oldPath);
                return Json("ok");
            }
            return Json("no");
        }

    }
}
