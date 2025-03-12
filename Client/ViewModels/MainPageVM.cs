using Client.Model;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client.ViewModels
{
    public class MainPageVM : INotifyPropertyChanged, IParameterReceiver
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<MyApp> Apps { get; }
        public ICommand OpenAddPageCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand OpenViewEditPageCommand { get; }
        public ICommand DownloadDataFromDBCommand { get; }
        public event Action CloseCurrentWindow;

        private MyApp? selectedApp;
        private HttpWrapper httpWrapper;
        private readonly INavigationService _navigationService;

        //Залогиненный пользователь
        private User? loginedUser;
        public string UserInfo
        { 
            get => loginedUser?.Login;
        }

        public MainPageVM(INavigationService navigationService)
        {
            httpWrapper = HttpWrapper.GetInstance();
            Apps = new ObservableCollection<MyApp>();
            OpenAddPageCommand = new RelayCommand(OpenAddPage);
            DeleteItemCommand = new RelayCommand(DeleteItem);
            OpenViewEditPageCommand = new RelayCommand(OpenViewEditPage);
            DownloadDataFromDBCommand = new RelayCommand(DownloadDataFromDataBase);
            _navigationService = navigationService;
            selectedApp = null;

            WeakReferenceMessenger.Default.Register<DataToPass>(this, (r, m) => 
            {
                //if (r is AddPageVM)
                //{
                //    OnNewAppCreated(m.MyApp);
                //}
                selectedApp = m.MyApp; 
            });
        }

        private void OpenAddPage()
        {
            //if (addWindow == null)
            //{
            //AddPage addPage = new AddPage();
            _navigationService.OpenWindow<AddPage>(loginedUser.Id, window =>
            {
                window.Width = 500;
                window.Height = 500;
            });

            //AddPageVM addPageVM = new AddPageVM(_user);

            //addPageVM.NewAppCreated += OnNewAppCreated;

            //addPage.BindingContext = addPageVM;

            //addWindow = new Window(addPage);
            //addWindow.Destroying += (s, e) => 
            //{ 
            //    addPageVM.NewAppCreated -= OnNewAppCreated;
            //    addWindow = null;
            //};
            //addWindow.Width = 500;
            //addWindow.Height = 500;

            //Application.Current?.OpenWindow(addWindow);
            //}
        }

        private void OpenViewEditPage()
        {
            //_navigationService.OpenWindow<LoginPage>(window =>
            //{
            //    window.Width = 500;
            //    window.Height = 500;
            //});

            CloseCurrentWindow?.Invoke();

            //if (viewEditWindow == null)
            //{
            //    ViewEditPage viewEditPage = new ViewEditPage();
            //    ViewEditPageVM viewEditPageVM = new ViewEditPageVM(selectedApp);

            //    viewEditPageVM.AppChanged += OnAppChanged;
            //    viewEditPage.BindingContext = viewEditPageVM;

            //    viewEditWindow = new Window(viewEditPage);
            //    viewEditWindow.Destroying += (s, e) =>
            //    {
            //        viewEditWindow = null;
            //    };
            //    viewEditWindow.Width = 500;
            //    viewEditWindow.Height = 500;
            //    Application.Current?.OpenWindow(viewEditWindow);
            //}
        }


        private async void DeleteItem()
        {
            if (selectedApp == null)
            {
                return;
            }

            string? token = await SecureStorage.GetAsync($"AccsessToken");
            httpWrapper.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            await httpWrapper.Delete(selectedApp.Id);
            Apps.Remove(selectedApp);
        }

        private async void DownloadDataFromDataBase()
        {
            if (Apps.Count == 0)
            {
                string? token = await SecureStorage.GetAsync($"AccsessToken");
                httpWrapper.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                List<MyApp>? listOfApps = await httpWrapper.Get();
                if (listOfApps != null)
                {
                    foreach (MyApp item in listOfApps)
                        Apps.Add(item);
                }
                else
                {
                    //СВЯЗАНО С MainPage.xaml.cs
                    WeakReferenceMessenger.Default.Send(new Message("В базе данных нет записей"), 0);
                }
            }
        }

        public void SetParameter(object parameter)
        {
            if (parameter is User user)
            {
                loginedUser = user;
                OnPropertyChanged("UserInfo");
            }
        }

        private void OnAppChanged(MyApp changedApp)
        {
            MyApp? tmp = Apps.FirstOrDefault(element => element.Id == changedApp.Id);
            if (tmp != null)
            { 
                tmp.Title = changedApp.Title;
                tmp.UserLogin = changedApp.UserLogin;
                tmp.UserPassword = changedApp.UserPassword;
            }
        }

        public void OnNewAppCreated(MyApp newApp) => Apps?.Add(newApp);

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                                      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
