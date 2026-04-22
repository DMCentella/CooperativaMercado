using CooperativaMercado.Repository.Dao;
using Microsoft.AspNetCore.Mvc;

namespace CooperativaMercado.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagosController : ControllerBase
    {
        private readonly PagoDao _pagoDao;

        public PagosController(PagoDao pagoDao)
        {
            _pagoDao = pagoDao;
        }

        [HttpPost("pagar")]
        public ActionResult pagar(int idDeuda, decimal monto, string metodo)
        {
            _pagoDao.RegistrarPago(idDeuda, monto, metodo);
            return Ok();
        }

        [HttpGet("getPagos")]
        public ActionResult getPagos()
        {
            return Ok(_pagoDao.ListarPagos());
        }


        [HttpGet("reporteRecaudado")]
        public ActionResult reporteRecaudado(DateTime inicio, DateTime fin)
        {
            if (inicio > fin)
                return BadRequest(new { mensaje = "La fecha inicio no puede ser mayor que la fecha fin" });

            return Ok(new
            {
                inicio,
                fin,
                total = _pagoDao.TotalRecaudadoPorRango(inicio, fin)
            });
        }
    }
}
