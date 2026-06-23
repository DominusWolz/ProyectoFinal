namespace ProyectoFinal.View; // <-- Corregido el "namespace"

public partial class BrujulaPage : ContentPage
{
    public BrujulaPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (Compass.Default.IsSupported)
        {
            if (!Compass.Default.IsMonitoring)
            {
                Compass.Default.ReadingChanged += Compass_ReadingChanged;
                Compass.Default.Start(SensorSpeed.UI);
            }
        }
        else
        {
            DisplayAlert("Error", "Tu dispositivo no tiene brújula integrada.", "OK");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
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
            double heading = e.Reading.HeadingMagneticNorth;
            lblGrados.Text = $"{heading:F0}°";

            // Ahora rotamos el "ContenedorAguja" completo
            ContenedorAguja.Rotation = 360 - heading;
        });
    }

    // --- ESTE ES EL MÉTODO QUE TE FALTABA PARA QUE EL BOTÓN FUNCIONE ---
    private async void OnVolverClicked(object sender, EventArgs e)
    {
        // Esta ruta obliga a la aplicación a regresar siempre a la pantalla de Inicio
        await Shell.Current.GoToAsync("///Inicio");
    }
}