using CooperativaMercado.Model;

namespace CooperativaMercado.Repository.Interfaces
{
    public interface ISocio
    {
        int RegistrarSocio(Socio socio);
        int ActualizarSocio(int idSocio, Socio socio);
        int DesactivarSocio(int idSocio);
    }
}
