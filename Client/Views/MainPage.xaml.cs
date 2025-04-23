using Client.Model;
using Client.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Client
{
    public partial class MainPage : ContentPage
    {
        private MainPageVM _mainPageVM;
        private MyApp? selectedItem;
        private DateTime lastClickTime = DateTime.MinValue;
        private const int DoubleClickThresholdMs = 300;

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

        private void OnSingleClick(object sender, EventArgs e)
        { 
            DateTime currentTime = DateTime.Now;
            double timeSinceLastClick = (currentTime - lastClickTime).TotalMilliseconds;

            Grid? grid = sender as Grid; //Получить grid который был выбран в CollectionView
            selectedItem = grid.BindingContext as MyApp; //Получить объект MyApp из выбранного grid

            //Данная проверка нужна, чтобы избежать ситуации когда при совершении двойного клика 
            //<TapGestureRecognizer> считал что совершается сначала одинарный а потом двойной
            if (timeSinceLastClick > DoubleClickThresholdMs)
            { 
                _mainPageVM.SelectedApp = selectedItem;
            }

            lastClickTime = currentTime;
        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            //selectedItem заполняет в методе OnSingleClick потому что такой принцип работы <TapGestureRecognizer>
            //который вне зависимости от количества кликов вызывает методы обработчики кликов по очереди.
            //Есть вопросы по работе <TapGestureRecognizer> читай в инете!
            _mainPageVM.OpenViewEditPage(selectedItem);
        }

        private void MainPageVM_CloseCurrentWindow()
        {
            _mainPageVM.CloseCurrentWindow -= MainPageVM_CloseCurrentWindow;
            Application.Current?.CloseWindow(Window);
        }
    }
}
