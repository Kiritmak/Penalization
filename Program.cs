namespace Penalization
{
  public static class Program
  {
    static List<double> Penalizados = new List<double>();
    static double Regular = 0;
    static int Portero = 0;
    static double Sobrante = 0;
    static double CantidadInicial = 0;
    static int TotalPenalizados = 0;
    static double RepartoInicial = 0;

    static string TABBING = "\n*****************************************************\n";

    public static void Main(string[] args)
    {
      Console.WriteLine($"Bienvenido a Penalization, Copyright © Kiritmak {TABBING}");
      Console.WriteLine("Ingrese la cantidad a repartir, asegurese de separar lugares decimales por puntos (.)");
      InputAmmount(out CantidadInicial);
      Console.WriteLine(CantidadInicial);
    }

    static void InputAmmount(out double Number)
    {
      Number = 0;
      string? Input = Console.ReadLine();
      while (Input == null || !double.TryParse(Input, out Number)) 
      { 
        Console.WriteLine("El formato recivido es invalido");
        Input = Console.ReadLine();
      }
    }
  }
}
