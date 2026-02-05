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
        string resizedImage;
        string newImagePath;
        string oldImage;
        private string? title;
        private string? userLogin;
        private string? userPassword;
        private string? imagePath;
        private bool isEditAllowed;
        private bool isTitleEnabled;
        private bool isUserLoginEnabled;
        private bool isUserPasswordEnabled;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand SaveChangesCommand { get; }
        public ICommand SelectImageCommand { get; }

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
            SelectImageCommand = new RelayCommand(SelectImage);

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
                ImagePath = string.IsNullOrEmpty(newImagePath) ? ImagePath : newImagePath
            };

            using HttpResponseMessage response = await httpWrapper.Put(changedApp);
            {
                if (response.IsSuccessStatusCode)
                {
                    if (File.Exists(oldImage))
                    {
                        File.Delete(oldImage);
                    }
                    File.Copy(resizedImage, newImagePath);

                    WeakReferenceMessenger.Default.Send(new Message<MyApp>(changedApp, false, this), (int)MessengerTokens.Tokens.MainPageVM);
                    WeakReferenceMessenger.Default.Send(new Message<string>("Данные изменены"), (int)MessengerTokens.Tokens.ViewEditPage);
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new Message<string>("Не удалось изменить данные"), (int)MessengerTokens.Tokens.ViewEditPage);
                }
            }  
        }

        private async void SelectImage()
        {
            try
            {
                PickOptions pickOptions = new PickOptions() { FileTypes = FilePickerFileType.Images };

                FileResult? result = await FilePicker.Default.PickAsync(pickOptions);
                if (result != null)
                {
                    TransformFile(result.FullPath);
                }
            }
            catch (Exception)
            {
                WeakReferenceMessenger.Default.Send(new Message<string>("Не удалось выбрать указанный файл."), (int)MessengerTokens.Tokens.AddPage);
            }
        }

        private void TransformFile(string selectedImagePath)
        {
            resizedImage = Image.ResizeImage(selectedImagePath, 300, 300);
            string resizedImageFileName = Path.GetFileNameWithoutExtension(resizedImage);
            string extension = Path.GetExtension(resizedImage);
            string formattedDateTime = $"{DateTime.Now.ToString("dd-MM-yyyyy")}_{DateTime.Now.ToString("hh-mm-ss")}";

            string? sourceImageDir = Path.GetDirectoryName(ImagePath);
            newImagePath = Path.Combine(sourceImageDir, $"{resizedImageFileName}_{formattedDateTime}{extension}");

            oldImage = ImagePath;
            ImagePath = resizedImage;
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
