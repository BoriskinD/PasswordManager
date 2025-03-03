using Client.Model;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace Client.ViewModels
{
    public class AddPageVM : INotifyPropertyChanged
    {
        private HttpWrapper httpWrapper;
        private string? title, userLogin, userPassword, imagePath;
        private string baseDirectory, imageFolder, pathToImage, selectedImage;
        private int _userId;

        public delegate void NewAppCreatedHandler(MyApp newApp);
        public event NewAppCreatedHandler? NewAppCreated;
        public event PropertyChangedEventHandler? PropertyChanged;

        public RelayCommand SaveCommand { get; }
        public RelayCommand SelectImageCommand { get; }

        public string Title 
        {
            get => title;
            set 
            {
                title = value;
            }
        }

        public string UserLogin
        {
            get => userLogin;
            set
            {
                userLogin = value;
            }
        }

        public string UserPassword
        {
            get => userPassword;
            set
            {
                userPassword = value;
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

        public AddPageVM(int userId)
        {
            baseDirectory = AppDomain.CurrentDomain.BaseDirectory; 
            imageFolder = Path.Combine(baseDirectory, "Images");

            ImagePath = "dotnet_bot.png";
            pathToImage = string.Empty;
            selectedImage = string.Empty;
            _userId = userId;

            httpWrapper = HttpWrapper.GetInstance();
            SaveCommand = new RelayCommand(Save);
            SelectImageCommand = new RelayCommand(SelectImage);
        }

        private async void Save()
        {
            MyApp newApp = new MyApp()
            {
                UserId = _userId,
                Title = Title,
                UserLogin = UserLogin,
                UserPassword = UserPassword,
                ImagePath = ImagePath
            };

            bool IsSuccess;
            using HttpResponseMessage response = await httpWrapper.Post(newApp);
            {
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    newApp.Id = int.Parse(responseContent);

                    IsSuccess = true;
                    //СВЯЗАНО С AddPage.xaml.cs
                    WeakReferenceMessenger.Default.Send(new Message("Данные были успешно добавлены.", IsSuccess), 1);

                    File.Copy(selectedImage, pathToImage, true);

                    NewAppCreated?.Invoke(newApp);
                }
                else
                {
                    IsSuccess = false;
                    WeakReferenceMessenger.Default.Send(new Message("Не удалось добавить данные.", IsSuccess), 1);
                }
            } 
        }

        private async void SelectImage() 
        {
            if (!Directory.Exists(imageFolder))
                Directory.CreateDirectory(imageFolder);

            try
            {
                PickOptions pickOptions = new PickOptions()
                { 
                    FileTypes = FilePickerFileType.Images
                };

                FileResult? result = await FilePicker.Default.PickAsync(pickOptions);
                if (result != null)
                {
                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        selectedImage = result.FullPath;
                        pathToImage = Path.Combine(imageFolder, result.FileName);
                        ImagePath = pathToImage;
                    }
                }
            }
            catch (Exception)
            {
                WeakReferenceMessenger.Default.Send(new Message("Не удалось выбрать указанный файл.", false), 1);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                                      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
