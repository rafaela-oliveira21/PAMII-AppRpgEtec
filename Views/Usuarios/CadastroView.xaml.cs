using AndroidX.Lifecycle;
using AppRpgEtec.Models;
using AppRpgEtec.ViewModels.Usuarios;

namespace AppRpgEtec.Views.Usuarios;

public partial class CadastroView : ContentPage
{
	UsuarioViewModel viewModel;

	public CadastroView()
	{
		InitializeComponent();

		ViewModel = new UsuarioViewModel();
		BindingContext = ViewModel;
	}
}