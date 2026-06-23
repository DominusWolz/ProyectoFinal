namespace ProyectoFinal;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("BrujulaPage", typeof(View.BrujulaPage));
    }
}
