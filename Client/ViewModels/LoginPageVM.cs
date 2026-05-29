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

            User user = new User();
            user.Login = userLogin;

            using HttpResponseMessage getUserSaltResponse = await httpWrapper.GetUserSalt(user);
            {
                string content = await getUserSaltResponse.Content.ReadAsStringAsync();
                if (getUserSaltResponse.IsSuccessStatusCode)
                {
                    ServerResponse? serverResponse = JsonConvert.DeserializeObject<ServerResponse>(content);
                    user.AuthSalt = serverResponse.AuthSalt;
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new Message<string>($"Ошибка входа. {content}"), (int)MessengerTokens.Tokens.LoginPage);
                    return;
                }
            }

            //очистить старый токен
            SecureStorage.Remove("AccsessToken");

            user.PasswordHash = CryptoGraphicHelper.HashPassword(userPassword, user.AuthSalt);
            using HttpResponseMessage loginUserResponse = await httpWrapper.Login(user);
            {
                string content = await loginUserResponse.Content.ReadAsStringAsync();
                if (loginUserResponse.IsSuccessStatusCode)
                {
                    ServerResponse? serverResponse = JsonConvert.DeserializeObject<ServerResponse>(content);
                    user.Id = serverResponse.UserId;
                    user.EncryptionSalt = serverResponse.EncryptionSalt;

                    //Генерация мастер ключа
                    SecureSession.getInstance().Initialize(userPassword, user.EncryptionSalt);

                    await SecureStorage.SetAsync("AccsessToken", serverResponse.Token);

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
                    WeakReferenceMessenger.Default.Send(new Message<string>($"Ошибка входа. {content}"), (int)MessengerTokens.Tokens.LoginPage);
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
            newUser.AuthSalt = CryptoGraphicHelper.GenerateSalt();
            newUser.EncryptionSalt = CryptoGraphicHelper.GenerateSalt();
            newUser.PasswordHash = CryptoGraphicHelper.HashPassword(userPassword, newUser.AuthSalt);

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
