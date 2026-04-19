using CooperativaMercado.Model;
using CooperativaMercado.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CooperativaMercado.Repository.Dao
{
    public class IngresoDAO : IIngreso
    {
        private readonly string connectionString;

        public IngresoDAO()
        {
            connectionString = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("database");
        }

        public int RegistrarIngreso(IngresoDiario ingresoDiario)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_RegistrarIngreso", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPuesto", ingresoDiario.IdPuesto);
                cmd.Parameters.AddWithValue("@Fecha", ingresoDiario.Fecha);
                cmd.Parameters.AddWithValue("@Monto", ingresoDiario.Monto);
                cmd.Parameters.AddWithValue("@IdUsuario", ingresoDiario.IdUsuario);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
