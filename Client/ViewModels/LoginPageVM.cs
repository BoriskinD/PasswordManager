using Client.Model;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client.ViewModels
{
    public class LoginPageVM : INotifyPropertyChanged
    {
        private string? userLogin;
        private string? userPassword;
        private bool isEntryPassword;
        private bool isShowPassword;
        private HttpWrapper httpWrapper;
        private CryptoGraphicHelper cryptographicHelper;
        private readonly INavigationService _navigationService;

        public RelayCommand LoginCommand { get; }
        public RelayCommand RegisterCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsEntryPassword
        {
            get => isEntryPassword;
            set 
            {
                isEntryPassword = value;
                OnPropertyChanged();
            }
        }

        public bool IsShowPassword
        {
            get => isShowPassword;
            set 
            {
                isShowPassword = value;
                IsEntryPassword = !isShowPassword;
            }
        }

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
            cryptographicHelper = new CryptoGraphicHelper();
            httpWrapper = HttpWrapper.GetInstance();
            LoginCommand = new RelayCommand(LoginUser);
            RegisterCommand = new RelayCommand(RegisterNewUser);
            _navigationService = navigationService;

            IsEntryPassword = true;
        }

        private async void LoginUser()
        {
            if (string.IsNullOrEmpty(UserLogin) || string.IsNullOrEmpty(UserPassword))
            {
                WeakReferenceMessenger.Default.Send(new Message<string>("Не все поля заполнены!"), (int)MessengerTokens.Tokens.LoginPage);
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

                    _navigationService.OpenWindow<MainPage>(window =>
                    {
                        window.Title = "PWDManager";
                        window.Height = 800;
                        window.Width = 1300;
                    }, user);

                    //Закрыть окно авторизации
                    WeakReferenceMessenger.Default.Send(new Message<string>(string.Empty, true), (int)MessengerTokens.Tokens.LoginPage);
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    WeakReferenceMessenger.Default.Send(new Message<string>($"Не удалось войти. {responseContent}"), (int)MessengerTokens.Tokens.LoginPage);
                }
            }
        }

        private async void RegisterNewUser() 
        {
            if (string.IsNullOrEmpty(userLogin) || string.IsNullOrEmpty(userPassword))
            {
                WeakReferenceMessenger.Default.Send(new Message<string>("Не все поля заполнены!"), (int)MessengerTokens.Tokens.LoginPage);
                return;
            }

            User newUser = new();
            newUser.Login = userLogin;
            newUser.Salt = cryptographicHelper.GenerateSalt();
            newUser.PasswordHash = cryptographicHelper.HashPassword(userPassword, newUser.Salt);

            using HttpResponseMessage response = await httpWrapper.RegisterUser(newUser);
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    WeakReferenceMessenger.Default.Send(new Message<string>(responseContent), (int)MessengerTokens.Tokens.LoginPage);
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new Message<string>($"Не далось зарегистрироваться. {responseContent}"), (int)MessengerTokens.Tokens.LoginPage);
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                                      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
