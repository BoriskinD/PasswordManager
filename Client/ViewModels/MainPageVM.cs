using Client.Model;
using Client.Views;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class MainPageVM : INotifyPropertyChanged, IParameterReceiver
    {
        public MyApp SelectedApp { get; set; }
        public ObservableCollection<MyApp> Apps { get; }
        public ICommand SetSelectedItemCommand { get; }
        public ICommand OpenAddPageCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand OpenViewEditPageCommand { get; }
        public ICommand DownloadDataFromDBCommand { get; }
        public ICommand BackToLoginPageCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private HttpWrapper httpWrapper;
        private readonly INavigationService _navigationService;

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
            DeleteItemCommand = new RelayCommand(DeleteSelectedItem);
            SetSelectedItemCommand = new RelayCommand<MyApp>(SetSelectedItem);
            OpenViewEditPageCommand = new RelayCommand<MyApp>(OpenViewEditPage);
            DownloadDataFromDBCommand = new RelayCommand(DownloadItemsFromDB);
            BackToLoginPageCommand = new RelayCommand(BackToLoginPage);
            _navigationService = navigationService;

            WeakReferenceMessenger.Default.Register<Message<MyApp>, int>(this, (int)MessengerTokens.Tokens.MainPageVM, (recipient, message) =>
            {
                //Данные пришли из окна редактирования сервиса
                if (message.Sender is ViewEditPageVM)
                {
                    OnAppChanged(message.Value);
                    return;
                }

                //Данные пришли из окна добавления сервиса
                Apps?.Add(message.Value);
            });
        }

        private void OpenAddPage()
        {
            _navigationService.OpenWindow<AddPage>(window =>
            {
                window.Width = 400;
                window.Height = 500;
            }, loginedUser.Id);
        }

        private void SetSelectedItem(MyApp selectedApp)
        {
            SelectedApp = selectedApp;
        }

        private void OpenViewEditPage(MyApp selectedApp)
        {
            _navigationService.OpenWindow<ViewEditPage>(window =>
            {
                window.Width = 500;
                window.Height = 500;
            }, selectedApp);
        }

        private async void DeleteSelectedItem()
        {
            if (SelectedApp == null)
            {
                return;
            }

            string? token = await SecureStorage.GetAsync($"AccsessToken");
            httpWrapper.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            await httpWrapper.Delete(SelectedApp.Id);    
            Apps.Remove(SelectedApp);
        }

        private async void DownloadItemsFromDB()
        {
            Apps.Clear();

            string? token = await SecureStorage.GetAsync($"AccsessToken");
            httpWrapper.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            List<MyApp>? listOfApps = await httpWrapper.Get();
            if (listOfApps != null)
            {
                foreach (MyApp item in listOfApps)
                {
                    Apps.Add(item);
                }
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new Message<string>("В базе данных нет записей"), (int)MessengerTokens.Tokens.MainPage);
            }
        }

        private void BackToLoginPage()
        {
            _navigationService.OpenWindow<LoginPage>(window =>
            { 
                window.Title = "PWDManager";
                window.Height = 200;
                window.Width = 450;
            });

            WeakReferenceMessenger.Default.Send(new Message<string>(string.Empty, true), (int)MessengerTokens.Tokens.MainPage);
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

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                                      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
