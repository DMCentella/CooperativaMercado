using CooperativaMercado.Model;
using CooperativaMercado.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CooperativaMercado.Repository.Dao
{
    public class DeudaDAO : IDeuda
    {
        private readonly string connectionString;

        public DeudaDAO()
        {
            connectionString = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("database");
        }

        public int Actualizar(int idDeuda, Deuda deuda)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ActualizarDeuda", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdDeuda", idDeuda);
                cmd.Parameters.AddWithValue("@Descripcion", (object)deuda.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Monto", deuda.Monto);
                cmd.Parameters.AddWithValue("@FechaVencimiento", (object)deuda.FechaVencimiento ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", deuda.Estado);
                return cmd.ExecuteNonQuery();
            }
        }

        public int Crear(Deuda deuda)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_CrearDeuda", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPuesto", deuda.IdPuesto);
                cmd.Parameters.AddWithValue("@IdTipoDeuda", deuda.IdTipoDeuda);
                cmd.Parameters.AddWithValue("@Descripcion", (object)deuda.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Monto", deuda.Monto);
                cmd.Parameters.AddWithValue("@Mes", deuda.Mes);
                cmd.Parameters.AddWithValue("@Anio", deuda.Anio);
                cmd.Parameters.AddWithValue("@FechaVencimiento", (object)deuda.FechaVencimiento ?? DBNull.Value);
                return cmd.ExecuteNonQuery();
            }
        }

        public int GenerarAlquilerMensual(int mes, int anio)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_GenerarAlquilerMensual", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Mes", mes);
                cmd.Parameters.AddWithValue("@Anio", anio);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
