namespace Penalization
{
  public static class Program
  {
    static List<double> Penalizados = new List<double>();
    static double Regular = 0;
    static double Sobrante = 0;
    static double CantidadInicial = 0;
    static double RepartoInicial = 0;
    static double Portero = 0;
    static int TotalTrabajadores = 0;
    static int TotalPenalizados = 0;

    static string TABBING = "\n*****************************************************\n";

    public static void Main(string[] args)
    {
      Console.WriteLine($"{TABBING} Bienvenido a Penalization, Copyright © Kiritmak {TABBING}");

      Console.WriteLine("Ingrese la cantidad a repartir, asegurese de separar lugares decimales por puntos (.)\n");
      InputAmmount(out CantidadInicial);

      Console.WriteLine("\nIngrese la cantidad de trabajadores en el turno\n");
      InputAmmount(out TotalTrabajadores);

      Console.WriteLine("\nIngrese la cantidad de penalizados en el turno\n");
      InputAmmount(out TotalPenalizados);

      Console.WriteLine("\nAhora, ingrese la informacion requerida de cada trabajador penalizado, teniendo en cuenta el siguiente formato");
      Console.WriteLine(FormatoPenalizacion());

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
  }
}
