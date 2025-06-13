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
        private string? title, userLogin, userPassword, imagePath;
        private string baseDirectory, imageFolder, pathToImage, selectedImage;
        private int _userId;

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

        public AddPageVM()
        {
            baseDirectory = AppDomain.CurrentDomain.BaseDirectory; 
            imageFolder = Path.Combine(baseDirectory, "Images");

            ImagePath = "no_image_available.jpg";
            pathToImage = string.Empty;
            selectedImage = string.Empty;

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

            string? token = await SecureStorage.GetAsync($"AccsessToken");
            //Передаем токен в запрос
            httpWrapper.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using HttpResponseMessage response = await httpWrapper.Post(newApp);
            {
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    newApp.Id = int.Parse(responseContent);

                    File.Copy(selectedImage, pathToImage, true);

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
                PickOptions pickOptions = new PickOptions()
                { 
                    FileTypes = FilePickerFileType.Images
                };

                FileResult? result = await FilePicker.Default.PickAsync(pickOptions);
                if (result != null)
                {
                    selectedImage = result.FullPath;
                    pathToImage = Path.Combine(imageFolder, result.FileName);
                    ImagePath = pathToImage;
                }
            }
            catch (Exception)
            {
                WeakReferenceMessenger.Default.Send(new Message<string>("Не удалось выбрать указанный файл."), (int)MessengerTokens.Tokens.AddPage);
            }
        }

        public void SetParameter(object parameter)
        {
            if (parameter is int userId)
            {
                _userId = userId;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                                      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
