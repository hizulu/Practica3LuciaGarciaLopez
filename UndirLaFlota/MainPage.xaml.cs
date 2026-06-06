using UndirLaFlota.Juego;

namespace UndirLaFlota
{
    public partial class MainPage : ContentPage
    {
        private Tablero tabMaquina;
        private Tablero tabJugador;
        private int disparos = 0;
        private int aciertosUI = 0;
        private bool turnoJugador = true;

        public MainPage()
        {
            InitializeComponent();
            IniciarJuego();
        }

        /// <summary>
        /// Inicializa el juego creando los tableros para la máquina y el jugador, reseteando los contadores de disparos y aciertos, y generando los botones en la interfaz.
        /// </summary>
        private void IniciarJuego()
        {
            tabMaquina = new Tablero();
            tabJugador = new Tablero();

            disparos = 0;
            aciertosUI = 0;
            turnoJugador = true;
            ActualizarMarcadores();

            LimpiarGrids();
            GenerarBotonesMaquina();
            GenerarBotonesJugador();
        }

        /// <summary>
        /// Elimina los botones de los tableros de la máquina y el jugador para preparar la interfaz para una nueva partida.
        /// </summary>
        private void LimpiarGrids()
        {
            var botonesMaquina = TableroGridEnemigo.Children.OfType<ImageButton>().ToList();
            foreach (var btn in botonesMaquina) TableroGridEnemigo.Children.Remove(btn);

            var botonesJugador = MiTableroGrid.Children.OfType<ImageButton>().ToList();
            foreach (var btn in botonesJugador) MiTableroGrid.Children.Remove(btn);
        }

        /// <summary>
        /// Genera los botones para el tablero de la máquina, asignando la imagen inicial y el evento de clic para cada botón. El evento de clic solo se activa si es el turno del jugador.
        /// </summary>
        private void GenerarBotonesMaquina()
        {
            for (int i = 0; i < tabMaquina.Dim; i++)
            {
                for (int j = 0; j < tabMaquina.Dim; j++)
                {
                    var imageButton = new ImageButton
                    {
                        Source = "sea.png",
                        CommandParameter = new Tuple<int, int>(i, j)
                    };

                    imageButton.Clicked += (sender, e) =>
                    {
                        if (!turnoJugador) return;

                        var btn = sender as ImageButton;
                        var position = (Tuple<int, int>)btn.CommandParameter;
                        Seleccion(btn, position.Item1, position.Item2);
                    };

                    TableroGridEnemigo.Add(imageButton, i + 1, j + 1);
                }
            }
        }

        /// <summary>
        /// Genera los botones para el tablero del jugador, asignando la imagen inicial basada en si hay un barco o no. Estos botones no tienen eventos de clic ya que el jugador no interactúa directamente con su propio tablero.
        /// </summary>
        private void GenerarBotonesJugador()
        {
            for (int i = 0; i < tabJugador.Dim; i++)
            {
                for (int j = 0; j < tabJugador.Dim; j++)
                {
                    string imagenInicial = tabJugador.TableroList[i][j] >= 10 ? "barco.png" : "sea.png";

                    var imageButton = new ImageButton
                    {
                        Source = imagenInicial,
                    };

                    MiTableroGrid.Add(imageButton, i + 1, j + 1);
                }
            }
        }

        //Turno Jugador
        /// <summary>
        /// Maneja la selección del jugador al hacer clic en un botón del tablero de la máquina. Realiza la jugada, actualiza el contador de disparos y aciertos, cambia la imagen del botón según el resultado, y luego inicia el turno de la máquina.
        /// </summary>
        /// <param name="btn">El botón que el jugador ha seleccionado para disparar. Este botón se actualizará visualmente según el resultado de la jugada (agua, tocado, hundido).</param>
        /// <param name="row">La fila del tablero de la máquina que el jugador ha seleccionado para disparar. Este valor se utiliza para determinar la posición en el tablero y realizar la jugada correspondiente.</param>
        /// <param name="column">La columna del tablero de la máquina que el jugador ha seleccionado para disparar. Similar a la fila, este valor se utiliza para determinar la posición en el tablero y realizar la jugada correspondiente.</param>
        public async void Seleccion(ImageButton btn, int row, int column)
        {
            String str = tabMaquina.Jugada(row, column);
            if (str == null) return;

            disparos++;

            if (str.Equals("Agua")) btn.Source = "agua_fallo.png";
            else if (str.Equals("Tocado")) { aciertosUI++; btn.Source = "tocado.png"; }
            else if (str.Equals("Hundido"))
            {
                aciertosUI++; btn.Source = "hundido.png";
                await DisplayAlert("¡Hundido!", "Has destruido un barco enemigo.", "OK");
            }
            else if (str.Equals("Partida finalizada"))
            {
                aciertosUI++; btn.Source = "hundido.png";
                ActualizarMarcadores();
                await DisplayAlert("¡Victoria!", "¡Has hundido toda la flota enemiga!", "OK");
                return; 
            }

            ActualizarMarcadores();
            TurnoMaquina();
        }

        //Turno Maquina
        /// <summary>
        /// Realiza el turno de la máquina después de que el jugador ha hecho su jugada. La máquina selecciona aleatoriamente una posición en el tablero del jugador para disparar, verifica si la jugada es válida (no se ha disparado antes), y luego actualiza el tablero del jugador según el resultado (agua, tocado, hundido). Si la máquina hunde un barco o finaliza la partida, muestra una alerta correspondiente. Después de la jugada de la máquina, se vuelve a permitir que el jugador haga su movimiento.
        /// </summary>
        private async void TurnoMaquina()
        {
            turnoJugador = false; 
            await Task.Delay(1000);

            Random rnd = new Random();
            bool jugadaValida = false;
            int x = 0, y = 0;

            while (!jugadaValida)
            {
                x = rnd.Next(0, tabJugador.Dim);
                y = rnd.Next(0, tabJugador.Dim);

                int valor = tabJugador.TableroList[x][y];
                if (valor == 0 || valor >= 10)
                {
                    jugadaValida = true;
                }
            }

            String resultado = tabJugador.Jugada(x, y);

            var miBoton = MiTableroGrid.Children.OfType<ImageButton>().FirstOrDefault(b => Grid.GetRow(b as BindableObject) == x + 1 && Grid.GetColumn(b as BindableObject) == y + 1);

            if (miBoton != null)
            {
                if (resultado.Equals("Agua")) miBoton.Source = "agua_fallo.png";
                else if (resultado.Equals("Tocado")) miBoton.Source = "hundido.png";
                else if (resultado.Equals("Hundido"))
                {
                    miBoton.Source = "hundido.png";
                    await DisplayAlert("Alerta", "La máquina ha hundido uno de tus barcos.", "OK");
                }
                else if (resultado.Equals("Partida finalizada"))
                {
                    miBoton.Source = "hundido.png";
                    await DisplayAlert("Derrota", "La máquina ha hundido toda tu flota.", "OK");
                    return;
                }
            }

            turnoJugador = true;

            if (resultado.Equals("Partida finalizada"))
            {
                await
                DisplayAlert("Nueva Partida", "¿Quieres iniciar una nueva partida?", "Sí", "No").ContinueWith(t =>
                {
                    if (t.Result)
                    {
                        IniciarJuego();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        /// <summary>
        /// Actualiza los marcadores de disparos y aciertos en la interfaz de usuario después de cada jugada. Este método se llama tanto después del turno del jugador como del turno de la máquina para reflejar el estado actual del juego en la pantalla.
        /// </summary>
        private void ActualizarMarcadores()
        {
            LblDisparos.Text = $"Disparos: {disparos}";
            LblAciertos.Text = $"Aciertos: {aciertosUI}";
        }

        /// <summary>
        /// Maneja el evento de clic para el botón de "Nueva Partida". Cuando el jugador hace clic en este botón, se llama al método IniciarJuego() para reiniciar el juego y preparar la interfaz para una nueva partida. Esto permite al jugador comenzar de nuevo sin tener que cerrar y volver a abrir la aplicación.
        /// </summary>
        /// <param name="sender">El objeto que envió el evento de clic, en este caso, el botón de "Nueva Partida". Este parámetro se puede utilizar para identificar qué botón fue presionado si hay múltiples botones que comparten el mismo evento de clic. Sin embargo, en este caso específico, no se utiliza dentro del método ya que solo hay un botón que desencadena esta acción.</param>
        /// <param name="e">Los argumentos del evento de clic. Este parámetro contiene información adicional sobre el evento, como el tipo de evento, pero en este caso específico, no se utiliza dentro del método ya que no se requiere información adicional para iniciar una nueva partida. El método simplemente llama a IniciarJuego() para reiniciar el juego sin necesidad de procesar detalles específicos del evento.</param>
        private void OnNuevaPartidaClicked(object sender, EventArgs e)
        {
            IniciarJuego();
        }
    }
}