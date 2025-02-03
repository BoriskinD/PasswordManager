using Client.Model;
using CommunityToolkit.Mvvm.Input;

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

        private void LoginUser()
        { 
            
        }

        private void RegisterNewUser() 
        { 
        
        }
    }
}
