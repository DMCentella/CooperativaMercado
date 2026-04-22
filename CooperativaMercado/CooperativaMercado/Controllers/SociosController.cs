using CooperativaMercado.Model;
using CooperativaMercado.Repository.Dao;
using Microsoft.AspNetCore.Mvc;

namespace CooperativaMercado.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SociosController : ControllerBase
    {
        private readonly SocioDao _socioDao;

        public SociosController(SocioDao socioDao)
        {
            _socioDao = socioDao;
        }

        [HttpGet("getSocios")]
        public ActionResult getSocios()
        {
            return Ok(_socioDao.Listar());
        }

        [HttpGet("getSocio/{id}")]
        public ActionResult getSocio(int id)
        {
            var socio = _socioDao.ObtenerPorId(id);

            if (socio == null)
                return NotFound();

            return Ok(socio);
        }

        [HttpPost("saveSocio")]
        public ActionResult saveSocio(Socio socio)
        {
            _socioDao.Registrar(socio);
            return Created("", socio);
        }

        [HttpPut("updateSocio")]
        public ActionResult updateSocio(Socio socio)
        {
            _socioDao.Actualizar(socio);
            return Ok(socio);
        }
    }
}
