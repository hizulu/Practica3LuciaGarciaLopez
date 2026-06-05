namespace UndirLaFlota.Juego;

public class BarcoInfo
{
    public int Id { get; set; }
    public int Tamano { get; set; }
    public int Toques { get; set; }
    public bool Hundido => Toques >= Tamano;
}

public class Tablero
{
    public List<List<int>> TableroList { get; set; }
    private List<int> Barcos;
    public List<BarcoInfo> ListaBarcosInfo { get; set; }
    private int aciertos = 0;
    private int TotalPuntos;
    public int Dim;
    private int nextBarcoId = 10;

    public Tablero()
    {
        Dim = 10;
        TableroList = new List<List<int>>();
        Barcos = new List<int> { 6, 5, 3, 3, 2 };
        ListaBarcosInfo = new List<BarcoInfo>();

        foreach (int i in Barcos)
        {
            TotalPuntos += i;
        }

        TableroLimpio();
        GenerarTablero();
    }

    public void GenerarTablero()
    {
        foreach (int i in Barcos)
        {
            GenerarBarco(i);
        }
    }

    private void GenerarBarco(int tamano)
    {
        Random random = new Random();
        int x = 0, y = 0, dir = 0, P_x = 0, P_y = 0, pos = 0;
        bool entra = false;

        while (!entra)
        {
            entra = true;
            x = random.Next(0, Dim);
            y = random.Next(0, Dim);
            dir = random.Next(0, 4);
            P_x = x;
            P_y = y;
            pos = 0;

            while (pos < tamano)
            {
                if (P_x < 0 || P_x >= 10 || P_y < 0 || P_y >= 10)
                {
                    entra = false;
                    break;
                }
                else if (TableroList[P_x][P_y] != 0)
                {
                    entra = false;
                    break;
                }

                switch (dir)
                {
                    case 0: P_x++; break;
                    case 1: P_x--; break;
                    case 2: P_y++; break;
                    case 3: P_y--; break;
                }
                pos++;
            }
        }

        if (entra)
        {
            P_x = x;
            P_y = y;
            pos = 0;

            int idActual = nextBarcoId++;
            ListaBarcosInfo.Add(new BarcoInfo { Id = idActual, Tamano = tamano, Toques = 0 });

            while (pos < tamano)
            {
                TableroList[P_x][P_y] = idActual;
                switch (dir)
                {
                    case 0: P_x++; break;
                    case 1: P_x--; break;
                    case 2: P_y++; break;
                    case 3: P_y--; break;
                }
                pos++;
            }
        }
    }

    private void TableroLimpio()
    {
        aciertos = 0;
        for (int i = 0; i < Dim; i++)
        {
            TableroList.Add(new List<int>());
            for (int j = 0; j < Dim; j++)
            {
                TableroList[i].Add(0);
            }
        }
    }

    public String Jugada(int x, int y)
    {
        int valorCelda = TableroList[x][y];

        if (valorCelda == 0) //Agua
        {
            TableroList[x][y] = 1;
            return "Agua";
        }
        else if (valorCelda == 1 || valorCelda == 3 || valorCelda < 0) //Ya se ha disparado aquí
        {
            return null;
        }
        else if (valorCelda >= 10) //Barco
        {
            aciertos++;
            TableroList[x][y] = -valorCelda; //Barco tocado

            var barco = ListaBarcosInfo.FirstOrDefault(b => b.Id == valorCelda);
            if (barco != null)
            {
                barco.Toques++;

                if (aciertos >= TotalPuntos)
                {
                    return "Partida finalizada";
                }
                else if (barco.Hundido)
                {
                    return "Hundido";
                }
            }
            return "Tocado";
        }
        return null;
    }
}