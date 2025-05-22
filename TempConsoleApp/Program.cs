using System;
using FerreteriaPOS.Data;

namespace RecreateDatabaseApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Recreando la base de datos...");
                
                using (var context = new FerreteriaContext())
                {
                    // Recrear la base de datos
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    
                    Console.WriteLine("Base de datos recreada correctamente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
            
            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}
