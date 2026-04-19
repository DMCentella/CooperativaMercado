using Microsoft.Data.SqlClient;
using CooperativaMercado.Model;
using CooperativaMercado.Repository.Interfaces;
using System.Data;

namespace CooperativaMercado.Repository.Dao
{
    public class PuestoDAO : IPuesto
    {
        // no se olviden de cambiar su cadena de conexion
        // aunque ahora no es nesesario :D
        private readonly string connectionString;

        public PuestoDAO()
        {
            connectionString = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("database");
        }

        public int Actualizar(int id, Puesto puesto)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ActualizarPuesto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPuesto", id);
                cmd.Parameters.AddWithValue("@Numero", puesto.Numero);
                cmd.Parameters.AddWithValue("@Metraje", (object)puesto.Metraje ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ubicacion", (object)puesto.Ubicacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Giro", (object)puesto.Giro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MontoAlquiler", puesto.MontoAlquiler);
                cmd.Parameters.AddWithValue("@IdSocio", (object)puesto.IdSocio ?? DBNull.Value);
                return cmd.ExecuteNonQuery();
            }
        }

        public int Asociar(int idPuesto, int idSocio)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            { 
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_AsociarPuesto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPuesto", idPuesto);
                cmd.Parameters.AddWithValue("@IdSocio", idSocio);
                return cmd.ExecuteNonQuery();
            }
        }

        public int Desactivar(int idPuesto)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_DesasociarPuesto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPuesto", idPuesto);
                return cmd.ExecuteNonQuery();
            }
        }

        public int Desasociar(int idPuesto)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_DesacativarPuesto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPuesto", idPuesto);
                return cmd.ExecuteNonQuery();
            }
        }

        public int Registrar(Puesto puesto)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_RegistrarPuesto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Numero", puesto.Numero);
                cmd.Parameters.AddWithValue("@Metraje", (object)puesto.Metraje ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ubicacion", (object)puesto.Ubicacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Giro", (object)puesto.Giro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MontoAlquiler", puesto.MontoAlquiler);
                cmd.Parameters.AddWithValue("@IdSocio", (object)puesto.IdSocio ?? DBNull.Value);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
