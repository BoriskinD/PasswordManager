using Client.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Client.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();

        LoginPageVM loginPageVM = new LoginPageVM(new NavigationService());
        loginPageVM.CloseCurrentWindow += LoginPageVM_CloseCurrentWindow;

        BindingContext = loginPageVM;

        WeakReferenceMessenger.Default.Register<Message, int>(this, 3, (r, m) =>
        {
            OnMessageReceived(m);
        });
    }

    private void LoginPageVM_CloseCurrentWindow()
    {
        Application.Current?.CloseWindow(Window);
    }

    private async void OnMessageReceived(Message message)
    {
        if (message.IsSuccess)
        {
            await DisplayAlert("Успех", message.Value, "ОК");
        }
        else
        {
            await DisplayAlert("Ошибка", message.Value, "ОК");
        }
    }
}