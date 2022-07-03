namespace TP_Final
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


    }

    public class Evento
    {
        public double Reloj { get; set; }
        public double RandomLlegada { get; set; }
        public double TiempoEntreLlegada { get; set; }
        public double SiguienteLlegada { get; set; }
        public double RandomAtencion { get; set; }
        public double TiempoAtencion { get; set; }
        public double TiempoFinAtencion { get; set; }
        public EstadoServidor EstadoServidor { get; set; }
        public int Cola { get; set; }
        public int TiempoTotalClientesEnSistema { get; set; }
        public int CantidadClientesEnSistema { get; set; }
        public double TiempoPermanenciaCliente { get; set; }
        public List<Vehiculo> Vehiculos { get; set; }


        public void CalcularLlegadaCliente(double random, double media)
        {
            RandomLlegada = random;
            TiempoEntreLlegada = (media * -1) * (Math.Log(1 - random));
            SiguienteLlegada = TiempoEntreLlegada + Reloj;
        }

        public void CalcularTiempoFinAtencion(double random, double a, double b, Vehiculo vehiculo)
        {
            if (Vehiculos.Count == 0)
            {
                Vehiculos = new List<Vehiculo>();
            }
            RandomAtencion = random;
            TiempoAtencion = a + (b - a) * random;
            TiempoFinAtencion = TiempoAtencion + Reloj;
            vehiculo.SetearTiempoFinAtencion(TiempoFinAtencion);
            Vehiculos.Add(vehiculo);
        }

        public void GuardarEventoAnterior(Evento evento)
        {
            SiguienteLlegada = evento.SiguienteLlegada;
            TiempoFinAtencion = evento.TiempoFinAtencion;
            EstadoServidor = evento.EstadoServidor;
            Cola = evento.Cola;
            TiempoTotalClientesEnSistema = evento.TiempoTotalClientesEnSistema;
            CantidadClientesEnSistema = evento.CantidadClientesEnSistema;
            TiempoPermanenciaCliente = evento.TiempoPermanenciaCliente;
            Vehiculos = evento.Vehiculos;
        }
    }

    public class Vehiculo
    {
        public long id { get; set; }
        public double HoraLlegada { get; set; }
        public double TiempoFinAtencion { get; set; }
        public EstadoVehiculoEnum Estado { get; set; }

        public Vehiculo(long _id, double _horaLlegada, EstadoVehiculoEnum _estado)
        {
            id = _id;
            HoraLlegada = _horaLlegada;
            Estado = _estado;
        }

        public void SetearTiempoFinAtencion(double tiempoFinAtencion)
        {
            TiempoFinAtencion = tiempoFinAtencion;
            Estado = EstadoVehiculoEnum.SIENDO_ATENDIDO;
        }

        public void CansansioCliente()
        {
            Estado = EstadoVehiculoEnum.CANSANSIO;
        }

    }

    public enum EstadoVehiculoEnum
    {
        ESPERANDO_ATENCION = 1,
        SIENDO_ATENDIDO = 2,
        CANSANSIO = 3,
    }

    public enum EstadoServidor
    {
        LIBRE = 0,
        OCUPADO = 1,
    }
}