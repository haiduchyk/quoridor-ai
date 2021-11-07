namespace Quoridor.Controller.Io
{
    using System;

    public class ConsoleWorker : IIoWorker
    {
        public string ReadInput()
        {
            return Console.ReadLine();
        }

        public void Write(string message)
        {
            Console.Write(message);
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
