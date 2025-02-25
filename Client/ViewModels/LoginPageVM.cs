using Client.Model;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Client.ViewModels
{
    public class LoginPageVM
    {
        private string? userLogin, userPassword;
        private HttpWrapper httpWrapper;

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
                Password = userPassword,
            };

            HttpResponseMessage response = await httpWrapper.RegisterUser(newUser);
            if (response.IsSuccessStatusCode)
            {
                WeakReferenceMessenger.Default.Send(new Message("Регистрация прошла успешно!"), 3);
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new Message("Ошибка регистрации!"), 3);
            }
        }
    }
}
