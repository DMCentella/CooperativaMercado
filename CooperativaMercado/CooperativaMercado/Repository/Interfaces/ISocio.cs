using CooperativaMercado.Model;

namespace CooperativaMercado.Repository.Interfaces
{
    public interface ISocio
    {
        List<Socio> Listar();
        Socio ObtenerPorId(int id);
        void Registrar(Socio socio);
        void Actualizar(Socio socio);
    }
}
