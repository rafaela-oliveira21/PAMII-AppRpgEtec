using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Usuarios;
using AppRpgEtec.Views.Usuarios;
using AppRpgEtec.Views.Personagens;

namespace AppRpgEtec.ViewModels.Usuarios
{
    public class UsuarioViewModel : BaseViewModel
    {

        public UsuarioViewModel()
        {
            uService = new UsuarioService();
            InicializarCommand();
        }

        public void InicializarCommand()
        {
            AutenticarCommand = new Command(async() => await AutenticarUsuario());
            RegistrarCommand = new Command(async () => await RegistrarUsuario());
            DirecionarCadastroCommand = new Command(async() => await DirecionarParaCadastro());
        }

        private UsuarioService uService;

        public ICommand AutenticarCommand { get; set; }

        public ICommand RegistrarCommand { get; set; }
        public ICommand DirecionarCadastroCommand { get; set; }
        

        #region AtributosPropriedades
        private string login = string.Empty;
        private string senha = string.Empty;

        public string Login
        {
            get => login;
            set
            {
                login = value;
                OnPropertyChanged();
            }
        }
        public string Senha 
        {   get => senha;
            set
            {
                senha = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Metodos

        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;

        public async Task AutenticarUsuario()
        {
            try
            {
                Usuario u = new Usuario();
                u.Username = Login;
                u.PasswordString = Senha;

                Usuario uAutenticado = await uService.PostAutenticarUsuarioAsync(u);

                if(!string.IsNullOrEmpty(uAutenticado.Token))
                {
                    string mensagem = $"Bem Vindo(a) 👀🎉 {uAutenticado.Username}";

                    //Guardando dados para o futuro
                    Preferences.Set("UsuarioId", uAutenticado.Id);
                    Preferences.Set("UsuarioUsername", uAutenticado.Username);
                    Preferences.Set("UsuarioPerfil", uAutenticado.Perfil);
                    Preferences.Set("UsuarioToken", uAutenticado.Token);

                    //Inicio da coleta de Geolocalizacao atual para Atualizacao na API
                    _isCheckingLocation = true;
                    _cancelTokenSource = new CancellationTokenSource();
                    GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                    Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                    Usuario uLoc = new Usuario();
                    uLoc.Id = uAutenticado.Id;
                    uLoc.Latitude = location.Latitude;
                    uLoc.Longitude = location.Longitude;

                    UsuarioService uServiceLoc = new UsuarioService(uAutenticado.Token);
                    await uServiceLoc.PutAtualizarLocalizacaoAsync(uLoc);
                    //Fim da coleta de Geolocalização atual para atualizacao da API

                    await Application.Current.MainPage.DisplayAlert("Informação", mensagem, "OK");

                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Informação", "Dados INCORRETOS 😎🤦‍", "OK");
                }
            }
            catch (Exception ex)
            {

                await Application.Current.MainPage.DisplayAlert("Informação", ex.Message + ex.InnerException, "OK");
            }
        }

        public async Task RegistrarUsuario()
        {
            try
            {
                Usuario u = new Usuario();
                u.Username = Login;
                u.PasswordString = Senha;

                Usuario uRegistrado = await uService.PostRegistrarUsuarioAsync(u);

                if (uRegistrado.Id != 0)
                {
                    string mensagem = $"Usuário Id {uRegistrado.Id} registrado com sucesso";
                    await Application.Current.MainPage.DisplayAlert("Informção", mensagem, "OK");

                    await Application.Current.MainPage.Navigation.PopAsync(); //REMOVE A PAGINA DA PILHA DE VISUALIZAÇÃO
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Informação", ex.Message + "Detalhes: " + ex.InnerException, "OK");
            }

        }

        public async Task DirecionarParaCadastro() //METODO PARA EXIBIÇÃO DA VIEW DE CADASTRO
        {
            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new CadastroView());
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Informação", ex.Message + "Detalhes" + ex.InnerException, "OK");
            }
        }

        #endregion

        
    }
}
