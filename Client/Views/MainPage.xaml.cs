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
    
            WeakReferenceMessenger.Default.Register<Message<string>,int>(this, (int)MessengerTokens.Tokens.MainPage, (recipient, message) => 
            {
                DisplayAlert("Инфо", message.Value, "ОК");
            });
        }

        private void MainPageVM_CloseCurrentWindow()
        {
            _mainPageVM.CloseCurrentWindow -= MainPageVM_CloseCurrentWindow;
            Application.Current?.CloseWindow(Window);
        }

        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _mainPageVM.SelectedApp = e.CurrentSelection.FirstOrDefault() as MyApp;
        }
    }
}
