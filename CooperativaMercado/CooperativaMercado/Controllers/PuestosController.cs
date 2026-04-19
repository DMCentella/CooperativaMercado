using Microsoft.AspNetCore.Mvc;

namespace CooperativaMercado.Controllers
{
    public class PuestosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
