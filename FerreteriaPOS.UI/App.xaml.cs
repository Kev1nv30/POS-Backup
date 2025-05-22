using System;
using System.IO;
using System.Windows;
using FerreteriaPOS.Data;
using FerreteriaPOS.Data.Services;
using FerreteriaPOS.UI.Windows;

namespace FerreteriaPOS.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            // Force recreate the database one time to apply schema changes
            bool recreateDb = false;
            
            #if DEBUG
            // Database recreation prompt disabled to prevent annoying users on every startup
            // To enable this again for development purposes, uncomment the following code:
            /*
            var result = MessageBox.Show(
                "¿Desea recrear la base de datos? Esto eliminará todos los datos existentes.",
                "Recrear Base de Datos",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            
            recreateDb = (result == MessageBoxResult.Yes);
            */
            #endif
            
            // Inicializar la base de datos
            try
            {
                using (var context = new FerreteriaContext())
                {
                    // Ejecutamos el seeder para datos iniciales, indicando si debe recrear la BD
                    FerreteriaPOS.Data.Services.DbSeeder.Seed(context, recreateDb);
                }
            }
            catch (Exception dbEx)
            {
                MessageBox.Show(
                    $"Error al inicializar la base de datos: {dbEx.Message}\n\nVerifique que los archivos .xlsx están disponibles y tienen el formato correcto.",
                    "Error de Base de Datos",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                
                throw; // Propagar la excepción para salir de la aplicación si la BD no se puede inicializar
            }
            
            // Creamos y mostramos la ventana principal (MainWindow)
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al iniciar la aplicación: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            
            // Terminar la aplicación de forma ordenada
            Current.Shutdown();
        }
    }
}

