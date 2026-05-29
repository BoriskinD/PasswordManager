using Client.Model;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client.ViewModels
{
    public class AddPageVM : INotifyPropertyChanged, IParameterReceiver
    {
        private HttpWrapper httpWrapper;
        private User user;
        private string? title;
        private string? userLogin;
        private string? userPassword;
        private string? imagePath;
        private string baseDirectory;
        private string imageFolder;
        private string pathToImage;
        private string selectedImage;

        public event PropertyChangedEventHandler? PropertyChanged;

        public RelayCommand SaveCommand { get; }
        public RelayCommand SelectImageCommand { get; }

        public string Title 
        {
            get => title;
            set => title = value;
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

        public string ImagePath
        {
            get => imagePath;
            set
            {
                imagePath = value;
                OnPropertyChanged();
            }
        }

        public AddPageVM()
        {
            httpWrapper = HttpWrapper.GetInstance();
            SaveCommand = new RelayCommand(Save);
            SelectImageCommand = new RelayCommand(SelectImage);

            baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ImagePath = MainPageVM.defaultImage;
            pathToImage = string.Empty;
            selectedImage = string.Empty;
        }

        private async void Save()
        {
            MyApp newApp = new MyApp();
            newApp.UserId = user.Id;
            newApp.Title = Title;
            newApp.UserLogin = UserLogin;
            newApp.ImagePath = string.IsNullOrEmpty(pathToImage) ? ImagePath : pathToImage;
            newApp.UserPassword = SecureSession.getInstance().Encrypt(UserPassword);

            string? token = await SecureStorage.GetAsync($"AccsessToken");
            //Передаем токен в запрос
            httpWrapper.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using HttpResponseMessage response = await httpWrapper.Post(newApp);
            {
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    newApp.Id = int.Parse(responseContent);

                    if (!string.IsNullOrEmpty(selectedImage))
                    {
                        File.Copy(selectedImage, pathToImage);
                    }

                    WeakReferenceMessenger.Default.Send(new Message<MyApp>(newApp), (int)MessengerTokens.Tokens.MainPageVM);
                    WeakReferenceMessenger.Default.Send(new Message<string>("Данные были успешно добавлены."), (int)MessengerTokens.Tokens.AddPage);
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new Message<string>("Не удалось добавить данные."), (int)MessengerTokens.Tokens.AddPage);
                }
            } 
        }

        private async void SelectImage() 
        {
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }
                
            try
            {
                PickOptions pickOptions = new PickOptions() { FileTypes = FilePickerFileType.Images };

                FileResult? result = await FilePicker.Default.PickAsync(pickOptions);
                if (result != null)
                {
                    selectedImage = result.FullPath;
                    ImagePath = Image.ResizeImage(selectedImage, 300, 300);

                    string fileName = Path.GetFileNameWithoutExtension(result.FullPath);
                    string extension = Path.GetExtension(result.FullPath);
                    string formattedDateTime = $"{DateTime.Now.ToString("dd-MM-yyyyy")}_{DateTime.Now.ToString("hh-mm-ss")}";

                    pathToImage = Path.Combine(imageFolder, $"{fileName}_{formattedDateTime}{extension}");
                }
            }
            catch (Exception)
            {
                WeakReferenceMessenger.Default.Send(new Message<string>("Не удалось выбрать указанный файл."), (int)MessengerTokens.Tokens.AddPage);
            }
        }

        public void SetParameter(object parameter1, object parameter2)
        {
            if (parameter1 is User loginedUser)
            {
                user = loginedUser;
                imageFolder = Path.Combine(baseDirectory, $"Images/{user.Login}");
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                                      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
