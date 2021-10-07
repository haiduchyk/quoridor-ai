using System;

namespace Quoridor
{
    using System.Text;
    using Model;

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var gameModel = new QuoridorModel();
        }
    }
}
