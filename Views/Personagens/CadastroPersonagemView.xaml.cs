using AppRpgEtec.ViewModels.Personagens;

namespace AppRpgEtec.Views.Personagens;

public partial class CadastroPersonagemView : ContentPage
{
	private CadastroPersonagemViewModel cadViewModel;
	public CadastroPersonagemView()
	{
		InitializeComponent();

        cadViewModel = new CadastroPersonagemViewModel();
		BindingContext = cadViewModel;
		Title = "Novo Prsonagem";


    }

    private void InitializeComponent()
    {
        throw new NotImplementedException();
    }
}