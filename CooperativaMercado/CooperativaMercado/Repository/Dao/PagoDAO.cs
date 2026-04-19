using CooperativaMercado.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CooperativaMercado.Repository.Dao
{
    public class PagoDAO : IPago
    {
        private readonly string connectionString;

        public PagoDAO()
        {
            connectionString = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("database");
        }

        public int RegistrarPago(int idDeuda, decimal monto, string metodoPago)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_RegistrarPago", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdDeuda", idDeuda);
                cmd.Parameters.AddWithValue("@Monto", monto);
                cmd.Parameters.AddWithValue("@MetodoPago", (object)metodoPago ?? DBNull.Value);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
