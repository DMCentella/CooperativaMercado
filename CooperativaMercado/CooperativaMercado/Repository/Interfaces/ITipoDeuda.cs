using CooperativaMercado.Model;

namespace CooperativaMercado.Repository.Interfaces
{
    public interface ITipoDeuda
    {
        int Registrar(TipoDeuda tipo);
        int Actualizar(int idTipoDeuda, TipoDeuda tipo);
        int Desactivar(int idTipoDeuda);
    }
}
