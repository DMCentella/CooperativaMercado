namespace CooperativaMercado.Repository.Interfaces
{
    public interface IPago
    {
        int RegistrarPago(int idDeuda, decimal monto, string metodoPago);
    }
}
