namespace Client
{
    //Интерфейс навигации между окнами (чтобы не нарушать принципы MVVM)
    public interface INavigationService
    {
        void OpenWindow<TPage>(Action<Window> configureWindow, object? parameter = null,
                               object? user = null) where TPage : Page;
    }
}
