namespace ProyectoFinal.View;

public partial class RadarPage : ContentPage
{
    private IDispatcherTimer timerGPS;
    private double brujulaActual = 0;
    private double rumboAlDestino = 0;
    
    // AQUÍ PONES LAS COORDENADAS DE TU DESTINO (Latitud, Longitud)
    // Ejemplo: Coordenadas genéricas, cámbialas por tu plaza o universidad más cercana
    private Location ubicacionDestino = new Location(-33.4489, -70.6693); 

    public RadarPage()
    {
        InitializeComponent();

        // Configuramos un temporizador para consultar el GPS cada 3 segundos
        // Esto ahorra batería y no satura la interfaz
        timerGPS = Application.Current.Dispatcher.CreateTimer();
        timerGPS.Interval = TimeSpan.FromSeconds(3);
        timerGPS.Tick += async (s, e) => await ActualizarGPS();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // 1. Pedir permisos
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status == PermissionStatus.Granted)
        {
            // 2. Iniciar sensores
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
        
        // Apagar sensores al salir para ahorrar memoria
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
                // Calcular distancia nativa en MAUI
                double distanciaMetros = Location.CalculateDistance(ubicacionActual, ubicacionDestino, DistanceUnits.Kilometers) * 1000;
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblDistancia.Text = $"{distanciaMetros:F0} m";
                });

                // Calcular el Rumbo (Matemática esférica)
                rumboAlDestino = CalcularRumbo(ubicacionActual, ubicacionDestino);
                ActualizarRotacionFlecha();
            }
        }
        catch (Exception)
        {
            // Evitar cuelgues si el GPS pierde señal
        }
    }

    private void ActualizarRotacionFlecha()
    {
        // La magia de la fusión de sensores: 
        // Rumbo del destino MENOS hacia dónde miro = Ángulo de la flecha
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