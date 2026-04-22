using CooperativaMercado.Model;
using CooperativaMercado.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CooperativaMercado.Repository.Dao
{
    public class DeudaDao : IDeuda
    {
        private readonly string connectionString;

        public DeudaDao()
        {
            connectionString = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("dataBase")!;
        }

        // 🔹 LISTAR
        public List<Deuda> Listar()
        {
            List<Deuda> lista = new List<Deuda>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_ListarDeudas", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(MapearDeuda(dr));
                }
            }

            return lista;
        }

        // 🔹 OBTENER POR ID
        public Deuda? ObtenerPorId(int? id)
        {
            Deuda? deuda = null;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_ObtenerDeudaPorId", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdDeuda", id);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    deuda = MapearDeuda(dr);
                }
            }

            return deuda;
        }

        // 🔹 DEUDAS POR PUESTO
        public List<Deuda> ObtenerPorPuesto(int idPuesto)
        {
            List<Deuda> lista = new List<Deuda>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_ObtenerDeudasPorPuesto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPuesto", idPuesto);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(MapearDeuda(dr));
                }
            }

            return lista;
        }

        // 🔹 DEUDAS PENDIENTES
        public List<Deuda> ObtenerPendientes()
        {
            List<Deuda> lista = new List<Deuda>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_DeudasPendientes", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(MapearDeuda(dr));
                }
            }

            return lista;
        }

        // 🔹 REGISTRAR DEUDA
        public void Registrar(Deuda deuda)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_CrearDeuda", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPuesto", deuda.IdPuesto);
                cmd.Parameters.AddWithValue("@IdTipoDeuda", deuda.IdTipoDeuda);
                cmd.Parameters.AddWithValue("@Descripcion", (object?)deuda.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Monto", deuda.Monto);
                cmd.Parameters.AddWithValue("@Mes", deuda.Mes);
                cmd.Parameters.AddWithValue("@Anio", deuda.Anio);
                cmd.Parameters.AddWithValue("@FechaVencimiento", (object?)deuda.FechaVencimiento ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        // 🔹 GENERAR ALQUILER MASIVO
        public void GenerarAlquilerMensual(int mes, int anio)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_GenerarAlquilerMensual", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Mes", mes);
                cmd.Parameters.AddWithValue("@Anio", anio);

                cmd.ExecuteNonQuery();
            }
        }

        // 🔹 MÉTODO PRIVADO PARA MAPEAR
        private Deuda MapearDeuda(SqlDataReader dr)
        {
            return new Deuda()
            {
                IdDeuda = dr.GetInt32(0),
                IdPuesto = dr.GetInt32(1),
                IdTipoDeuda = dr.GetInt32(2),
                Descripcion = dr.GetString(3),
                Monto = dr.GetDecimal(4),
                Mes = dr.GetInt32(5),
                Anio = dr.GetInt32(6),
                FechaVencimiento = dr.IsDBNull(7) ? null : dr.GetDateTime(7),
                Estado = dr.GetString(8)
            };
        }

        public Deuda ObtenerPorId(int id)
        {
            throw new NotImplementedException();
        }

        public List<Deuda> ReportePendientes()
        {
            List<Deuda> lista = new List<Deuda>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_ReporteDeudasPendientes", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Deuda()
                    {
                        IdDeuda = dr.GetInt32(0),

                        // 🔥 OJO: estos vienen del JOIN
                        // no todos existen en tu modelo tal cual
                        Descripcion = dr.GetString(1), // Puesto
                        Estado = dr.GetString(6),

                        Monto = dr.GetDecimal(3),
                        Mes = dr.GetInt32(4),
                        Anio = dr.GetInt32(5)
                    });
                }
            }

            return lista;
        }

        public List<ReporteDeuda> ReportePagadas()
        {
            List<ReporteDeuda> lista = new List<ReporteDeuda>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_ReporteDeudasPagadas", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new ReporteDeuda()
                    {
                        IdDeuda = dr.GetInt32(0),
                        Puesto = dr.GetString(1),
                        Socio = dr.IsDBNull(2) ? null : dr.GetString(2),
                        Monto = dr.GetDecimal(3),
                        Mes = dr.GetInt32(4),
                        Anio = dr.GetInt32(5),
                        Estado = dr.GetString(6)
                    });
                }
            }

            return lista;
        }
    }
}
