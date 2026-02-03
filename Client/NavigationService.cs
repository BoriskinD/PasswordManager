namespace Client
{
    public class NavigationService : INavigationService
    {
        //DI контейнер
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OpenWindow<TPage>(Action<Window> configureWindow, object? parameter = null) where TPage : Page
        {
            //Получить страницу из контейнера
            TPage page = _serviceProvider.GetRequiredService<TPage>();

            if (page.BindingContext is IParameterReceiver receiver && parameter != null)
            { 
                receiver.SetParameter(parameter);
            }

            Window window = new Window(page);

            //Настроить окно
            configureWindow?.Invoke(window);

            Application.Current?.OpenWindow(window);
        }
    }
}
