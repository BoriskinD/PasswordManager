using Client.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Client
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageVM mainPageVM)
        {
            InitializeComponent();
            BindingContext = mainPageVM;
    
            WeakReferenceMessenger.Default.Register<Message<string>,int>(this, (int)MessengerTokens.Tokens.MainPage, (recipient, message) => 
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
}
