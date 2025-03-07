using Microsoft.Extensions.Logging;

namespace Client
{
    //Точка входа в приложение
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            //Регистрация сервиса в DI контейнер
            builder.Services.AddSingleton<INavigationService, NavigationService>();

            return builder.Build();
        }
    }
}
