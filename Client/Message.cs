using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Client
{
    //Класс для обмена сообщениями между модулями программы via WeakReferenceMessenger
    public class Message : ValueChangedMessage<string>
    {
        private bool isSuccess;

        public bool IsSuccess 
        {
            get => isSuccess;
            set => isSuccess = value;
        }

        public Message(string value, bool IsSuccess = false) : base(value)
        {
            this.IsSuccess = IsSuccess;
        }
    }
}
