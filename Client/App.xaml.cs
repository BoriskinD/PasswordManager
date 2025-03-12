using Client.Views;

namespace Client
{
    public partial class App : Application
    {
        //loginPage создаётся и передаётся в метод с помощью DI контейнера
        public App(LoginPage loginPage)
        {
            InitializeComponent();
            MainPage = loginPage;
        }
    }
}
