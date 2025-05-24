namespace Penalization
{
  public interface IPenalizado
  {
    double CalcularPenalizacion();
    void ValidarFormato(string[] args);
  }

  public class Penalizado : IPenalizado
  {
    public Penalizado(string? Formato = "10%")
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
    }

    double PorcentajePenalizado {  get; set; }
    public double CantidadFinal { get => CalcularPenalizacion(); }

    public double CalcularPenalizacion()
    {
      return Program.RepartoInicial * PorcentajePenalizado;
    }

    public void ValidarFormato(string[] Formato)
    {
      //Separa el numero seguido de la X en el Formato (X%)
      PorcentajePenalizado = double.Parse(Formato[0].Substring(0, Formato[0].IndexOf('%')));
      PorcentajePenalizado /= 100;
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
    static double Regular = 0;
    static double Sobrante = 0;
    static double CantidadInicial = 0;
    static double Portero = 0;
    public static double RepartoInicial = 0;
    static int TotalTrabajadores = 0;
    static int TotalPenalizados = 0;

    static string TABBING = "*****************************************************\n";

    public static void Main(string[] args)
    {
      Penalizado penalizado = new Penalizado();
      Console.WriteLine($"{TABBING} Bienvenido a Penalization, Copyright © Kiritmak \n{TABBING}");

      Console.WriteLine("Ingrese la cantidad a repartir, asegurese de separar lugares decimales por puntos (.)\n");
      InputAmmount(out CantidadInicial);

      Console.WriteLine("\nIngrese la cantidad de trabajadores en el turno (Sin incluir al portero)\n");
      InputAmmount(out TotalTrabajadores);
      RepartoInicial = CantidadInicial / TotalTrabajadores;

      Console.WriteLine("\nIngrese la cantidad de penalizados en el turno\n");
      InputAmmount(out TotalPenalizados);

      Console.WriteLine("\nAhora, ingrese la informacion requerida de cada trabajador penalizado, teniendo en cuenta el siguiente formato");
      Console.WriteLine(FormatoPenalizacion());

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

    static void DisplayPenalizados()
    {
      Console.WriteLine($"{TABBING}Informacion sobre los penalizados\n");
      foreach (var p in Penalizados)
      {
        Console.WriteLine(p + "\n");
      }
      Console.Write($"{TABBING}");
    }
  }
}
