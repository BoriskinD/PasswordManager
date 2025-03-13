using Client.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Client.Views;

public partial class LoginPage : ContentPage
{
    private LoginPageVM _loginPageVM;

	public LoginPage(LoginPageVM loginPageVM)
	{
		InitializeComponent();

        _loginPageVM = loginPageVM;
        _loginPageVM.CloseCurrentWindow += LoginPageVM_CloseCurrentWindow;

        BindingContext = loginPageVM;

        WeakReferenceMessenger.Default.Register<Message<string>,int>(this, (int)MessengerTokens.Tokens.LoginPage, (recipient, message) =>
        {
            DisplayAlert("Инфо", message.Value, "ОК");
        });
    }

    private void LoginPageVM_CloseCurrentWindow()
    {
        _loginPageVM.CloseCurrentWindow -= LoginPageVM_CloseCurrentWindow;
        Application.Current?.CloseWindow(Window);
    }
}