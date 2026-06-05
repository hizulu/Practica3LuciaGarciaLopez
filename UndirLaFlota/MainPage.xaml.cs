using UndirLaFlota.Juego;

namespace UndirLaFlota
{
    public partial class MainPage : ContentPage
    {
        private Tablero tab;
        private int disparos = 0;
        private int aciertosUI = 0;

        public MainPage()
        {
            InitializeComponent();
            IniciarJuego();
        }

        private void IniciarJuego()
        {
            tab = new Tablero();
            disparos = 0;
            aciertosUI = 0;
            ActualizarMarcadores();

            var botonesAntiguos = TableroGrid.Children.OfType<Button>().ToList();
            foreach (var btn in botonesAntiguos)
            {
                TableroGrid.Children.Remove(btn);
            }

            for (int i = 0; i < tab.Dim; i++)
            {
                for (int j = 0; j < tab.Dim; j++)
                {
                    var imageButton = new ImageButton
                    {
                        Source = "sea.png",
                        CornerRadius = 0,
                        CommandParameter = new Tuple<int, int>(i, j)
                    };

                    imageButton.Clicked += (sender, e) =>
                    {
                        var btn = sender as ImageButton;
                        var position = (Tuple<int, int>)btn.CommandParameter;
                        Seleccion(btn, position.Item1, position.Item2);
                    };

                    TableroGrid.Add(imageButton, i + 1, j + 1);
                }
            }
        }

        public void Seleccion(ImageButton btn, int row, int column)
        {
            String str = tab.Jugada(row, column);

            if (str == null) return;

            disparos++;

            if (str.Equals("Agua"))
            {
                btn.Source = "agua_fallo.png";
            }
            else if (str.Equals("Tocado"))
            {
                aciertosUI++;
                btn.Source = "tocado.png";
            }
            else if (str.Equals("Hundido"))
            {
                aciertosUI++;
                btn.Source = "hundido.png";
                DisplayAlert("¡Hundido!", "Has destruido un barco enemigo por completo.", "OK");
            }
            else if (str.Equals("Partida finalizada"))
            {
                aciertosUI++;
                btn.Source = "hundido.png";
                DisplayAlert("¡Victoria!", "¡Has hundido toda la flota enemiga!", "OK");
                DisplayAlert("Nueva Partida", "¿Quieres iniciar una nueva partida?", "Sí", "No").ContinueWith(t =>
                {
                    if (t.Result)
                    {
                        IniciarJuego();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }

            ActualizarMarcadores();
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