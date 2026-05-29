namespace Client
{
    //Интерфейс для передачи параметров между окнами
    interface IParameterReceiver
    {
        void SetParameter(object parameter1, object parameter2);
    }
}
