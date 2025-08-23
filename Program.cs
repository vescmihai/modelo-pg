using System;

/// <summary>

/// </summary>
class Program
{
    static void Main(string[] args)
    {

        try
        {
            using var escena = new EscenaPC();
            escena.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al ejecutar la aplicación: {ex.Message}");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }

    }
}