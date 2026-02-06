namespace Client
{
    //Класс для обмена сообщениями между модулями программы via WeakReferenceMessenger
    public class Message<T> 
    {   
        public T Value { get; set; }
        public object? Sender { get; set; }
        public bool CloseWindow { get; set; }

        public Message(T value, bool closeWindow = false, object? sender = null)
        {
            Value = value;
            CloseWindow = closeWindow;
            Sender = sender;
        }
    }
}
