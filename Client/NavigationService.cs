namespace Client
{
    public class NavigationService : INavigationService
    {
        public async Task NavigateToAsync<TPage>() where TPage : Page
        {
           Page page = Activator.CreateInstance<TPage>();
           await Shell.Current.Navigation.PushAsync(page);
        }

        public void OpenWindow(Action<Window> configureWindow, Page page)
        {
            //Создать окно
            Window window = new Window(page);

            //Настроить окно
            configureWindow?.Invoke(window);

            //Открыть новое окно
            Application.Current?.OpenWindow(window);
        }
    }
}
