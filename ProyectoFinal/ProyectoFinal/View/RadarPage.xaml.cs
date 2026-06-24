namespace ProyectoFinal.View;

// Estas etiquetas le dicen a la página que esté lista para recibir datos
[QueryProperty(nameof(Coordenadas), "Coordenadas")]
[QueryProperty(nameof(NombreLugar), "NombreLugar")]
public partial class RadarPage : ContentPage
{
    private IDispatcherTimer timerGPS;
    private double brujulaActual = 0;
    private double rumboAlDestino = 0;

    // Variables dinámicas (ya no están fijas)
    private Location ubicacionDestino;
    private string nombreLugarDestino;

    // Propiedad que recibe las coordenadas
    public Location Coordenadas
    {
        set { ubicacionDestino = value; }
    }

    // Propiedad que recibe el nombre
    public string NombreLugar
    {
        set { nombreLugarDestino = value; }
    }

    public RadarPage()
    {
        InitializeComponent();

        timerGPS = Application.Current.Dispatcher.CreateTimer();
        timerGPS.Interval = TimeSpan.FromSeconds(3);
        timerGPS.Tick += async (s, e) => await ActualizarGPS();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Actualizamos la pantalla con el texto del botón que presionó el usuario
        if (!string.IsNullOrEmpty(nombreLugarDestino))
        {
            lblNombreDestino.Text = $"Distancia hacia {nombreLugarDestino}";
        }

        // Si hay algún fallo, por seguridad ponemos una coordenada por defecto
        if (ubicacionDestino == null)
        {
            ubicacionDestino = new Location(-33.4489, -70.6693);
        }

        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status == PermissionStatus.Granted)
        {
            timerGPS.Start();

            if (Compass.Default.IsSupported && !Compass.Default.IsMonitoring)
            {
                Compass.Default.ReadingChanged += Compass_ReadingChanged;
                Compass.Default.Start(SensorSpeed.UI);
            }
        }
        else
        {
            await DisplayAlert("Error", "Necesitamos el GPS para guiarte.", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        timerGPS.Stop();
        if (Compass.Default.IsSupported && Compass.Default.IsMonitoring)
        {
            Compass.Default.Stop();
            Compass.Default.ReadingChanged -= Compass_ReadingChanged;
        }
    }

    private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            brujulaActual = e.Reading.HeadingMagneticNorth;
            ActualizarRotacionFlecha();
        });
    }

    private async Task ActualizarGPS()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(2));
            var ubicacionActual = await Geolocation.Default.GetLocationAsync(request);

            if (ubicacionActual != null)
            {
                double distanciaMetros = Location.CalculateDistance(ubicacionActual, ubicacionDestino, DistanceUnits.Kilometers) * 1000;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblDistancia.Text = $"{distanciaMetros:F0} m";
                });

                rumboAlDestino = CalcularRumbo(ubicacionActual, ubicacionDestino);
                ActualizarRotacionFlecha();
            }
        }
        catch (Exception)
        {
            // Silenciado temporalmente si hay cortes de señal
        }
    }

    private void ActualizarRotacionFlecha()
    {
        double rotacionFinal = rumboAlDestino - brujulaActual;
        FlechaRadar.Rotation = rotacionFinal;
    }

    private double CalcularRumbo(Location inicio, Location destino)
    {
        double lat1 = inicio.Latitude * Math.PI / 180.0;
        double lon1 = inicio.Longitude * Math.PI / 180.0;
        double lat2 = destino.Latitude * Math.PI / 180.0;
        double lon2 = destino.Longitude * Math.PI / 180.0;

        double dLon = lon2 - lon1;

        double y = Math.Sin(dLon) * Math.Cos(lat2);
        double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);

        double rumbo = Math.Atan2(y, x) * 180.0 / Math.PI;
        return (rumbo + 360) % 360;
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Inicio");
    }
}