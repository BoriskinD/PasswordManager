using Client.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Client.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageVM loginPageVM)
	{
		InitializeComponent();
        BindingContext = loginPageVM;

        WeakReferenceMessenger.Default.Register<Message<string>,int>(this, (int)MessengerTokens.Tokens.LoginPage, (recipient, message) =>
        {
            if (message.CloseWindow)
            {
                Application.Current?.CloseWindow(Window);
                return;
            }

            DisplayAlert("Инфо", message.Value, "ОК");
        });
    }

    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        // Получаем холст и его размеры
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        // Очищаем холст белым цветом
        canvas.Clear(SKColors.White);

        // Создаем настройки для рисования
        var paint = new SKPaint
        {
            Color = SKColors.Blue,       // Цвет заливки
            IsAntialias = true,          // Сглаживание
            Style = SKPaintStyle.Fill     // Заливка
        };

        // Рисуем круг
        float centerX = info.Width / 2;  // Центр по ширине
        float centerY = info.Height / 2; // Центр по высоте
        float radius = 100;              // Радиус круга

        canvas.DrawCircle(centerX, centerY, radius, paint);
    }
}