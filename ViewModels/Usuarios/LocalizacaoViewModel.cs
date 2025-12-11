using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppRpgEtec.Services.Usuarios;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;
using AppRpgEtec.Models;
using System.Collections.ObjectModel;


namespace AppRpgEtec.ViewModels.Usuarios
{
    public class LocalizacaoViewModel : BaseViewModel
    {
        private UsuarioService uService;
        public LocalizacaoViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            uService = new UsuarioService(token);
        }
        private Map meuMapa;

        public Map MeuMapa
        {
            get => meuMapa;
            set
            {
                if (value != meuMapa)
                {
                    meuMapa = value;
                    OnPropertyChanged(nameof(MeuMapa));
                }
            }
        }

        public async void InicializarMapa()
        {
            try
            {
                Location location = new Location(-23.5200241d, -46.596498d);

                Pin pinEtec = new Pin()
                {
                    Type = PinType.Place,
                    Label = "Etec Horacio",
                    Address = "Rua alcantara, 113, Vila Guilherme",
                    Location = location,
                };

                Map map = new Map();

                MapSpan mapSpan = MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(5));

                map.Pins.Add(pinEtec);

                map.MoveToRegion(mapSpan);

                MeuMapa = map;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        public async void ExibirUsuariosNoMapa()
        {
            try
            {
                ObservableCollection<Usuario> ocUsuarios = await uService.GetUsuariosAsync();
                List<Usuario> listaUsuarios = new List<Usuario>(ocUsuarios);
                Map map = new Map();

                foreach (Usuario u in listaUsuarios)
                {
                    if (u.Latitude != null && u.Longitude != null)
                    {
                        double latitude = (double)u.Latitude;
                        double longitude = (double)u.Longitude;
                        Location location = new Location(latitude, longitude);

                        Pin pinAtual = new Pin()
                        {
                            Type = PinType.Place,
                            Label = u.Username,
                            Address = $"E-mail: {u.Email}",
                            Location = location
                        };
                        map.Pins.Add(pinAtual);
                    }
                }
                MeuMapa = map;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", ex.Message, "OK");
            }
        }
    }
}