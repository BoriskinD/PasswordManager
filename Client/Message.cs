namespace Client
{
    //Класс для обмена сообщениями между модулями программы via WeakReferenceMessenger
    public class Message<T> 
    {   
        public T Value { get; set; }
        public object Sender { get; set; }

        public Message(T value, object sender = null)
        {
            Value = value;
            Sender = sender;
        }
    }
}
