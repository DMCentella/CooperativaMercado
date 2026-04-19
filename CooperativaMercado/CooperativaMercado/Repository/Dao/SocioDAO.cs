using CooperativaMercado.Model;
using CooperativaMercado.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CooperativaMercado.Repository.Dao
{
    public class SocioDAO : ISocio
    {
        private readonly string connectionString;

        public SocioDAO()
        {
            connectionString = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("database");
        }

        public int ActualizarSocio(int idSocio, Socio socio)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ActualizarSocio", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSocio", idSocio);
                cmd.Parameters.AddWithValue("@Nombre", socio.Nombre);
                cmd.Parameters.AddWithValue("@DNI", (object)socio.DNI ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", (object)socio.Telefono ?? DBNull.Value);
                return cmd.ExecuteNonQuery();
            }
        }

        public int DesactivarSocio(int idSocio)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_DesactivarSocio", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSocio", idSocio);
                return cmd.ExecuteNonQuery();
            }
        }

        public int RegistrarSocio(Socio socio)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_RegistrarSocio", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", socio.Nombre);
                cmd.Parameters.AddWithValue("@DNI", (object)socio.DNI ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", (object)socio.Telefono ?? DBNull.Value);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
