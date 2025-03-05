using Client.Model;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client.ViewModels
{
    public class AddPageVM : INotifyPropertyChanged
    {
        private HttpWrapper httpWrapper;
        private string? title, userLogin, userPassword, imagePath;
        private string baseDirectory, imageFolder, pathToImage, selectedImage;
        private User _user;

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

        public AddPageVM(User user)
        {
            baseDirectory = AppDomain.CurrentDomain.BaseDirectory; 
            imageFolder = Path.Combine(baseDirectory, "Images");

            ImagePath = "dotnet_bot.png";
            pathToImage = string.Empty;
            selectedImage = string.Empty;
            _user = user;

            httpWrapper = HttpWrapper.GetInstance();
            SaveCommand = new RelayCommand(Save);
            SelectImageCommand = new RelayCommand(SelectImage);
        }

        private async void Save()
        {
            MyApp newApp = new MyApp()
            {
                UserId = _user.Id,
                Title = Title,
                UserLogin = UserLogin,
                UserPassword = UserPassword,
                ImagePath = ImagePath
            };

            string? token = await SecureStorage.GetAsync($"AccsessToken");
            //Передаем токен в запрос
            httpWrapper.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using HttpResponseMessage response = await httpWrapper.Post(newApp);
            {
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    newApp.Id = int.Parse(responseContent);

                    //СВЯЗАНО С AddPage.xaml.cs
                    WeakReferenceMessenger.Default.Send(new Message("Данные были успешно добавлены.", true), 1);

                    File.Copy(selectedImage, pathToImage, true);

                    NewAppCreated?.Invoke(newApp);
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new Message("Не удалось добавить данные."), 1);
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
