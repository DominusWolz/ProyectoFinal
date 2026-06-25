namespace ProyectoFinal.View; // Namespace corregido que contiene la vista de la brújula

public partial class BrujulaPage : ContentPage
{
    public BrujulaPage()
    {
        InitializeComponent(); // Inicializa los componentes definidos en el XAML
    }

    // Este método se ejecuta cuando la página aparece en pantalla
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Verifica si el dispositivo tiene soporte para brújula
        if (Compass.Default.IsSupported)
        {
            // Si la brújula no está activa, la iniciamos
            if (!Compass.Default.IsMonitoring)
            {
                // Suscribimos el evento que se dispara cuando cambia la orientación
                Compass.Default.ReadingChanged += Compass_ReadingChanged;
                // Iniciamos el sensor con velocidad UI para mejor rendimiento visual
                Compass.Default.Start(SensorSpeed.UI);
            }
        }
        else
        {
            // Mensaje de error si el dispositivo no tiene brújula
            DisplayAlert("Error", "Tu dispositivo no tiene brújula integrada.", "OK");
        }
    }

    // Este método se ejecuta cuando la página desaparece
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        // Detenemos el sensor y liberamos el evento para ahorrar batería
        if (Compass.Default.IsSupported && Compass.Default.IsMonitoring)
        {
            Compass.Default.Stop(); // Detiene el monitoreo
            Compass.Default.ReadingChanged -= Compass_ReadingChanged; // Desuscribe el evento
        }
    }

    // Evento que se dispara cada vez que cambia la orientación de la brújula
    private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
    {
        // Actualizamos la UI en el hilo principal para evitar errores de threading
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Obtenemos el ángulo magnético del norte (0-360 grados)
            double heading = e.Reading.HeadingMagneticNorth;
            
            // Actualizamos el texto con el ángulo (formato sin decimales)
            lblGrados.Text = $"{heading:F0}°";

            // Rotamos la aguja completa restando el ángulo para que apunte al norte
            // 360 - heading: porque la rotación es en sentido horario
            ContenedorAguja.Rotation = 360 - heading;
        });
    }

    // Manejador del botón "Volver al Inicio"
    private async void OnVolverClicked(object sender, EventArgs e)
    {
        // Navega directamente a la página de Inicio usando Shell
        // El "///" fuerza la navegación absoluta desde la raíz
        await Shell.Current.GoToAsync("///Inicio");
    }
}