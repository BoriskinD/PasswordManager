namespace Client
{
    public interface INavigationService
    {
        void OpenWindow(Action<Window> configureWindow, Page page);
        Task NavigateToAsync<TPage>() where TPage : Page;
    }
}
