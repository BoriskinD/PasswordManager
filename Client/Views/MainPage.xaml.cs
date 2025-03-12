using Client.Model;
using Client.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Client
{
    public partial class MainPage : ContentPage
    {
        private MainPageVM _mainPageVM;

        public MainPage(MainPageVM mainPageVM)
        {
            InitializeComponent();

            _mainPageVM = mainPageVM;
            _mainPageVM.CloseCurrentWindow += MainPageVM_CloseCurrentWindow;
            BindingContext = mainPageVM;
    
            WeakReferenceMessenger.Default.Register<Message,int>(this, 0, (r, m) => 
            { 
                OnMessageReceived(m); 
            });
        }

        private void MainPageVM_CloseCurrentWindow()
        {
            _mainPageVM.CloseCurrentWindow -= MainPageVM_CloseCurrentWindow;
            Application.Current?.CloseWindow(Window);
        }

        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyApp? selectedApp = e.CurrentSelection.FirstOrDefault() as MyApp;
            //Передаёт данные в MainPageVM
            WeakReferenceMessenger.Default.Send(new DataToPass(selectedApp));
        }

        private async void OnMessageReceived(Message message)
        {
            await DisplayAlert("Инфо", message.Value, "ОК");
        }
    }
}
