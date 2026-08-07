using Client.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Client.ViewModels
{
    public partial class ViewEditPageVM : ObservableObject, IParameterReceiver
    {
        [ObservableProperty] private bool _isTitleEnabled;
        [ObservableProperty] private bool _isUserLoginEnabled;
        [ObservableProperty] private bool _isUserPasswordEnabled;
        [ObservableProperty] private bool _isEditAllowed;
        [ObservableProperty] private string _title;
        [ObservableProperty] private string _userLogin;
        [ObservableProperty] private string _userPassword;
        [ObservableProperty] private string _imagePath;

        private HttpWrapper httpWrapper;
        private int selectedAppId;
        private string resizedImage;
        private string newImagePath;
        private string oldImage;

        public ViewEditPageVM()
        {
            httpWrapper = HttpWrapper.GetInstance();

            resizedImage = string.Empty;
            newImagePath = string.Empty;
            oldImage = string.Empty;    
        }

        //хук-метод
        partial void OnIsEditAllowedChanged(bool value)
        {
            if (IsEditAllowed)
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

        [RelayCommand]
        private async Task SaveChanges()
        {
            MyApp changedApp = new MyApp()
            {
                Id = selectedAppId,
                Title = Title,
                UserLogin = UserLogin,
                UserPassword = SecureSession.getInstance().Encrypt(UserPassword),
                ImagePath = string.IsNullOrEmpty(newImagePath) ? ImagePath : newImagePath
            };

            using HttpResponseMessage response = await httpWrapper.Put(changedApp);
            {
                if (response.IsSuccessStatusCode)
                {
                    if (File.Exists(oldImage)) 
                        File.Delete(oldImage);

                    if (!string.IsNullOrEmpty(resizedImage) && !string.IsNullOrEmpty(newImagePath))
                        File.Copy(resizedImage, newImagePath);

                    WeakReferenceMessenger.Default.Send(new Message<MyApp>(changedApp, false, this), (int)MessengerTokens.Tokens.MainPageVM);
                    WeakReferenceMessenger.Default.Send(new Message<string>("Данные изменены"), (int)MessengerTokens.Tokens.ViewEditPage);
                }
                else WeakReferenceMessenger.Default.Send(new Message<string>("Не удалось изменить данные"), (int)MessengerTokens.Tokens.ViewEditPage);
            }  
        }

        [RelayCommand]
        private async Task SelectImage()
        {
            try
            {
                PickOptions pickOptions = new PickOptions() { FileTypes = FilePickerFileType.Images };

                FileResult? result = await FilePicker.Default.PickAsync(pickOptions);
                if (result != null)
                    TransformFile(result.FullPath);
            }
            catch (Exception) { WeakReferenceMessenger.Default.Send(new Message<string>("Не удалось выбрать указанный файл."),
                                (int)MessengerTokens.Tokens.AddPage); }
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

        public void SetParameter(object parameter1, object parameter2)
        {
            if (parameter1 is MyApp myApp && 
                (parameter2 is User loginedUser && parameter2 != null))
            { 
                selectedAppId = myApp.Id;
                Title = myApp.Title;
                UserLogin = myApp.UserLogin;
                ImagePath = myApp.ImagePath;
                UserPassword = SecureSession.getInstance().Decrypt(myApp.UserPassword);
            }
        }
    }
}
