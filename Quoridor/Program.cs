namespace Quoridor
{
    using System;
    using System.Text;
    using Controller;

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
