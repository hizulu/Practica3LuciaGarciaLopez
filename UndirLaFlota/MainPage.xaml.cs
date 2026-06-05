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

        private void LimpiarGrids()
        {
            var botonesMaquina = TableroGridEnemigo.Children.OfType<ImageButton>().ToList();
            foreach (var btn in botonesMaquina) TableroGridEnemigo.Children.Remove(btn);

            var botonesJugador = MiTableroGrid.Children.OfType<ImageButton>().ToList();
            foreach (var btn in botonesJugador) MiTableroGrid.Children.Remove(btn);
        }

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

        private void ActualizarMarcadores()
        {
            LblDisparos.Text = $"Disparos: {disparos}";
            LblAciertos.Text = $"Aciertos: {aciertosUI}";
        }

        private void OnNuevaPartidaClicked(object sender, EventArgs e)
        {
            IniciarJuego();
        }
    }
}