using Client.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Client.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageVM loginPageVM)
	{
		InitializeComponent();
        BindingContext = loginPageVM;

        WeakReferenceMessenger.Default.Register<Message<string>,int>(this, (int)MessengerTokens.Tokens.LoginPage, (recipient, message) =>
        {
            if (message.CloseWindow)
            {
                Application.Current?.CloseWindow(Window);
                return;
            }

            DisplayAlert("Инфо", message.Value, "ОК");
        });
    }
}