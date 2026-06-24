using Microsoft.Maui.Devices.Sensors;

namespace ProyectoFinal.View;

public partial class BuscadorPage : ContentPage
{
    public BuscadorPage()
    {
        InitializeComponent();
    }

    private async void OnBuscarClicked(object sender, EventArgs e)
    {
        string direccionTexto = txtBusqueda.Text;

        if (string.IsNullOrWhiteSpace(direccionTexto))
        {
            await DisplayAlert("Aviso", "Por favor, escribe una dirección.", "OK");
            return;
        }

        // Mostrar rueda de carga
        indicadorCarga.IsVisible = true;
        indicadorCarga.IsRunning = true;

        try
        {
            // Traducir texto a coordenadas GPS
            var ubicaciones = await Geocoding.Default.GetLocationsAsync(direccionTexto);
            var ubicacionEncontrada = ubicaciones?.FirstOrDefault();

            if (ubicacionEncontrada != null)
            {
                // Empaquetar y enviar al Radar
                var parametros = new Dictionary<string, object>
                {
                    { "NombreLugar", direccionTexto },
                    { "Coordenadas", new Location(ubicacionEncontrada.Latitude, ubicacionEncontrada.Longitude) }
                };

                txtBusqueda.Text = string.Empty;
                await Shell.Current.GoToAsync("RadarPage", parametros);
            }
            else
            {
                await DisplayAlert("No encontrado", "No pudimos localizar esa dirección.", "Reintentar");
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Asegúrate de tener internet para buscar.", "OK");
        }
        finally
        {
            indicadorCarga.IsVisible = false;
            indicadorCarga.IsRunning = false;
        }
    }
}