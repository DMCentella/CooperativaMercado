using CooperativaMercado.Model;

namespace CooperativaMercado.Repository.Interfaces
{
    public interface IIngreso
    {
        void RegistrarIngreso(IngresoDiario ingreso);

        List<IngresoDiario> Listar();
        List<IngresoDiario> ObtenerPorPuesto(int idPuesto);


    }
}

