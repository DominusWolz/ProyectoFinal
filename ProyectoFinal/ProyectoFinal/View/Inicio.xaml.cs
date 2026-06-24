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
    private async void OnRadarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("RadarPage");
    }
}