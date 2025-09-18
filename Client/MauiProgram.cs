using Client.ViewModels;
using Client.Views;
using CommunityToolkit.Maui;
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
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
                
#if DEBUG
    		builder.Logging.AddDebug();
#endif
            //builder.UseSkiaSharp();
            //Регистрация сервисов в DI контейнер
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddTransient<LoginPageVM>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPageVM>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AddPage>();
            builder.Services.AddTransient<AddPageVM>();
            builder.Services.AddTransient<ViewEditPage>();
            builder.Services.AddTransient<ViewEditPageVM>();

            return builder.Build();
        }
    }
}
