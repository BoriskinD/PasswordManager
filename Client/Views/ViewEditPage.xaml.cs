using Client.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Client;

public partial class ViewEditPage : ContentPage
{
	public ViewEditPage(ViewEditPageVM viewEditPageVM)
	{
		InitializeComponent();
        BindingContext = viewEditPageVM;

        WeakReferenceMessenger.Default.Register<Message<string>,int>(this, (int)MessengerTokens.Tokens.ViewEditPage, (recipient, message) => 
        {
            DisplayAlert("Инфо", message.Value, "ОК");
        });
    }
}