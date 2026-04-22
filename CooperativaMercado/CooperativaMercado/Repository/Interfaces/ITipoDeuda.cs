using CooperativaMercado.Model;

namespace CooperativaMercado.Repository.Interfaces
{
    public interface ITipoDeuda
    {

        List<TipoDeuda> Listar();
        TipoDeuda ObtenerPorId(int id);

        void Registrar(TipoDeuda tipo);
        void Actualizar(TipoDeuda tipo);

        void Desactivar(int id);
    }
}
