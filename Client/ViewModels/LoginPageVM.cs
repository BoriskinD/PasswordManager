using Client.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Client.ViewModels
{
    public partial class LoginPageVM : ObservableObject, INotifyPropertyChanged
    {
        [ObservableProperty] private bool _isEntryPassword;
        [ObservableProperty] private bool _isShowPassword;
        [ObservableProperty] private string _userLogin;
        [ObservableProperty] private string _userPassword;

        private HttpWrapper httpWrapper;
        private readonly INavigationService _navigationService;

        public LoginPageVM(INavigationService navigationService)
        {
            httpWrapper = HttpWrapper.GetInstance();
            _navigationService = navigationService;

            IsEntryPassword = true;
        }

        //хук метод свойства IsShowPassword
        partial void OnIsShowPasswordChanged(bool value)
        {
            IsEntryPassword = !value;
        }

        [RelayCommand]
        private async Task LoginUser()
        {
            if (string.IsNullOrEmpty(UserLogin) || string.IsNullOrEmpty(UserPassword))
            {
                WeakReferenceMessenger.Default.Send(new Message<string>("Не все поля заполнены!"), (int)MessengerTokens.Tokens.LoginPage);
                return;
            }

            User user = new User();
            user.Login = UserLogin;

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

            user.PasswordHash = CryptoGraphicHelper.HashPassword(UserPassword, user.AuthSalt);
            using HttpResponseMessage loginUserResponse = await httpWrapper.Login(user);
            {
                string content = await loginUserResponse.Content.ReadAsStringAsync();
                if (loginUserResponse.IsSuccessStatusCode)
                {
                    ServerResponse? serverResponse = JsonConvert.DeserializeObject<ServerResponse>(content);
                    user.Id = serverResponse.UserId;
                    user.EncryptionSalt = serverResponse.EncryptionSalt;

                    //Генерация мастер ключа
                    SecureSession.getInstance().Initialize(UserPassword, user.EncryptionSalt);

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
                else { WeakReferenceMessenger.Default.Send(new Message<string>($"Ошибка входа. {content}"),
                                                           (int)MessengerTokens.Tokens.LoginPage);}
            }
        }

        [RelayCommand]
        private async Task RegisterNewUser() 
        {
            if (string.IsNullOrEmpty(UserLogin) || string.IsNullOrEmpty(UserPassword))
            {
                WeakReferenceMessenger.Default.Send(new Message<string>("Не все поля заполнены!"), (int)MessengerTokens.Tokens.LoginPage);
                return;
            }

            User newUser = new();
            newUser.Login = UserLogin;
            newUser.AuthSalt = CryptoGraphicHelper.GenerateSalt();
            newUser.EncryptionSalt = CryptoGraphicHelper.GenerateSalt();
            newUser.PasswordHash = CryptoGraphicHelper.HashPassword(UserPassword, newUser.AuthSalt);

            using HttpResponseMessage response = await httpWrapper.RegisterUser(newUser);
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    WeakReferenceMessenger.Default.Send(new Message<string>(responseContent), (int)MessengerTokens.Tokens.LoginPage);
                else
                    WeakReferenceMessenger.Default.Send(new Message<string>($"Не далось зарегистрироваться. {responseContent}"),
                                                        (int)MessengerTokens.Tokens.LoginPage);
            }
        }
    }
}
