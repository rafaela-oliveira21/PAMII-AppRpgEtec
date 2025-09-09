using AppRpgEtec.ViewModels.Usuarios;

namespace AppRpgEtec.Views.Usuarios;

public partial class CadastroView : ContentPage
{
    UsuarioViewModel ViewModel;

    public CadastroView()
	{
		InitializeComponent();

		ViewModel = new UsuarioViewModel();
		BindingContext = ViewModel;
	}
}