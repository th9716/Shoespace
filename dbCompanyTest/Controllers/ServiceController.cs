using Microsoft.AspNetCore.Mvc;
using dbCompanyTest.Models;
using dbCompanyTest.Hubs;

namespace dbCompanyTest.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult Service()
        {
            if (HttpContext.Session.Keys.Contains(CDittionary.SK_STAFF_NUMBER_SESSION))
                return View();
            else
                return RedirectToAction("login", "Staff_Home");
        }
    }
}
