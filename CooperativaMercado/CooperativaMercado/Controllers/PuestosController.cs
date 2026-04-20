using CooperativaMercado.Model;
using CooperativaMercado.Repository.Dao;
using Microsoft.AspNetCore.Mvc;

namespace CooperativaMercado.Controllers
{
    [ApiController]
    [Route("api/puestos")]
    public class PuestosController : ControllerBase
    {
        private readonly PuestoDAO puestoDAO;

        public PuestosController(PuestoDAO puestoDAO)
        {
            this.puestoDAO = puestoDAO;
        }

        [HttpPost]
        public ActionResult Registrar([FromBody] Puesto puesto)
        {
            if (puesto == null)
                return BadRequest();

            var resultado = puestoDAO.Registrar(puesto);
            if (resultado > 0)
                return Created("", puesto);

            return BadRequest();
        }

        [HttpPut("{id:int}")]
        public ActionResult Actualizar(int id, [FromBody] Puesto puesto)
        {
            var resultado = puestoDAO.Actualizar(id, puesto);
            if (resultado == 0)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{idPuesto:int}/asociar/{idSocio:int}")]
        public ActionResult Asociar(int idPuesto, int idSocio)
        {
            var resultado = puestoDAO.Asociar(idPuesto, idSocio);
            if (resultado == 0)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{idPuesto:int}/desasociar")]
        public ActionResult Desasociar(int idPuesto)
        {
            var resultado = puestoDAO.Desasociar(idPuesto);
            if (resultado == 0)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{idPuesto:int}")]
        public ActionResult Desactivar(int idPuesto)
        {
            var resultado = puestoDAO.Desactivar(idPuesto);
            if (resultado == 0)
                return NotFound();

            return NoContent();
        }

    }
}
