﻿using Client.Model;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;

namespace Client.ViewModels
{
    public class LoginPageVM
    {
        private string? userLogin, userPassword;
        private HttpWrapper httpWrapper;
        private readonly INavigationService _navigationService;

        //private Window? mainWindow;

        public event Action CloseCurrentWindow;
        public RelayCommand LoginCommand { get; }
        public RelayCommand RegisterCommand { get; }

        public string UserLogin
        {
            get => userLogin;
            set => userLogin = value;
        }

        public string UserPassword
        {
            get => userPassword;
            set => userPassword = value; 
        }

        public LoginPageVM(INavigationService navigationService)
        {
            httpWrapper = HttpWrapper.GetInstance();
            LoginCommand = new RelayCommand(LoginUser);
            RegisterCommand = new RelayCommand(RegisterNewUser);
            _navigationService = navigationService;
        }

        private async void LoginUser()
        {
            if (string.IsNullOrEmpty(UserLogin) || string.IsNullOrEmpty(UserPassword))
            {
                WeakReferenceMessenger.Default.Send(new Message("Не все поля заполнены!"), 3);
                return;
            }

            //очистить старый токен
            SecureStorage.Remove("AccsessToken");

            User user = new User()
            {
                Login = userLogin,
                PasswordHash = userPassword
            };

            using HttpResponseMessage response = await httpWrapper.Login(user);
            {
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    LoginResponse? loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                    user.Id = loginResponse.UserId;

                    await SecureStorage.SetAsync("AccsessToken", loginResponse.Token);

                    MainPage mainPage = new MainPage(user);
                    _navigationService.OpenWindow(window =>
                    {
                        window.Height = 700;
                        window.Width = 600;
                    }, mainPage);
                    //mainWindow = new Window(mainPage)
                    //{
                    //    Height = 700,
                    //    Width = 1000,
                    //};
                    //Application.Current?.OpenWindow(mainWindow);

                    CloseCurrentWindow?.Invoke();
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    WeakReferenceMessenger.Default.Send(new Message($"Не удалось войти. {responseContent}"), 3);
                }
            }
        }

        private async void RegisterNewUser() 
        {
            if (string.IsNullOrEmpty(userLogin) || string.IsNullOrEmpty(userPassword))
            {
                WeakReferenceMessenger.Default.Send(new Message("Не все поля заполнены!"), 3);
                return;
            }

            User newUser = new User()
            {
                Login = userLogin,
                PasswordHash = userPassword,
            };

            using HttpResponseMessage response = await httpWrapper.RegisterUser(newUser);
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    WeakReferenceMessenger.Default.Send(new Message(responseContent), 3);
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new Message($"Не далось зарегистрироваться. {responseContent}"), 3);
                }
            }
        }
    }
}
