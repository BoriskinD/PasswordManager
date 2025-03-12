namespace Client
{
    //Интерфейс навигации между окнами (чтобы не нарушать принципы MVVM)
    public interface INavigationService
    {
        void OpenWindow<TPage>(object parameter, Action<Window> configureWindow) where TPage : Page;
    }
}
