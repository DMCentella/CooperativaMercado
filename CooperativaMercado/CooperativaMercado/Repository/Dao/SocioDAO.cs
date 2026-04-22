using CooperativaMercado.Model;
using CooperativaMercado.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CooperativaMercado.Repository.Dao
{
    public class SocioDao : ISocio
    {
        private readonly string connectionString;

        public SocioDao()
        {
            connectionString = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("dataBase")!;
        }

     
        public List<Socio> Listar()
        {
            List<Socio> lista = new List<Socio>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_ListarSocios", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Socio()
                    {
                        IdSocio = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        DNI = dr.GetString(2),
                        Telefono =  dr.GetString(3),
                        Activo = dr.GetBoolean(4)
                    });
                }
            }

            return lista;
        }

        public Socio ObtenerPorId(int id)
        {
            Socio socio = null;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_ObtenerSocioPorId", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSocio", id);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    socio = new Socio()
                    {
                        IdSocio = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        DNI =  dr.GetString(2),
                        Telefono =  dr.GetString(3),
                        Activo = dr.GetBoolean(4)
                    };
                }
            }

            return socio;
        }


        public void Registrar(Socio socio)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_RegistrarSocio", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", socio.Nombre);
                cmd.Parameters.AddWithValue("@DNI", (object?)socio.DNI ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", (object?)socio.Telefono ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        // 🔹 ACTUALIZAR
        public void Actualizar(Socio socio)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_ActualizarSocio", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdSocio", socio.IdSocio);
                cmd.Parameters.AddWithValue("@Nombre", socio.Nombre);
                cmd.Parameters.AddWithValue("@DNI", (object?)socio.DNI ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", (object?)socio.Telefono ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
