using CooperativaMercado.Model;
using CooperativaMercado.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CooperativaMercado.Repository.Dao
{
    public class TipoDeudaDAO : ITipoDeuda
    {
        private readonly string connectionString;

        public TipoDeudaDAO()
        {
            connectionString = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("database");
        }

        public int Actualizar(int idTipoDeuda, TipoDeuda tipo)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ActualizarTipoDeuda", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoDeuda", idTipoDeuda);
                cmd.Parameters.AddWithValue("@Nombre", tipo.Nombre);
                cmd.Parameters.AddWithValue("@MontoBase", (object)tipo.MontoBase ?? DBNull.Value);
                return cmd.ExecuteNonQuery();
            }
        }

        public int Desactivar(int idTipoDeuda)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_DesactivarTipoDeuda", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoDeuda", idTipoDeuda);
                return cmd.ExecuteNonQuery();
            }
        }

        public int Registrar(TipoDeuda tipo)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_RegistrarTipoDeuda", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", tipo.Nombre);
                cmd.Parameters.AddWithValue("@MontoBase", (object)tipo.MontoBase ?? DBNull.Value);
                return cmd.ExecuteNonQuery();
            }
    }
}
