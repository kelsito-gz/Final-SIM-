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
            grid_simulacion.Rows.Clear();
            Simular();
        }

        private void Simular()
        {
            //Inicializar las variables
            Random randomTiempoLlegada = new Random();
            Random randomTiempoAtencion = new Random();
            Evento eventoAnterior = new Evento();

            //Realizamos la primera ejecución que carga cuando vendra el primer cliente
            eventoAnterior.CalcularLlegadaCliente(randomTiempoLlegada.NextDouble(), MediaExponencialNegativa);
            AñadirEventoAGrilla(eventoAnterior);

            for (int i = 0; i < CantidadAGenerar; i++)
            {
                Evento eventoActual = new Evento();
                eventoActual.GuardarEventoAnterior(eventoAnterior);

                //Llega un cliente
                if(eventoActual.SiguienteLlegada <= eventoActual.TiempoFinAtencion)
                {
                    eventoActual.EventoNombre = "Llegada Cliente";
                    var idUltimoVehiculo = eventoActual.Vehiculos[eventoActual.Vehiculos.Count - 1].id;
                    Vehiculo vehiculo;
                    if (eventoActual.Cola == 1)
                    {
                        vehiculo = new Vehiculo(idUltimoVehiculo++, EstadoVehiculoEnum.CANSANSIO);
                    }
                    else
                    {
                        if(eventoActual.EstadoServidor == EstadoServidor.LIBRE)
                        {
                            vehiculo = new Vehiculo(idUltimoVehiculo++, EstadoVehiculoEnum.SIENDO_ATENDIDO);
                            eventoActual.EstadoServidor = EstadoServidor.OCUPADO;
                        }
                        else
                        {
                            vehiculo = new Vehiculo(idUltimoVehiculo++, EstadoVehiculoEnum.ESPERANDO_ATENCION);
                            eventoActual.Cola++;
                        }
                    }
                    eventoActual.Vehiculos.Add(vehiculo);
                }
                //Termina atencion
                else
                {
                    eventoActual.EventoNombre = "Fin atención";
                    if(eventoActual.Cola > 0)
                    {
                        Vehiculo vehiculoEnCola = eventoActual.Vehiculos.Find(x => x.Estado == EstadoVehiculoEnum.ESPERANDO_ATENCION);
                        eventoActual.CalcularTiempoFinAtencion(randomTiempoAtencion.NextDouble(), UniformeA, UniformeB, vehiculoEnCola);
                        eventoActual.Cola = 0;
                    }
                    else
                    {
                        eventoActual.EstadoServidor = EstadoServidor.LIBRE;
                    }
                }

                //Se verificia si la fila debe ser mostrada, si se encuentra en el desde y hasta, o es la ultima
                if(( i >= Desde && i <= Hasta) || i == CantidadAGenerar - 1 )
                {
                    AñadirEventoAGrilla(eventoActual);
                }

            }


        }

        private void AñadirEventoAGrilla(Evento evento)
        {
            int i = grid_simulacion.Rows.Add();
            grid_simulacion.Rows[i].Cells[0].Value = evento.EventoNombre;
            grid_simulacion.Rows[i].Cells[1].Value = evento.Reloj;
            grid_simulacion.Rows[i].Cells[2].Value = evento.RandomLlegada;
            grid_simulacion.Rows[i].Cells[3].Value = evento.TiempoEntreLlegada;
            grid_simulacion.Rows[i].Cells[4].Value = evento.SiguienteLlegada;
            grid_simulacion.Rows[i].Cells[5].Value = evento.RandomAtencion;
            grid_simulacion.Rows[i].Cells[6].Value = evento.TiempoAtencion;
            grid_simulacion.Rows[i].Cells[7].Value = evento.TiempoFinAtencion;
            grid_simulacion.Rows[i].Cells[8].Value = evento.EstadoServidor;
            grid_simulacion.Rows[i].Cells[9].Value = evento.Cola;
            grid_simulacion.Rows[i].Cells[10].Value = evento.TiempoTotalClientesEnSistema;
            grid_simulacion.Rows[i].Cells[11].Value = evento.CantidadClientesEnSistema;
            grid_simulacion.Rows[i].Cells[12].Value = evento.TiempoPermanenciaCliente;

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
            vehiculo.SetearTiempoFinAtencion();
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
        public EstadoVehiculoEnum Estado { get; set; }

        public Vehiculo(long _id, EstadoVehiculoEnum _estado)
        {
            id = _id;
            Estado = _estado;
        }

        public void SetearTiempoFinAtencion()
        {
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