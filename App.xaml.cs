using AppRpgEtec.Views.Personagens;

namespace AppRpgEtec
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new Views.Usuarios.LoginView());

            Routing.RegisterRoute("cadPersonagemView", typeof(CadastroPersonagemView));
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new AppShell());
        //}
    }
}