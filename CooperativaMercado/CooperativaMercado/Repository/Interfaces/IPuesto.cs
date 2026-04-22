using CooperativaMercado.Model;

namespace CooperativaMercado.Repository.Interfaces
{
    public interface IPuesto
    {
        List<Puesto> Listar();
        Puesto ObtenerPorId(int id);

        void Registrar(Puesto puesto);
        void Actualizar(Puesto puesto);

        void AsignarSocio(int idPuesto, int idSocio);
        void DesasignarSocio(int idPuesto);
    }
}
