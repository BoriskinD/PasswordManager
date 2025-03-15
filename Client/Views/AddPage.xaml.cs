using Client.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Client;

public partial class AddPage : ContentPage
{
	public AddPage(AddPageVM addPageVM)
	{
		InitializeComponent();

		BindingContext = addPageVM;

		WeakReferenceMessenger.Default.Register<Message<string>,int>(this, (int)MessengerTokens.Tokens.AddPage, (recipient, message) => 
		{
            DisplayAlert("Инфо", message.Value, "Ок");
        });
    }
}