using System;

namespace Quoridor
{
    using System.Text;
    using Controller;
    using Model;

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var gameContainer = new GameContainer();
            gameContainer.MenuController.StartNewGame();
        }
    }
}
