using Client.Model;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class ViewEditPageVM : INotifyPropertyChanged, IParameterReceiver
    {
        private HttpWrapper httpWrapper;
        private int selectedAppId;
        private string? title, userLogin, userPassword, imagePath;
        private bool isEditAllowed, isTitleEnabled, isUserLoginEnabled, isUserPasswordEnabled;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand SaveChangesCommand { get; }

        public bool IsTitleEnabled
        { 
            get => isTitleEnabled;
            set 
            {
                isTitleEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsUserLoginEnabled
        {
            get => isUserLoginEnabled;
            set
            {
                isUserLoginEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsUserPasswordEnabled
        {
            get => isUserPasswordEnabled;
            set
            {
                isUserPasswordEnabled = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => title;
            set 
            {
                title = value;
                OnPropertyChanged();
            }
        }

        public string UserLogin
        {
            get => userLogin;
            set 
            {
                userLogin = value;
                OnPropertyChanged();
            }
        }

        public string UserPassword
        {
            get => userPassword;
            set
            {
                userPassword = value;
                OnPropertyChanged();
            }
        }

        public string ImagePath
        {
            get => imagePath;
            set
            {
                imagePath = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditAllowed
        {
            get => isEditAllowed;
            set
            {
                isEditAllowed = value;
                OnPropertyChanged();

                if (isEditAllowed)
                {
                    IsTitleEnabled = true;
                    IsUserLoginEnabled = true;
                    IsUserPasswordEnabled = true;
                }
                else
                {
                    IsTitleEnabled = false;
                    IsUserLoginEnabled = false;
                    IsUserPasswordEnabled = false;

                }
            }
        }

        public ViewEditPageVM()
        {
            httpWrapper = HttpWrapper.GetInstance();
            SaveChangesCommand = new RelayCommand(SaveChanges);

            IsTitleEnabled = false;
            IsUserLoginEnabled = false;
            IsUserPasswordEnabled = false;
            IsEditAllowed = false;
        }

        private async void SaveChanges()
        {
            MyApp changedApp = new MyApp()
            {
                Id = selectedAppId,
                Title = Title,
                UserLogin = UserLogin,
                UserPassword = UserPassword,
                ImagePath = ImagePath
            };

            using HttpResponseMessage response = await httpWrapper.Put(changedApp);
            {
                if (response.IsSuccessStatusCode)
                {
                    WeakReferenceMessenger.Default.Send(new Message<MyApp>(changedApp, false, this), (int)MessengerTokens.Tokens.MainPageVM);
                    WeakReferenceMessenger.Default.Send(new Message<string>("Данные изменены"), (int)MessengerTokens.Tokens.ViewEditPage);
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new Message<string>("Не удалось изменить данные"), (int)MessengerTokens.Tokens.ViewEditPage);
                }
            }  
        }

        public void SetParameter(object parameter)
        {
            if (parameter is MyApp myApp)
            { 
                selectedAppId = myApp.Id;
                Title = myApp.Title;
                UserLogin = myApp.UserLogin;
                UserPassword = myApp.UserPassword;
                ImagePath = myApp.ImagePath;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                                      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
