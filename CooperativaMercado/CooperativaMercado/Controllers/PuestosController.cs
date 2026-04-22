using CooperativaMercado.Model;
using CooperativaMercado.Repository.Dao;
using Microsoft.AspNetCore.Mvc;

namespace CooperativaMercado.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PuestosController : ControllerBase
    {
        private readonly PuestoDao _puestoDao;

        public PuestosController(PuestoDao puestoDao)
        {
            _puestoDao = puestoDao;
        }

        [HttpGet("getPuestos")]
        public ActionResult getPuestos()
        {
            return Ok(_puestoDao.Listar());
        }

        [HttpPost("savePuesto")]
        public ActionResult savePuesto(Puesto puesto)
        {
            _puestoDao.Registrar(puesto);
            return Created("", puesto);
        }

        [HttpPut("updatePuesto")]
        public ActionResult updatePuesto(Puesto puesto)
        {
            _puestoDao.Actualizar(puesto);
            return Ok(puesto);
        }

        [HttpPut("asignar")]
        public ActionResult asignar(int idPuesto, int idSocio)
        {
            _puestoDao.AsignarSocio(idPuesto, idSocio);
            return Ok();
        }

        [HttpPut("desasignar")]
        public ActionResult desasignar(int idPuesto)
        {
            _puestoDao.DesasignarSocio(idPuesto);
            return Ok();
        }
    }
}
