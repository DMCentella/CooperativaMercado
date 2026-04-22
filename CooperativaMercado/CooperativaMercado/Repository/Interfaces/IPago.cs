using CooperativaMercado.Model;

namespace CooperativaMercado.Repository.Interfaces
{
    public interface IPago
    {
        void RegistrarPago(int idDeuda, decimal monto, string metodoPago);

        List<Pago> ListarPagos();
        List<Pago> ObtenerPorPuesto(int idPuesto);
    }
}
