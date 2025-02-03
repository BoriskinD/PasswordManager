using Client.Model;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Client
{
    public class DataToPass : ValueChangedMessage<MyApp>
    {
        private MyApp? app;

        public MyApp MyApp
        {
            get => app;
            set => app = value;
        }

        public DataToPass(MyApp myApp) : base(myApp)
        {
            MyApp = myApp;
        }
    }
}
