using Client.Model;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client.ViewModels
{
    class MainPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<MyApp> Apps { get; }
        public ICommand OpenAddPageCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand OpenViewEditPageCommand { get; }
        public ICommand DownloadDataFromDBCommand { get; }

        private Window? addWindow, viewEditWindow;
        private MyApp? selectedApp;
        private HttpWrapper httpWrapper;
        //залогиненный пользователь
        private User _user;

        private string? userName;

        public string UserInfo
        { 
            get => userName;
            set
            {
                userName = value;
                OnPropertyChanged();
            } 
        }

        public MainPageVM(User user)
        {
            httpWrapper = HttpWrapper.GetInstance();
            Apps = new ObservableCollection<MyApp>();
            OpenAddPageCommand = new RelayCommand(OpenAddPage);
            DeleteItemCommand = new RelayCommand(DeleteItem);
            OpenViewEditPageCommand = new RelayCommand(OpenViewEditPage);
            DownloadDataFromDBCommand = new RelayCommand(DownloadDataFromDataBase);
            _user = user;

            addWindow = null;
            viewEditWindow = null;
            selectedApp = null;

            UserInfo = $"Пользователь: {_user.Login}";

            WeakReferenceMessenger.Default.Register<DataToPass>(this, (r, m) => 
            { 
                selectedApp = m.MyApp; 
            });
        }

        private void OpenAddPage()
        {
            if (addWindow == null)
            {
                AddPage addPage = new AddPage();
                AddPageVM addPageVM = new AddPageVM(_user);

                addPageVM.NewAppCreated += OnNewAppCreated;
                addPage.BindingContext = addPageVM;
                
                addWindow = new Window(addPage);
                addWindow.Destroying += (s, e) => 
                { 
                    addPageVM.NewAppCreated -= OnNewAppCreated;
                    addWindow = null;
                };
                addWindow.Width = 500;
                addWindow.Height = 500;

                Application.Current?.OpenWindow(addWindow);
            }
        }

        private void OpenViewEditPage()
        {
            if (viewEditWindow == null)
            {
                ViewEditPage viewEditPage = new ViewEditPage();
                ViewEditPageVM viewEditPageVM = new ViewEditPageVM(selectedApp);

                viewEditPageVM.AppChanged += OnAppChanged;
                viewEditPage.BindingContext = viewEditPageVM;

                viewEditWindow = new Window(viewEditPage);
                viewEditWindow.Destroying += (s, e) =>
                {
                    viewEditWindow = null;
                };
                viewEditWindow.Width = 500;
                viewEditWindow.Height = 500;
                Application.Current?.OpenWindow(viewEditWindow);
            }
        }


        private async void DeleteItem()
        {
            if (selectedApp == null)
            {
                return;
            }

            string? token = await SecureStorage.GetAsync($"AccsessToken");
            httpWrapper.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            await httpWrapper.Delete(selectedApp.Id);
            Apps.Remove(selectedApp);
        }

        private async void DownloadDataFromDataBase()
        {
            if (Apps.Count == 0)
            {
                string? token = await SecureStorage.GetAsync($"AccsessToken");
                httpWrapper.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                List<MyApp>? listOfApps = await httpWrapper.Get();
                if (listOfApps != null)
                {
                    foreach (MyApp item in listOfApps)
                        Apps.Add(item);
                }
                else
                {
                    //СВЯЗАНО С MainPage.xaml.cs
                    WeakReferenceMessenger.Default.Send(new Message("В базе данных нет записей"), 0);
                }
            }
        }

        private void OnNewAppCreated(MyApp newApp) => Apps?.Add(newApp);

        private void OnAppChanged(MyApp changedApp)
        {
            MyApp? tmp = Apps.FirstOrDefault(element => element.Id == changedApp.Id);
            if (tmp != null)
            { 
                tmp.Title = changedApp.Title;
                tmp.UserLogin = changedApp.UserLogin;
                tmp.UserPassword = changedApp.UserPassword;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                                      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
