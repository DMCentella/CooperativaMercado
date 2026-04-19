using Microsoft.AspNetCore.Mvc;

namespace CooperativaMercado.Controllers
{
    public class DeudasControllers : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
