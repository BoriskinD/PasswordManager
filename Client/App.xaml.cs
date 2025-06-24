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

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);
            window.Height = 500;
            window.Width = 500;

            return window;
        }
    }
}
