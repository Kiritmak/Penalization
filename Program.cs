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
      for (int i = 0; i < Elementos.Length; i++)
      {
        string newElement = "";
        for (int j = 0; j < Elementos[i].Length; j++)
        {
          if (Elementos[i][j] == '*') continue;
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
      double CantidadBase = Program.RepartoInicial * 100 * (PorcentajePenalizado * 100);
      CantidadBase /= 10000;
      Program.ParseAmmount(ref CantidadBase);
      Program.CantidadTotalPenalizados *= 100;
      Program.CantidadTotalPenalizados += CantidadBase * 100;
      Program.CantidadTotalPenalizados /= 100;
      //Obteniendo ganancia del custodio
      Program.HandleDecimalRemoval(ref CantidadBase);

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
      string s = $"{Nombre} {PorcentajePenalizado * 100}% : {CantidadFinal}";
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
    public static double CantidadTotalPenalizados = 0;
    static double RestoDePenalizados = 0;
    static int TotalTrabajadores = 0;
    static int TotalPenalizados = 0;

    static string TABBING = "*****************************************************\n";

    public static void Main(string[] args)
    {
      Console.WriteLine($"{TABBING} Bienvenido a Penalization, Copyright © Kiritmak \n{TABBING}");

      Console.WriteLine("Ingrese la cantidad a repartir, asegurese de separar lugares decimales por puntos (.)\n");
      InputAmmount(out CantidadInicial);
      ParseAmmount(ref CantidadInicial);

      Console.WriteLine("\nIngrese la cantidad de trabajadores en el turno (Sin incluir al custodio)\n");
      InputAmmount(out TotalTrabajadores);
      RepartoInicial = CantidadInicial * 100 / TotalTrabajadores;
      RepartoInicial /= 100;
      ParseAmmount(ref RepartoInicial);

      Console.WriteLine("\nIngrese la cantidad de penalizados en el turno\n");
      InputAmmount(out TotalPenalizados);

      Console.WriteLine("\nAhora, ingrese la informacion requerida de cada trabajador penalizado, teniendo en cuenta el siguiente formato");
      Console.WriteLine(FormatoPenalizacion());

      //Recibiendo la data de los penalizados
      for (int i = 1; i <= TotalPenalizados; i++)
      {
        try
        {
          string? Input;
          Console.WriteLine($"Penalizado {i} :\n");
          Input = Console.ReadLine();
          Input += $",Penalizado al";
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
      RestoDePenalizados = RepartoInicial * 100 * TotalPenalizados - CantidadTotalPenalizados * 100;
      RestoDePenalizados /= 100;
      ParseAmmount(ref RestoDePenalizados);
      RepartoRegular = (((RestoDePenalizados * 100) / NoPenalizados()) / 100) * 100 + RepartoInicial * 100;
      RepartoRegular /= 100;
      ParseAmmount(ref RepartoRegular);

      //Calculando la ganancia del custodio
      for (int i = 0; i < NoPenalizados() - 1; i++)
      {
        double dummy = RepartoRegular;
        HandleDecimalRemoval(ref dummy);
      }
      HandleDecimalRemoval(ref RepartoRegular);
      ParseAmmount(ref Custodio);
      CantidadTotalPenalizados -= CantidadTotalPenalizados % 10;
      
      Console.WriteLine("Esto es solo informacion adicional, sientase libre de no leerla");
      DisplayProffits();
      Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");

      //Guardando la informacion y saliendo del programa
      string fecha = DateTime.Now.ToString("D");
      string directory = $@"Resumen\{fecha}.txt";
      List<string> Output = new List<string>();

      Output.Add($"Cantidad a repartir por trabajador regular : {RepartoRegular}");
      foreach (var p in Penalizados)
        Output.Add("Cantidad a repartir para el trabajador " + p.ToString());
      Output.Add($"Cantidad a repartir para el Custodio : {Custodio}");

      File.WriteAllLines(directory, Output.ToArray());

      Console.Write("Se ha generado un archivo con la fecha actual en la carpeta Resumen\nPulse enter para cerrar el programa");
      Console.Read();
    }

    static void InputAmmount<T>(out T Number) where T : IParsable<T>
    {
      string? Input = Console.ReadLine();
      while (Input == null || !T.TryParse(Input, null, out Number))
      {
        Console.WriteLine("El formato recivido es invalido");
        Input = Console.ReadLine();
      }
    }

    static string FormatoPenalizacion()
    {
      string Format = "PorcentajeEnBase100 (X%)\n";
      string Ejemplo = "Ejemplo: 20%\n";
      return Format + Ejemplo;
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
        s += $"{Penalizados[i - 1]}\n\n";
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

      double CantidadTotalRegular = RepartoRegular * 100 * NoPenalizados();
      CantidadTotalRegular /= 100;

      CantidadTotalPenalizados = Penalizados.Aggregate((double)0, (acc, p) => (acc * 100 + p.CantidadFinal * 100) / 100);
      ParseAmmount(ref CantidadTotalPenalizados);

      Custodio = CantidadInicial * 100 - CantidadTotalPenalizados * 100 - CantidadTotalRegular * 100;
      Custodio /= 100;
      Custodio = Math.Floor(Custodio);

      double CantidadTotalReal = Custodio + CantidadTotalPenalizados + CantidadTotalRegular;
      ParseAmmount(ref CantidadTotalReal);

      s += $"Cantidad total a repartir para todos los trabajadores no penalizados: {CantidadTotalRegular}\n";
      s += $"Cantidad total a repartir para todos los trabajadores penalizados: {CantidadTotalPenalizados}\n";
      s += $"Cantidad a repartir al custodio: {Custodio}\n";
      s += $"Cantidad total a repartir: {CantidadTotalReal}\n";
      s += $"Cantidad restante: {(CantidadInicial * 100 - CantidadTotalReal * 100) / 100}\n";
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
    public static void HandleDecimalRemoval(ref double WorkerAmmount)
    {
      WorkerAmmount *= 100;
      WorkerAmmount -= WorkerAmmount % 1000;
      WorkerAmmount /= 100;
    }
  }
}