using Client.Model;
using Client.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace Client.ViewModels
{
    public partial class MainPageVM : ObservableObject, IParameterReceiver
    {
        [ObservableProperty]
        private string? _userInfo;

        public MyApp? SelectedApp { get; set; }
        public ObservableCollection<MyApp> Apps { get; }
        private HttpWrapper httpWrapper;
        private User? loginedUser;
        private readonly INavigationService _navigationService;
        public static string defaultImage = "no_image_available.jpg";

        public MainPageVM(INavigationService navigationService)
        {
            httpWrapper = HttpWrapper.GetInstance();
            Apps = new ObservableCollection<MyApp>();
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

        [RelayCommand]
        private void OpenAddPage()
        {
            _navigationService.OpenWindow<AddPage>(window =>
            {
                window.Width = 400;
                window.Height = 500;
            }, loginedUser, null); //<--- именованый параметр
        }

        [RelayCommand]
        private void OpenViewEditPage(MyApp selectedApp)
        {
            _navigationService.OpenWindow<ViewEditPage>(window =>
            {
                window.Width = 400;
                window.Height = 560;
            }, selectedApp, loginedUser);
        }

        [RelayCommand]
        private void BackToLoginPage()
        {
            _navigationService.OpenWindow<LoginPage>(window =>
            {
                window.Title = "PWDManager";
                window.Height = 250;
                window.Width = 450;
            });

            SecureSession.getInstance().Clear();

            //Закрыть все дочерние окна
            WeakReferenceMessenger.Default.Send(new Message<string>(string.Empty, true), (int)MessengerTokens.Tokens.MainPage);
            WeakReferenceMessenger.Default.Send(new Message<string>(string.Empty, true), (int)MessengerTokens.Tokens.ViewEditPage);
            WeakReferenceMessenger.Default.Send(new Message<string>(string.Empty, true), (int)MessengerTokens.Tokens.AddPage);
        }

        [RelayCommand]
        private async Task DeleteSelectedItem()
        {
            if (SelectedApp == null)
            {
                return;
            }

            string? token = await SecureStorage.GetAsync($"AccsessToken");
            httpWrapper.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            if (File.Exists(SelectedApp.ImagePath))
            {
                File.Delete(SelectedApp.ImagePath);
            }
            
            await httpWrapper.Delete(SelectedApp.Id);
            Apps.Remove(SelectedApp);
        }

        [RelayCommand]
        private async Task DownloadItemsFromDB()
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

        [RelayCommand]
        private void SetSelectedItem(MyApp selectedApp)
        {
            SelectedApp = selectedApp;
        }

        public void SetParameter(object parameter1, object parameter2)
        {
            if (parameter1 is User user)
            {
                loginedUser = user;
                UserInfo = loginedUser.Login;
            }
        }

        private void OnAppChanged(MyApp changedApp)
        {
            MyApp? tmp = Apps.FirstOrDefault(element => element.Id == changedApp.Id);
            if (tmp != null)
            {
                Apps.Remove(tmp);
                Apps.Add(changedApp);
            }
        }
    }
}
