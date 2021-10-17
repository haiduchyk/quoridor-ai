namespace Quoridor.Controller
{
    public interface IInputReader
    {
        string ReadInput();
    }

    public interface IInputWriter
    {
        void Write(string message);

        void WriteLine(string message);
    }

    public interface IIoWorker : IInputReader, IInputWriter
    {
    }
}
