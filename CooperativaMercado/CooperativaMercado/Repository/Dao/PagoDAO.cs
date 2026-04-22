using CooperativaMercado.Model;
using CooperativaMercado.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CooperativaMercado.Repository.Dao
{
    public class PagoDao : IPago
    {
        private readonly string connectionString;

        public PagoDao()
        {
            connectionString = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("dataBase")!;
        }

        // 🔹 REGISTRAR PAGO
        public void RegistrarPago(int idDeuda, decimal monto, string metodoPago)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_RegistrarPago", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdDeuda", idDeuda);
                cmd.Parameters.AddWithValue("@Monto", monto);
                cmd.Parameters.AddWithValue("@MetodoPago",
                    (object?)metodoPago ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        // 🔹 LISTAR TODOS LOS PAGOS (usando reporte)
        public List<Pago> ListarPagos()
        {
            List<Pago> lista = new List<Pago>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_ReportePagosDetalle", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Pago()
                    {
                        IdPago = dr.GetInt32(0),
                        NumeroRecibo = dr.GetString(1),
                        Fecha = dr.GetDateTime(2),
                        Monto = dr.GetDecimal(3),
                        MetodoPago = dr.GetString(4),
                        // Nota: las demás columnas del SP son joins (puesto, concepto, etc.)
                    });
                }
            }

            return lista;
        }

        // 🔹 PAGOS POR PUESTO (simplificado)
        public List<Pago> ObtenerPorPuesto(int idPuesto)
        {
            List<Pago> lista = new List<Pago>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                // reutilizamos el SP de reporte y filtramos en código
                SqlCommand cmd = new SqlCommand("sp_ReportePagosDetalle", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    int puestoId = dr.GetInt32(5); // columna Puesto (ojo orden del SP)

                    if (puestoId == idPuesto)
                    {
                        lista.Add(new Pago()
                        {
                            IdPago = dr.GetInt32(0),
                            NumeroRecibo = dr.GetString(1),
                            Fecha = dr.GetDateTime(2),
                            Monto = dr.GetDecimal(3),
                            MetodoPago = dr.GetString(4)
                        });
                    }
                }
            }

            return lista;
        }
        public decimal TotalRecaudadoPorRango(DateTime inicio, DateTime fin)
        {
            decimal total = 0;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_ReporteRecaudadoPorRango", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FechaInicio", inicio.Date);
                cmd.Parameters.AddWithValue("@FechaFin", fin.Date);

                var result = cmd.ExecuteScalar();

                if (result != null)
                    total = Convert.ToDecimal(result);
            }

            return total;
        }

    }
}
