using CommunityToolkit.Mvvm.Messaging;

namespace Client;

public partial class ViewEditPage : ContentPage
{
	public ViewEditPage()
	{
		InitializeComponent();
        WeakReferenceMessenger.Default.Register<Message,int>(this, 2, (r, m) => 
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