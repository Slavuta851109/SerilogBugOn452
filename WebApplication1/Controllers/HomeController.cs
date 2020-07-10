using System;
using System.Web.Mvc;
using LoggingAdapter.Logs.DTOs;
using LoggingAdapter.Logs.Repositories;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            var request = new LogRequest("Info", "my source", "my message",  "my method", null, Environment.MachineName, null, null, DateTime.UtcNow);
            new SerilogAdapter().Log(request);

            return View();
        }
    }
}
