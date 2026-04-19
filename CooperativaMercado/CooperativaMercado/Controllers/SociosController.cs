using Microsoft.AspNetCore.Mvc;

namespace CooperativaMercado.Controllers
{
    public class SociosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
