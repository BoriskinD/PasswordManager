using Client.Model;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;

namespace Client.ViewModels
{
    public class LoginPageVM
    {
        private string? userLogin, userPassword;
        private HttpWrapper httpWrapper;

        private Window? mainWindow;

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

        public LoginPageVM()
        {
            httpWrapper = HttpWrapper.GetInstance();
            LoginCommand = new RelayCommand(LoginUser);
            RegisterCommand = new RelayCommand(RegisterNewUser);
        }

        private async void LoginUser()
        {
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
                    mainWindow = new Window(mainPage)
                    {
                        Height = 500,
                        Width = 500,
                    };
                    Application.Current?.OpenWindow(mainWindow);
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new Message("Ошибка входа!"), 3);
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
                if (response.IsSuccessStatusCode)
                {
                    WeakReferenceMessenger.Default.Send(new Message("Регистрация прошла успешно!"), 3);
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new Message("Ошибка регистрации"), 3);
                }
            }
        }
    }
}
