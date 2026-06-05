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
                    var button = new Button
                    {
                        BackgroundColor = Random.Shared.Next(0, 2) == 0 ? Colors.MediumTurquoise : Colors.DarkTurquoise,
                        Text = "OLA",
                        TextColor = Colors.Transparent,
                        CornerRadius = 0,
                        CommandParameter = new Tuple<int, int>(i, j)
                    };

                    button.Clicked += (sender, e) =>
                    {
                        var btn = sender as Button;
                        var position = (Tuple<int, int>)btn.CommandParameter;
                        Seleccion(btn, position.Item1, position.Item2);
                    };

                    TableroGrid.Add(button, i + 1, j + 1);
                }
            }
        }

        public void Seleccion(Button btn, int row, int column)
        {
            String str = tab.Jugada(row, column);

            if (str == null) return;

            disparos++;

            if (str.Equals("Agua"))
            {
                btn.Text = "🌊";
                btn.BackgroundColor = Colors.Teal;
            }
            else if (str.Equals("Tocado"))
            {
                aciertosUI++;
                btn.Text = "💥";
                btn.BackgroundColor = Colors.Orange;
            }
            else if (str.Equals("Hundido"))
            {
                aciertosUI++;
                btn.Text = "☠️";
                btn.BackgroundColor = Colors.Red;
                DisplayAlert("¡Hundido!", "Has destruido un barco enemigo por completo.", "OK");
            }
            else if (str.Equals("Partida finalizada"))
            {
                aciertosUI++;
                btn.Text = "🏆";
                btn.BackgroundColor = Colors.Gold;
                DisplayAlert("¡Victoria!", "¡Has hundido toda la flota enemiga!", "OK");
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