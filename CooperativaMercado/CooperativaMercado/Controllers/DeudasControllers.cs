using CooperativaMercado.Model;
using CooperativaMercado.Repository.Dao;
using Microsoft.AspNetCore.Mvc;

namespace CooperativaMercado.Controllers
{
    [ApiController]
    [Route("api/deudas")]
    public class DeudasControllers : ControllerBase
    {
        private readonly DeudaDAO deudaDAO;

        public DeudasControllers(DeudaDAO deudaDAO) 
        {
            this.deudaDAO = deudaDAO;
        }

        [HttpPost]
        public ActionResult Create([FromBody] Deuda deuda)
        {
            if (deuda == null)
                return BadRequest();

            var resultado = deudaDAO.Crear(deuda);
            if (resultado > 0)
                return Created("", deuda);

            return BadRequest();
        }

        [HttpPut]
        public ActionResult Actualizar(int idDeuda, Deuda deuda)
        {
            if (deuda == null)
                return BadRequest();

            var resultado = deudaDAO.Actualizar(idDeuda, deuda);
            if (resultado == 0)
                return NotFound();

            return NoContent();
        }

        [HttpPost("generar-alquiler")] // nose si este esta bien revisen
        public ActionResult GenerarAlquilerMensual(int mes, int anio)
        {
            if (mes < 1 || mes > 12)
                return BadRequest(new { mensaje = "Mes invalido" });

            deudaDAO.GenerarAlquilerMensual(mes, anio);
            return Ok(new { mensaje = "Alquileres generados correctamente" });
        }

    }
}
