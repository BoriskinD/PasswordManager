using Client.Model;
using Client.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Client
{
    public partial class MainPage : ContentPage
    {
        public MainPage(User user)
        {
            InitializeComponent();

            BindingContext = new MainPageVM(user);

            WeakReferenceMessenger.Default.Register<Message,int>(this, 0, (r, m) => 
            { 
                OnMessageReceived(m); 
            });
        }

        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyApp? selectedApp = e.CurrentSelection.FirstOrDefault() as MyApp;
            WeakReferenceMessenger.Default.Send(new DataToPass(selectedApp));
        }

        private async void OnMessageReceived(Message message)
        {
            await DisplayAlert("Инфо", message.Value, "ОК");
        }
    }
}
