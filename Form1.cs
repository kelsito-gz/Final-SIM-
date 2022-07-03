namespace TP_Final
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private double UniformeA { get; set; }
        private double UniformeB { get; set; }
        private double MediaExponencialNegativa { get; set; }
        private long CantidadAGenerar { get; set; }
        private long Desde { get; set; }
        private long Hasta { get; set;}

        /// <summary>
        /// Método que valida si la simulación tiene los datos correcto para funcionar.
        /// </summary>
        /// <returns></returns>
        private bool ValidarSimular()
        {
            try
            {
                if(this.txt_cantidad.Text == "" || this.txt_uniforme_a.Text == "" || this.txt_uniforme_b.Text == "" || this.txt_media.Text == ""
                    || this.txt_desde.Text == "" || this.txt_hasta.Text == "")
                {
                    MessageBox.Show("Existen datos incompletos");
                    return false;
                }
                UniformeA = Double.Parse(this.txt_uniforme_a.Text);
                UniformeB = Double.Parse(this.txt_uniforme_b.Text);
                MediaExponencialNegativa = double.Parse(this.txt_media.Text);
                CantidadAGenerar = long.Parse(this.txt_cantidad.Text);
                Desde = long.Parse(this.txt_desde.Text);
                Hasta = long.Parse(this.txt_hasta.Text);
                if(UniformeA <= 0 || UniformeB <= 0 || MediaExponencialNegativa <= 0)
                {
                    MessageBox.Show("Los valores de las distribuciones deben ser positivas mayores a 0");
                    return false;
                }
                if(Desde == Hasta)
                {
                    MessageBox.Show("Desde y hasta deben ser diferentes");
                    return false;
                }
                if(Hasta > CantidadAGenerar)
                {
                    MessageBox.Show("La cantidad a mostrar no puede ser mayor a la cantidad generada");
                    return false;
                }
                if(Desde <= 0 || Hasta <= 0)
                {
                    MessageBox.Show("Las filas a mostrar deben ser positivas");
                    return false;
                }
                if(Desde > Hasta)
                {
                    var auxiliarCambioVariable = Hasta;
                    Hasta = Desde;
                    Hasta = auxiliarCambioVariable;
                    this.txt_hasta.Text = Hasta.ToString();
                    this.txt_desde.Text = Desde.ToString();
                }
                if(CantidadAGenerar <= 0)
                {
                    MessageBox.Show("Debe cargar una cantidad a generar");
                    return false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Existe un error en los valores cargados. Por favor revise nuevamente los datos");
                return false;
                throw;
            }
            return true;
        }

        private void btn_simular_Click(object sender, EventArgs e)
        {
            if (!ValidarSimular())
                return;
            
        }
    }

    public class Evento
    {
        public string EventoNombre { get; set; }
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