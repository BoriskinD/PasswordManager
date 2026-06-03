using Client.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;


namespace Client.ViewModels
{
    public partial class AddPageVM : ObservableObject, IParameterReceiver 
    {
        [ObservableProperty]
        private string? _title;

        [ObservableProperty]
        private string? _userLogin;

        [ObservableProperty]
        private string? _userPassword;

        [ObservableProperty]
        private string? _imagePath;

        private HttpWrapper httpWrapper;
        private User? user;

        private string baseDirectory;
        private string? imageFolder;
        private string pathToImage;
        private string selectedImage;

        public AddPageVM()
        {
            httpWrapper = HttpWrapper.GetInstance();

            baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ImagePath = MainPageVM.defaultImage;
            pathToImage = string.Empty;
            selectedImage = string.Empty;
        }

        [RelayCommand]
        private async Task Save()
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

        [RelayCommand]
        private async Task SelectImage() 
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
    }
}
