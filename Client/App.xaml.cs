﻿
using Client.Views;

namespace Client
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            //MainPage = new AppShell();
            MainPage = new LoginPage();
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    Window window = base.CreateWindow(activationState);
        //    window.Height = 850;
        //    window.Width = 1300;

        //    return window;
        //}
    }
}
