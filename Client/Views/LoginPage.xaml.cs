using Client.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Client.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
        BindingContext = new LoginPageVM();

        WeakReferenceMessenger.Default.Register<Message, int>(this, 3, (r, m) =>
        {
            OnMessageReceived(m);
        });
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