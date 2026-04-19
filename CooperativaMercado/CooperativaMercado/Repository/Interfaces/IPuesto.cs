using CooperativaMercado.Model;

namespace CooperativaMercado.Repository.Interfaces
{
    public interface IPuesto
    {
        int Registrar(Puesto puesto);
        int Actualizar(int id, Puesto puesto);
        int Asociar(int idPuesto, int idSocio);
        int Desasociar(int idPuesto);
        int Desactivar(int idPuesto);
    }
}
