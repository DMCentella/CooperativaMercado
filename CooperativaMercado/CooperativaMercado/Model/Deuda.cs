namespace CooperativaMercado.Model
{
    public class Deuda
    {
        public int IdDeuda { get; set; }

        public int IdPuesto { get; set; }
        public Puesto Puesto { get; set; }

        public int IdTipoDeuda { get; set; }
        public TipoDeuda TipoDeuda { get; set; }

        public string Descripcion { get; set; }
        public decimal Monto { get; set; }

        public int Mes { get; set; }
        public int Anio { get; set; }

        public DateTime? FechaVencimiento { get; set; }
        public string Estado { get; set; }

        public Pago Pago { get; set; }

    }
}
