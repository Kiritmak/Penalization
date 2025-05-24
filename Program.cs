namespace Penalization
{
  public interface IPenalizado
  {
    double CalcularPenalizacion();
    void ValidarFormato(string[] args);
  }

  public class Penalizado : IPenalizado
  {
    public Penalizado(string? Formato = "10%,Nombre")
    {
      string[] Elementos = Formato.Split(",");
      for(int i = 0; i<Elementos.Length; i++)
      {
        string newElement = "";
        for (int j = 0; j < Elementos[i].Length; j++)
        {
          if (Elementos[i][j] == ' ') continue;
          newElement += Elementos[i][j];
        }
        Elementos[i] = newElement;
      }

      ValidarFormato(Elementos);
      CantidadFinal = CalcularPenalizacion();
    }

    double PorcentajePenalizado { get; set; }
    public double CantidadFinal
    {
      get; private set;
    }
    public string Nombre { get; private set; }

    public double CalcularPenalizacion()
    {
      //Redondeando al entero mas pequeño
      double CantidadBase = Program.RepartoInicial * PorcentajePenalizado;
      Program.ParseAmmount(ref CantidadBase);

      //Obteniendo ganancia del custodio
      Program.Custodio += CantidadBase % 10;
      CantidadBase -= CantidadBase % 10;

      return CantidadBase;
    }

    public void ValidarFormato(string[] Formato)
    {
      //Separa el numero seguido de la X en el Formato (X%)
      PorcentajePenalizado = double.Parse(Formato[0].Substring(0, Formato[0].IndexOf('%')));
      PorcentajePenalizado /= 100;

      Nombre = Formato[1];
    }

    public override string ToString()
    {
      string s = $"{PorcentajePenalizado*100}% {CantidadFinal}";
      return s;
    }
  }

  public static class Program
  {
    static List<Penalizado> Penalizados = new();
    static double RepartoRegular = 0;
    static double CantidadInicial = 0;
    public static double RepartoInicial = 0;
    public static double Custodio = 0;
    static double RestoDePenalizados = 0;
    static int TotalTrabajadores = 0;
    static int TotalPenalizados = 0;

    static string TABBING = "*****************************************************\n";

    public static void Main(string[] args)
    {
      Penalizado penalizado = new Penalizado();
      Console.WriteLine($"{TABBING} Bienvenido a Penalization, Copyright © Kiritmak \n{TABBING}");

      Console.WriteLine("Ingrese la cantidad a repartir, asegurese de separar lugares decimales por puntos (.)\n");
      InputAmmount(out CantidadInicial);

      Console.WriteLine("\nIngrese la cantidad de trabajadores en el turno (Sin incluir al custodio)\n");
      InputAmmount(out TotalTrabajadores);
      RepartoInicial = CantidadInicial / TotalTrabajadores;

      Console.WriteLine("\nIngrese la cantidad de penalizados en el turno\n");
      InputAmmount(out TotalPenalizados);

      Console.WriteLine("\nAhora, ingrese la informacion requerida de cada trabajador penalizado, teniendo en cuenta el siguiente formato");
      Console.WriteLine(FormatoPenalizacion());

      //Recibiendo la data de los penalizados
      for(int i=1; i<=TotalPenalizados; i++)
      {    
        try
        {
          string Input;
          Console.WriteLine($"Penalizado {i} :\n");
          Input = Console.ReadLine();
          Penalizado NewPenalizado = new Penalizado(Input);
          Penalizados.Add(NewPenalizado);
          Console.WriteLine();
        }
        catch (Exception ex) 
        {
          Console.WriteLine("Por favor, siga el formato establecido\n");
          i--;
        } 
      }

      //Calculando la ganancia de los no penalizados
      foreach (var p in Penalizados)
        RestoDePenalizados += RepartoInicial-p.CantidadFinal;
      RepartoRegular = (RestoDePenalizados) / NoPenalizados() + RepartoInicial;
      ParseAmmount(ref RepartoRegular);

      //Calculando la ganancia del custodio
      Custodio += (RepartoRegular % 10) * NoPenalizados();
      RepartoRegular -= RepartoRegular%10;

      DisplayProffits();
    }

    static void InputAmmount<T>(out T Number) where T : IParsable<T>
    {
      string? Input = Console.ReadLine();
      while (Input == null || ! T.TryParse(Input, null, out Number) ) 
      { 
        Console.WriteLine("El formato recivido es invalido");
        Input = Console.ReadLine();
      }
    }

    static string FormatoPenalizacion()
    {
      return "PorcentajeEnBase100 (X%)\n";
    }

    static void DisplayProffits()
    {
      List<string> Lineas = TextoPenalizados().Split('\n').ToList();
      Lineas.AddRange(TextoNoPenalizados().Split('\n').ToList());
      Lineas.AddRange(TextoSobrante().Split('\n').ToList());
      foreach (string Line in Lineas)
        Console.WriteLine(Line);
    }
    static string TextoPenalizados()
    {
      string s = "";
      s += $"{TABBING}Informacion sobre los penalizados\n\n";
      for (int i = 1; i <= Penalizados.Count; i++)
        s += $"{Penalizados[i]}\n\n";
      s += $"{TABBING}\n";
      return s;
    }
    static string TextoNoPenalizados()
    {
      string s = "";
      s += $"{TABBING}Informacion sobre los no penalizados\n\n";
      s += $"Ganancia para trabajadores no penalizados: {RepartoRegular}\n";
      s += $"{TABBING}\n";
      return s;
    }
    static string TextoSobrante()
    {
      string s = "";
      s += $"{TABBING}Informacion sobre dinero restante\n\n";
      double CantidadTotalRegular = RepartoRegular * NoPenalizados();
      double CantidadTotalPenalizados = Penalizados.Aggregate((double)0, (acc, p) => acc + p.CantidadFinal);
      double CantidadTotalReal = Custodio + CantidadTotalPenalizados + CantidadTotalRegular;
      s += $"Cantidad total a repartir para todos los trabajadores no penalizados: {CantidadTotalRegular}\n";
      s += $"Cantidad total a repartir para tods los trabajadores penalizados: {CantidadTotalPenalizados}\n";
      s += $"Cantidad a repartir al custodio: {Custodio}\n";
      s += $"Cantidad total a repartir: {CantidadTotalReal}\n";
      s += $"Cantidad restante: {CantidadInicial - CantidadTotalReal}\n";
      s += $"{TABBING}\n";
      return s;
    }

    public static void ParseAmmount(ref double Number)
    {
      Number *= 100;
      Number = Math.Round(Number);
      Number /= 100;
    }

    static int NoPenalizados() => TotalTrabajadores - TotalPenalizados;
  }
}

/*
 * 23450.50 
 * 80%
 * 50%
 */