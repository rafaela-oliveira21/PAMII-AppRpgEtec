using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Disputas;
using AppRpgEtec.Services.PersonagemHabilidades;
using AppRpgEtec.Services.Personagens;
using AppRpgEtec.Services.Usuarios;


namespace AppRpgEtec.ViewModels.Disputas
{
    public class DisputaViewModel : BaseViewModel
    {
        private string narracao;
        public string Narracao { get; set; } = string.Empty;
        private PersonagemService pService;
        public ObservableCollection<Personagem> PersonagensEncontrados { get; set; }
        public Personagem Atacante { get; set; }
        public Personagem Oponente { get; set; }
        private DisputaService dService;
        public Disputa DisputaPersonagens { get; set; }
        private PersonagemHabilidadeService phService;
        public ObservableCollection<PersonagemHabilidade> Habilidades { get; set; }
        public DisputaViewModel() 
        {
            string Token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new PersonagemService(Token);
            dService = new DisputaService(Token);
            phService = new PersonagemHabilidadeService(Token);

            Atacante = new Personagem();
            Oponente = new Personagem();
            DisputaPersonagens = new Disputa();

            PersonagensEncontrados = new ObservableCollection<Personagem>();

            PesquisarPersonagensCommand = new Command<string>(async (string pesquisa) => { await PesquisarPersonagens(pesquisa); });
            DisputaComArmaCommand = new Command(async () => { await ExecutarDisputaArmada(); });
            DisputaComHabilidadeCommand = new Command(async () => { await ExecutarDisputaHabilidades(); });
            DisputaGeralCommand = new Command(async () => { await ExecutarDisputaGeral(); });
        }
        public ICommand PesquisarPersonagensCommand { get; set; }
        public ICommand DisputaComArmaCommand { get; set; }
        public ICommand DisputaComHabilidadeCommand { get; set; }
        public ICommand DisputaGeralCommand { get; set; }

        public string DescricaoPersonagemAtacante
        {
            get => Atacante.Nome;
        }
        public string DescricaoPersonagemOponente
        {
            get => Oponente.Nome;
        }

        private Personagem personagemSelecionado;

        public Personagem PersonagemSelecionado
        {
            set
            {
                if (value != null)
                {
                    personagemSelecionado = value;
                    SelecionarPersonagem(personagemSelecionado);
                    OnPropertyChanged();
                    PersonagensEncontrados.Clear();
                }
            }
        }

        private PersonagemHabilidade habilidadeSelecionada;
        public PersonagemHabilidade HabilidadeSelecionada
        {
            get { return habilidadeSelecionada; }
            set
            {
                if (value != null)
                {
                    try
                    {
                        habilidadeSelecionada = value;
                        OnPropertyChanged();
                    }
                    catch (Exception ex)
                    {
                        Application.Current.MainPage.DisplayAlert("Ops", ex.Message, "Ok");
                    }
                }
            }
        }

        private async Task ExecutarDisputaHabilidades()
        {
            try
            {
                DisputaPersonagens = new Disputa();
                DisputaPersonagens.Narracao = string.Empty;
                DisputaPersonagens.AtacanteId = Atacante.Id;
                DisputaPersonagens.OponenteId = Oponente.Id;
                DisputaPersonagens.HabilidadeId = habilidadeSelecionada.HabilidadeId;
                DisputaPersonagens = await dService.PostDisputaComHabilidadesAsync(DisputaPersonagens);

                await Application.Current.MainPage
                    .DisplayAlert("Resultado", DisputaPersonagens.Narracao, "Ok");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }



        public async Task PesquisarPersonagens(string textoPesquisaPersonagem)
        {
            try
            {
                PersonagensEncontrados = await pService.GetByNomeAproximadoAsync(textoPesquisaPersonagem);
                OnPropertyChanged(nameof(PersonagensEncontrados));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "OK");
            }
        }

        public async void SelecionarPersonagem(Personagem p)
        {
            try
            {
                string tipoCombatente = await Application.Current.MainPage
                    .DisplayActionSheet("Atacante ou Oponente?", "Cancelar", "", "Atacante", "Oponente");

                if (tipoCombatente == "Atacante")
                {
                    await this.ObterHabilidadesAsync(p.Id);
                    Atacante = p;
                    OnPropertyChanged(nameof(DescricaoPersonagemAtacante));
                }
                else if (tipoCombatente == "Oponente")
                {
                    Oponente = p;
                    OnPropertyChanged(nameof(DescricaoPersonagemOponente));
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        private string textoBuscaDigitado = string.Empty;

        public string TextoBuscaDigitado
        {
            get { return textoBuscaDigitado; }
            set
            {
                //Verifica se não é nulo, se não é vazio e se o tamanho do texto é maior que zero.
                if ((value != null && !string.IsNullOrEmpty(value) && value.Length > 0))
                {
                    textoBuscaDigitado = value;
                    _ = PesquisarPersonagens(textoBuscaDigitado);
                }
                else
                {
                    //Limpa o list view que exibe o resultado da pesquisa
                    PersonagensEncontrados.Clear();
                }
            }
        }
        private async Task ExecutarDisputaArmada()
        {
            try
            {
                DisputaPersonagens = new Disputa();
                DisputaPersonagens.AtacanteId = Atacante.Id;
                DisputaPersonagens.OponenteId = Oponente.Id;
                DisputaPersonagens.Narracao = Narracao;
                DisputaPersonagens = await dService.PostDisputaComArmaAsync(DisputaPersonagens);

                await Application.Current.MainPage
                    .DisplayAlert("Resultado", DisputaPersonagens.Narracao, "Ok");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async Task ObterHabilidadesAsync(int personagemId)
        {
            try
            {
                Habilidades = await phService.GetPersonagemHabilidadesAsync(personagemId);
                OnPropertyChanged(nameof(Habilidades));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        private async Task ExecutarDisputaGeral()
        {
            try
            {
     
                ObservableCollection<Personagem> lista = await pService.GetPersonagensAsync();
                DisputaPersonagens = new Disputa();
                DisputaPersonagens.Narracao = string.Empty;
                DisputaPersonagens.ListaIdPersonagens = lista.Select(x => x.Id).ToList();

               
                DisputaPersonagens = await dService.PostDisputaGeralAsync(DisputaPersonagens);

                
                string resultados = string.Join("|", DisputaPersonagens.Resultados);
                await Application.Current.MainPage
                    .DisplayAlert("Resultado", resultados, "Ok");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }




    }
}
