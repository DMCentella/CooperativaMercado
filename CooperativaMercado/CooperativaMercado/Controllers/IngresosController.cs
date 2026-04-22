using CooperativaMercado.Model;
using CooperativaMercado.Repository.Dao;
using Microsoft.AspNetCore.Mvc;

namespace CooperativaMercado.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngresosController : ControllerBase
    {
        private readonly IngresoDao _ingresoDao;

        public IngresosController(IngresoDao ingresoDao)
        {
            _ingresoDao = ingresoDao;
        }

        [HttpPost("registrar")]
        public ActionResult registrar(IngresoDiario ingreso)
        {
            _ingresoDao.RegistrarIngreso(ingreso);
            return Ok();
        }

        [HttpGet("getIngresos")]
        public ActionResult getIngresos()
        {
            return Ok(_ingresoDao.Listar());
        }


        [HttpGet("reporteIngresos")]
        public ActionResult reporteIngresos(DateTime inicio, DateTime fin)
        {
            if (inicio > fin)
                return BadRequest(new { mensaje = "La fecha inicio no puede ser mayor que la fecha fin" });

            return Ok(new
            {
                inicio,
                fin,
                total = _ingresoDao.TotalIngresosPorRango(inicio, fin)
            });
        }

        [HttpGet("reporteIngresosPuesto")]
        public ActionResult reporteIngresosPuesto(DateTime inicio, DateTime fin)
        {
            if (inicio > fin)
                return BadRequest(new { mensaje = "Rango de fechas inválido" });

            return Ok(_ingresoDao.ReporteIngresosPorPuesto(inicio, fin));
        }
    }
}
