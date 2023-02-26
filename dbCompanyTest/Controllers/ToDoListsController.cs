using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dbCompanyTest.Models;
using dbCompanyTest.ViewModels;
using NPOI.HPSF;
using System.Reflection;
using System.Drawing.Imaging;
using System.Drawing;
using iText.Html2pdf;
//
using iText.Html2pdf.Resolver.Font;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace dbCompanyTest.Controllers
{
    public class ToDoListsController : Controller
    {
        dbCompanyTestContext _context = new dbCompanyTestContext();

        private IWebHostEnvironment _environment;
        public ToDoListsController(IWebHostEnvironment p)
        {
            _environment = p;
        }

        //private readonly dbCompanyTestContext _context;

        //public ToDoListsController(dbCompanyTestContext context)
        //{
        //    _context = context;
        //}

        // GET: ToDoLists
        //public async Task<IActionResult> Index()
        //{
        //    var dbCompanyTestContext = _context.ToDoLists.Include(t => t.員工編號Navigation);
        //    return View(await dbCompanyTestContext.ToListAsync());
        //}

        // GET: ToDoLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ToDoLists == null)
            {
                return NotFound();
            }

            var toDoList = await _context.ToDoLists
                .Include(t => t.員工編號Navigation)
                .FirstOrDefaultAsync(m => m.交辦事項id == id);
            if (toDoList == null)
            {
                return NotFound();
            }

            return View(toDoList);
        }

        // GET: ToDoLists/Create
        public IActionResult Create()
        {
            //ViewData["員工編號"] = new SelectList(_context.TestStaffs, "員工編號", "員工編號");
            string stfNum = HttpContext.Session.GetString(CDittionary.SK_STAFF_NUMBER_SESSION);
            var stf = _context.TestStaffs.FirstOrDefault(c => c.員工編號 == stfNum);

            ViewBag.acc = $"{stf.員工姓名} {stfNum} {stf.部門}";
            ViewBag.dep = stf.部門;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CToDoListViewModels cToDoListViewModels)
        //public async Task<IActionResult> Create(CToDoListViewModels cToDoListViewModels)
        /*[Bind("交辦事項id,員工編號,表單類型,表單內容,回覆,表單狀態,起單時間,起單人,部門主管,部門主管簽核,部門主管簽核意見,部門主管簽核時間,協辦部門,協辦部門簽核,協辦部門簽核人員,協辦部門簽核意見,協辦部門簽核時間,老闆簽核,老闆簽核意見,老闆簽核時間,執行人,執行時間,執行人簽核,附件,附件path")] ToDoList toDoList*/
        {
            var datas = (from c in _context.ToDoLists select c).OrderByDescending(d => d.交辦事項id).FirstOrDefault().交辦事項id;
            var count = Convert.ToInt32(datas) + 1;
            if (cToDoListViewModels.表單類型 == "人事表單")
            {
                Regex regex = null;


                var htmlTemplate = System.IO.File.ReadAllText(_environment.WebRootPath + "/File/htmlTemplate.html");
                var start = "<!--template-start-->";
                var end = "<!--template-end-->";

                var match = Regex.Match(htmlTemplate, $@"{start}(.|\s)+?{end}");
                //if (match != null && match.Value.Length > start.Length + end.Length)
                //{
                var template = match.Value.Substring(start.Length, match.Value.Length - start.Length - end.Length);
                //var sb = new StringBuilder(start);


                Dictionary<String, String> dic = new Dictionary<String, String> { ["{{Avatar}}"] = $"{_environment.WebRootPath}/Sign/{cToDoListViewModels.起單人}.jpg" };
                regex = new Regex(String.Join("|", dic.Keys), RegexOptions.IgnoreCase);
                var html = regex.Replace(template, m => dic[m.Value]);

                dic = new Dictionary<String, String> { ["{{Content}}"] = $"{cToDoListViewModels.表單內容}" };
                regex = new Regex(String.Join("|", dic.Keys), RegexOptions.IgnoreCase);
                html = regex.Replace(html, m => dic[m.Value]);

                dic = new Dictionary<String, String> { ["{{id}}"] = $"{count}" };
                regex = new Regex(String.Join("|", dic.Keys), RegexOptions.IgnoreCase);
                html = regex.Replace(html, m => dic[m.Value]);
                //}


                string pdfName = $"{Guid.NewGuid().ToString()}.pdf";
                cToDoListViewModels.附件 = pdfName;
              

                Task subThread1 = new Task(async () =>
                {
                    var properties = new ConverterProperties();
                    properties.SetBaseUri(_environment.WebRootPath + "\\File\\");
                    properties.SetCharset("utf-8");

                    var provider = new DefaultFontProvider(true, true, true);//系統字體 中文
                    properties.SetFontProvider(provider);

                    await using (FileStream file1 = new FileStream(_environment.WebRootPath + "\\File\\" + pdfName, FileMode.Create))
                    {
                        HtmlConverter.ConvertToPdf(html, file1, properties);
                    }
                });
                subThread1.Start();

            }

            if (ModelState.IsValid)
            {
                if (cToDoListViewModels.File != null)
                {
                    string FileNameSub = cToDoListViewModels.File.FileName;
                    string[] words = FileNameSub.Split('.');
                    int FileTypeIndex = words.Length;
                    string FileType = words[FileTypeIndex - 1];

                    string FileName = $"{Guid.NewGuid().ToString()}.{FileType}";
                    string path = _environment.WebRootPath + "\\File\\" + FileName;
                    cToDoListViewModels.附件path = FileName;

                    using (FileStream file = new FileStream(path, FileMode.Create))
                        cToDoListViewModels.File.CopyTo(file);
                }
                _context.Add(cToDoListViewModels.toDoList);
                /*await*/
                _context.SaveChangesAsync();

                return RedirectToAction("Index", "Staff_Home");
            }
            ViewData["員工編號"] = new SelectList(_context.TestStaffs, "員工編號", "員工編號", cToDoListViewModels.員工編號);
            return View(cToDoListViewModels);
        }

        // GET: ToDoLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ToDoLists == null)
            {
                return NotFound();
            }

            var toDoList = await _context.ToDoLists.FindAsync(id);
            if (toDoList == null)
            {
                return NotFound();
            }
            ViewData["員工編號"] = new SelectList(_context.TestStaffs, "員工編號", "員工編號", toDoList.員工編號);
            return View(toDoList);
        }

        // POST: ToDoLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("交辦事項id,員工編號,表單類型,表單內容,回覆,表單狀態,起單時間,起單人,部門主管,部門主管簽核,部門主管簽核意見,部門主管簽核時間,協辦部門,協辦部門簽核,協辦部門簽核人員,協辦部門簽核意見,協辦部門簽核時間,老闆簽核,老闆簽核意見,老闆簽核時間,執行人,執行時間,執行人簽核,附件,附件path")] ToDoList toDoList)
        {
            if (id != toDoList.交辦事項id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toDoList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoListExists(toDoList.交辦事項id))
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
            ViewData["員工編號"] = new SelectList(_context.TestStaffs, "員工編號", "員工編號", toDoList.員工編號);
            return View(toDoList);
        }

        // GET: ToDoLists/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.ToDoLists == null)
        //    {
        //        return NotFound();
        //    }

        //    var toDoList = await _context.ToDoLists
        //        .Include(t => t.員工編號Navigation)
        //        .FirstOrDefaultAsync(m => m.交辦事項id == id);
        //    if (toDoList == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(toDoList);
        //}

        // POST: ToDoLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ToDoLists == null)
            {
                return Problem("Entity set 'dbCompanyTestContext.ToDoLists'  is null.");
            }
            var toDoList = await _context.ToDoLists.FindAsync(id);
            if (toDoList != null)
            {
                _context.ToDoLists.Remove(toDoList);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoListExists(int id)
        {
            return _context.ToDoLists.Any(e => e.交辦事項id == id);
        }

        public string pdf(string id, string conten)
        {
            string html = "<div style=\"width:80%;margin:200px auto; border: 2px;\">" +
                $"<h1 style=\"text-align: center;\">表單{id}</h1>" +
                "<h3>表單內容</h3>" +
                $"<p>{conten}</p>" +
                "<hr/>" +
                " <div id=\"Grid\" style=\"border-style:solid;\">" +
                "<table cellpadding=\"5\" cellspacing=\"0\" style=\"border: 1px solid #ccc;font-size: 9pt; width: 100%;\">" +
                " <tr >" +
                "<th style=\"background-color: #B8DBFD;border: 1px solid #ccc\">起單人</th>" +
                " <th style=\"background-color: #B8DBFD;border: 1px solid #ccc\">部門主管</th>" +
                "<th style=\"background-color: #B8DBFD;border: 1px solid #ccc\">協辦部門</th>" +
                "<th style=\"background-color: #B8DBFD;border: 1px solid #ccc\">老闆</th>" +
                "<th style=\"background-color: #B8DBFD;border: 1px solid #ccc\">執行</th>" +
                "</tr>" +
                "<tr >" +
                " <th style=\"background-color: #B8DBFD;border: 1px solid #ccc\"></th>" +
                "<th style=\"background-color: #B8DBFD;border: 1px solid #ccc\"></th>" +
                "<th style=\"background-color: #B8DBFD;border: 1px solid #ccc\"></th>" +
                "<th style=\"background-color: #B8DBFD;border: 1px solid #ccc\"></th>" +
                "<th style=\"background-color: #B8DBFD;border: 1px solid #ccc\"></th>" +
                "</tr>" +
                "<tr >" +
                "<td style=\"width:120px; height: 150px; border: 1px solid #ccc\"></td>" +
                "<td style=\"width:120px; height: 150px;border: 1px solid #ccc\"></td>" +
                "<td style=\"width:120px; height: 150px;border: 1px solid #ccc\"></td>" +
                " <td style=\"width:120px; height: 150px;border: 1px solid #ccc\"></td>" +
                "<td style=\"width:120px; height: 150px;border: 1px solid #ccc\"></td>" +
                "</tr>" +
                "</table>" +
                "</div>" +
                "<br />" +
                "<br />" +
                "</div>";

            return html;
        }
    }
}
