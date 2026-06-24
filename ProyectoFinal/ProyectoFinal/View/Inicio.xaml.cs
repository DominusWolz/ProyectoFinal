using Microsoft.Maui.Controls;

namespace ProyectoFinal.View;

public partial class Inicio : ContentPage
{
    public Inicio()
    {
        InitializeComponent();
    }

    private async void OnComenzarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("BrujulaPage");
    }
    private async void OnCasaAbueloClicked(object sender, EventArgs e)
    {
        // Empaquetamos la información del destino
        var parametros = new Dictionary<string, object>
        {
            { "NombreLugar", "Jumbo" },
            // Reemplaza los números de abajo por la latitud y longitud real de la casa
            { "Coordenadas", new Location(-41.4685450, -72.9321730) }
        };

        // Enviamos al usuario a la página de Radar pasándole los parámetros
        await Shell.Current.GoToAsync("RadarPage", parametros);
    }

    private async void OnUniversidadClicked(object sender, EventArgs e)
    {
        var parametros = new Dictionary<string, object>
        {
            { "NombreLugar", "la Facultad de Ingeniería" },
            // Reemplaza estos números por la latitud y longitud real de tu universidad
            { "Coordenadas", new Location(-41.472593, -72.928829) }
        };

        await Shell.Current.GoToAsync("RadarPage", parametros);
    }
    private async void OnAbrirBuscadorClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("BuscadorPage");
    }
}