using CooperativaMercado.Model;
using CooperativaMercado.Repository.Dao;
using Microsoft.AspNetCore.Mvc;

namespace CooperativaMercado.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeudasController : ControllerBase
    {
        private readonly DeudaDao _deudaDao;

        public DeudasController(DeudaDao deudaDao)
        {
            _deudaDao = deudaDao;
        }

        [HttpGet("getDeudas")]
        public ActionResult getDeudas()
        {
            return Ok(_deudaDao.Listar());
        }

        [HttpGet("pendientes")]
        public ActionResult getPendientes()
        {
            return Ok(_deudaDao.ObtenerPendientes());
        }

        [HttpPost("saveDeuda")]
        public ActionResult saveDeuda(Deuda deuda)
        {
            _deudaDao.Registrar(deuda);
            return Created("", deuda);
        }

        [HttpPost("generarAlquiler")]
        public ActionResult generar(int mes, int anio)
        {
            _deudaDao.GenerarAlquilerMensual(mes, anio);
            return Ok();
        }
        [HttpGet("reporteDeudasPendientes")]
        public ActionResult reporteDeudasPendientes()
        {
            return Ok(_deudaDao.ReportePendientes());
        }

        [HttpGet("reporteDeudasPagadas")]
        public ActionResult reporteDeudasPagadas()
        {
            return Ok(_deudaDao.ReportePagadas());
        }


    }


}
