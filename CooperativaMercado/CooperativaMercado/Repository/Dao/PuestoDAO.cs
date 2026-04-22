using Microsoft.Data.SqlClient;
using CooperativaMercado.Model;
using CooperativaMercado.Repository.Interfaces;
using System.Data;

namespace CooperativaMercado.Repository.Dao
{
    public class PuestoDao : IPuesto
    {
        private readonly string connectionString;

        public PuestoDao()
        {
            connectionString = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("dataBase")!;
        }

        // 🔹 LISTAR
        public List<Puesto> Listar()
        {
            List<Puesto> lista = new List<Puesto>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_ListarPuestos", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Puesto()
                    {
                        IdPuesto = dr.GetInt32(0),
                        Numero = dr.GetString(1),
                        Metraje = dr.IsDBNull(2) ? null : dr.GetDecimal(2),
                        Ubicacion =  dr.GetString(3),
                        Giro = dr.GetString(4),
                        MontoAlquiler = dr.GetDecimal(5),
                        IdSocio = dr.IsDBNull(6) ? null : dr.GetInt32(6),
                        Activo = dr.GetBoolean(7)
                    });
                }
            }

            return lista;
        }

        // 🔹 OBTENER POR ID
        public Puesto ObtenerPorId(int id)
        {
            Puesto puesto = null;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_ObtenerPuestoPorId", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPuesto", id);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    puesto = new Puesto()
                    {
                        IdPuesto = dr.GetInt32(0),
                        Numero = dr.GetString(1),
                        Metraje = dr.IsDBNull(2) ? null : dr.GetDecimal(2),
                        Ubicacion =  dr.GetString(3),
                        Giro = dr.GetString(4),
                        MontoAlquiler = dr.GetDecimal(5),
                        IdSocio = dr.IsDBNull(6) ? null : dr.GetInt32(6),
                        Activo = dr.GetBoolean(7)
                    };
                }
            }

            return puesto;
        }

        // 🔹 REGISTRAR
        public void Registrar(Puesto puesto)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_RegistrarPuesto", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Numero", puesto.Numero);
                cmd.Parameters.AddWithValue("@Metraje", (object?)puesto.Metraje ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ubicacion", (object?)puesto.Ubicacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Giro", (object?)puesto.Giro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MontoAlquiler", puesto.MontoAlquiler);
                cmd.Parameters.AddWithValue("@IdSocio", (object?)puesto.IdSocio ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        // 🔹 ACTUALIZAR
        public void Actualizar(Puesto puesto)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_ActualizarPuesto", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPuesto", puesto.IdPuesto);
                cmd.Parameters.AddWithValue("@Numero", puesto.Numero);
                cmd.Parameters.AddWithValue("@Metraje", (object?)puesto.Metraje ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ubicacion", (object?)puesto.Ubicacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Giro", (object?)puesto.Giro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MontoAlquiler", puesto.MontoAlquiler);
                cmd.Parameters.AddWithValue("@IdSocio", (object?)puesto.IdSocio ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        // 🔹 ASIGNAR SOCIO
        public void AsignarSocio(int idPuesto, int idSocio)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_AsociarPuesto", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPuesto", idPuesto);
                cmd.Parameters.AddWithValue("@IdSocio", idSocio);

                cmd.ExecuteNonQuery();
            }
        }

        // 🔹 DESASIGNAR SOCIO
        public void DesasignarSocio(int idPuesto)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_DesasociarPuesto", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPuesto", idPuesto);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
