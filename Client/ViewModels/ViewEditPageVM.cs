using Client.Model;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class ViewEditPageVM : INotifyPropertyChanged
    {
        private HttpWrapper httpWrapper;
        private int selectedAppId;
        private string? title, userLogin, userPassword, imagePath;
        private bool isEditAllowed, isTitleEnabled, isUserLoginEnabled, isUserPasswordEnabled;

        public delegate void NewAppCreatedHandler(MyApp changedApp);
        public event NewAppCreatedHandler? AppChanged;
        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand SaveChangesCommand { get; }

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

        public ViewEditPageVM(MyApp selectedApp)
        {
            httpWrapper = HttpWrapper.GetInstance();
            SaveChangesCommand = new RelayCommand(SaveChanges);
            IsTitleEnabled = false;
            IsUserLoginEnabled = false;
            IsUserPasswordEnabled = false;
            IsEditAllowed = false;

            selectedAppId = selectedApp == null ? 0 : selectedApp.Id;

            #pragma warning disable CS8601 // Possible null reference assignment.
            Title = selectedApp?.Title;
            UserLogin = selectedApp?.UserLogin;
            UserPassword = selectedApp?.UserPassword;
            ImagePath = selectedApp?.ImagePath;
            #pragma warning restore CS8601 // Possible null reference assignment.
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
                    //СВЯЗАНО С ViewEditPage.xaml.cs
                    WeakReferenceMessenger.Default.Send(new Message("Данные изменены", true), 2);
                    AppChanged?.Invoke(changedApp);
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new Message("Не удалось изменить данные"), 2);
                }
            }  
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                                      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
