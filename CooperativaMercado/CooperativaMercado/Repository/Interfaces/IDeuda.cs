using CooperativaMercado.Model;

namespace CooperativaMercado.Repository.Interfaces
{
    public interface IDeuda
    {
        int Crear(Deuda deuda);
        int Actualizar(int idDeuda, Deuda deuda);
        int GenerarAlquilerMensual(int mes, int anio);
    }
}
