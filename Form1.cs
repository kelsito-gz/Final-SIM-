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
            eventoAnterior.EventoNombre = "Inicializacion";
            eventoAnterior.CantidadClientesEnSistema = 0;
            AñadirEventoAGrilla(eventoAnterior);
            var idVehiculoNuevo = 1;

            for (int i = 0; i < CantidadAGenerar; i++)
            {
                Evento eventoActual = new Evento();
                eventoActual.GuardarEventoAnterior(eventoAnterior);
                double diferenciaRelojes;

                //Llega un cliente
                if(eventoActual.SiguienteLlegada <= eventoActual.TiempoFinAtencion || eventoActual.TiempoFinAtencion == 0)
                {
                    //Carga de datos generales respecto al evento
                    eventoActual.EventoNombre = "Llegada Cliente";
                    eventoActual.Reloj = eventoAnterior.SiguienteLlegada;
                    diferenciaRelojes = eventoActual.Reloj - eventoAnterior.Reloj;
                    PintarCeldaAnterior(true);
                    grid_simulacion.Columns.Add($"column {i}", $"Cliente {idVehiculoNuevo}");

                    Vehiculo vehiculo;

                    eventoActual.CalcularLlegadaCliente(randomTiempoLlegada.NextDouble(), MediaExponencialNegativa);

                    //Si existe un vehiculo en cola, se va, sino se queda esperando
                    if (eventoActual.Cola == 1)
                    {
                        vehiculo = new Vehiculo(idVehiculoNuevo, EstadoVehiculoEnum.CANSANSIO);
                        eventoActual.CantidadClientesPerdida++;

                        //Suma el tiempo de los vehiculos que se encuentran
                        eventoActual.TiempoTotalClientesEnSistema += diferenciaRelojes * 2; //x2 los dos vehiculos
                    }
                    else
                    {
                        //Si el servidor se encuentra libre, es atendido
                        if(eventoActual.EstadoServidor == EstadoServidor.LIBRE)
                        {
                            vehiculo = new Vehiculo(idVehiculoNuevo, EstadoVehiculoEnum.SIENDO_ATENDIDO);
                            eventoActual.CalcularTiempoFinAtencion(randomTiempoAtencion.NextDouble(), UniformeA, UniformeB, vehiculo);
                        }
                        //Si el servidor se encuentra ocupado, va a la cola
                        else
                        {
                            vehiculo = new Vehiculo(idVehiculoNuevo, EstadoVehiculoEnum.ESPERANDO_ATENCION);
                            eventoActual.Cola++;

                            //Suma el tiempo del vehiculo
                            eventoActual.TiempoTotalClientesEnSistema += diferenciaRelojes;
                        }
                    }
                    //Actualizamos el id de los nuevos vehiculos
                    eventoActual.CantidadClientesEnSistema++;
                    idVehiculoNuevo++;

                    //Controlamos que exista la lista
                    if(eventoActual.Vehiculos == null)
                    {
                        eventoActual.Vehiculos = new List<Vehiculo>();
                    }
                    eventoActual.Vehiculos.Add(vehiculo);
                }
                //Termina atencion
                else
                {
                    //Carga de datos generales respecto al evento
                    eventoActual.EventoNombre = "Fin atención";
                    eventoActual.Reloj = eventoAnterior.TiempoFinAtencion;
                    diferenciaRelojes = eventoActual.Reloj - eventoAnterior.Reloj;
                    PintarCeldaAnterior(false);

                    var vehiculoAtendido = eventoActual.Vehiculos.Find(x => x.Estado == EstadoVehiculoEnum.SIENDO_ATENDIDO);
                    eventoActual.Vehiculos.Remove(vehiculoAtendido);

                    if (eventoActual.Cola > 0)
                    {
                        Vehiculo vehiculoEnCola = eventoActual.Vehiculos.Find(x => x.Estado == EstadoVehiculoEnum.ESPERANDO_ATENCION);
                        eventoActual.CalcularTiempoFinAtencion(randomTiempoAtencion.NextDouble(), UniformeA, UniformeB, vehiculoEnCola);
                        eventoActual.Cola = 0;

                        //Suma el tiempo de los dos vehiculos;
                        eventoActual.TiempoTotalClientesEnSistema += diferenciaRelojes*2;
                    }
                    else
                    {
                        eventoActual.EstadoServidor = EstadoServidor.LIBRE;
                        eventoActual.TiempoFinAtencion = 0;

                        //Suma el tiempo del vehiculo que se fue
                        eventoActual.TiempoTotalClientesEnSistema += diferenciaRelojes;
                    }
                }

                //Calcula el tiempo promedio de permanencia en el sistema
                eventoActual.TiempoPermanenciaCliente = eventoActual.TiempoTotalClientesEnSistema / (eventoActual.CantidadClientesEnSistema - eventoActual.CantidadClientesPerdida);

                //Se verificia si la fila debe ser mostrada, si se encuentra en el desde y hasta, o es la ultima
                if(( i+1 >= Desde && i+1 <= Hasta) || i == CantidadAGenerar - 1 )
                {
                    AñadirEventoAGrilla(eventoActual);
                }

                //Elimina el vehiculo cansado para que aparezca el registro aunque no ingreso dentro del sistema
                var vehiculoCansado = eventoActual.Vehiculos.Find(x => x.Estado == EstadoVehiculoEnum.CANSANSIO);
                if(vehiculoCansado != null)
                    eventoActual.Vehiculos.Remove(vehiculoCansado);


                eventoAnterior = eventoActual;

            }


        }

        /// <summary>
        /// Metodo que sirve para cargar un evento en la grilla
        /// </summary>
        /// <param name="evento"></param>
        private void AñadirEventoAGrilla(Evento evento)
        {
            int i = grid_simulacion.Rows.Add();
            grid_simulacion.Rows[i].Cells[0].Value = evento.EventoNombre;
            grid_simulacion.Rows[i].Cells[1].Value = TruncarACuatro(evento.Reloj);
            grid_simulacion.Rows[i].Cells[2].Value = TruncarACuatro(evento.RandomLlegada);
            grid_simulacion.Rows[i].Cells[3].Value = TruncarACuatro(evento.TiempoEntreLlegada);
            grid_simulacion.Rows[i].Cells[4].Value = TruncarACuatro(evento.SiguienteLlegada);
            grid_simulacion.Rows[i].Cells[5].Value = TruncarACuatro(evento.RandomAtencion);
            grid_simulacion.Rows[i].Cells[6].Value = TruncarACuatro(evento.TiempoAtencion);
            grid_simulacion.Rows[i].Cells[7].Value = TruncarACuatro(evento.TiempoFinAtencion);
            grid_simulacion.Rows[i].Cells[8].Value = evento.EstadoServidor;
            grid_simulacion.Rows[i].Cells[9].Value = evento.Cola;
            grid_simulacion.Rows[i].Cells[10].Value = evento.CantidadClientesPerdida;
            grid_simulacion.Rows[i].Cells[11].Value = TruncarACuatro(evento.TiempoTotalClientesEnSistema);
            grid_simulacion.Rows[i].Cells[12].Value = evento.CantidadClientesEnSistema;
            grid_simulacion.Rows[i].Cells[13].Value = TruncarACuatro(evento.TiempoPermanenciaCliente);
            
            if(evento.Vehiculos != null)
            {
                for (int j = 0; j < evento.Vehiculos.Count; j++)
                {
                    var vehiculo = evento.Vehiculos[j];
                    grid_simulacion.Rows[i].Cells[13 + Int16.Parse(vehiculo.id.ToString())].Value = vehiculo.Estado;
                }
            }
            
        }

        /// <summary>
        /// Metodo que sirve para pintar el evento que gano en orden temporal
        /// </summary>
        /// <param name="esLlegadaCliente"></param>
        private void PintarCeldaAnterior(bool esLlegadaCliente)
        {
            int x = grid_simulacion.Rows.Count -1;
            int y = esLlegadaCliente ? 4 : 7;
            grid_simulacion.Rows[x].Cells[y].Style.BackColor = Color.Orange;
        }

        /// <summary>
        /// Metodo que trunca a cuatro decimales un numero
        /// </summary>
        /// <param name="numero"></param>
        /// <returns></returns>
        private double TruncarACuatro(double numero)
        {
            return Math.Truncate(numero * 1000) / 1000;
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
        public long CantidadClientesPerdida { get; set; }
        public double TiempoTotalClientesEnSistema { get; set; }
        public int CantidadClientesEnSistema { get; set; }
        public double TiempoPermanenciaCliente { get; set; }
        public List<Vehiculo> Vehiculos { get; set; }

        /// <summary>
        /// Calcula el tiempo de llegada de un cliente
        /// </summary>
        /// <param name="random"></param>
        /// <param name="media"></param>
        public void CalcularLlegadaCliente(double random, double media)
        {
            RandomLlegada = random;
            TiempoEntreLlegada = (media * -1) * (Math.Log(1 - random));
            SiguienteLlegada = TiempoEntreLlegada + Reloj;
        }

        /// <summary>
        /// Calcula el tiempo de atencion del cliente
        /// </summary>
        /// <param name="random"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="vehiculo"></param>
        public void CalcularTiempoFinAtencion(double random, double a, double b, Vehiculo vehiculo)
        {
            RandomAtencion = random;
            TiempoAtencion = a + (b - a) * random;
            EstadoServidor = EstadoServidor.OCUPADO;
            TiempoFinAtencion = TiempoAtencion + Reloj;
            vehiculo.SetearFinAtencion();
        }

        /// <summary>
        /// Metodo que copia los atributos importantes que se necesitan mantener del evento anterior
        /// </summary>
        /// <param name="evento"></param>
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
            CantidadClientesPerdida = evento.CantidadClientesPerdida;
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

        public void SetearFinAtencion()
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