namespace Quoridor.View
{
    using System;
    using Model;
    using Model.Players;

    public class FieldView
    {
        private const char Wall = '⬛';
        private const char EmptyWall = ' ';
        private const char EmptyCharacter = '○';
        private const char Blue = '✱';
        private const char Red = '❤';
        private const string Symbols = "0123456789abcdefgh";

        private const string HorizontalLine = "  0 1 2 3 4 5 6 7 8 9 a b c d e f g \n";

        public void Draw(Field field, Player bluePlayer, Player redPlayer)
        {
            RenderBoardLine();
            for (var y = 0; y < FieldMask.BitboardSize; y++)
            {
                RenderBoardNumber(y);
                for (var x = 0; x < FieldMask.BitboardSize; x++)
                {
                    if (field.HasWall(y, x))
                    {
                        RenderWall(y, x, bluePlayer, redPlayer);
                    }
                    else if (IsPlayerPosition(y, x))
                    {
                        RenderPlayer(y, x, bluePlayer, redPlayer);
                    }
                    else
                    {
                        RenderEmptyWall();
                    }
                }
                RenderBoardNumber(y);
                Console.WriteLine();
            }
            RenderBoardLine();
        }

        private bool IsPlayerPosition(int y, int x)
        {
            return y % 2 == 0 && x % 2 == 0;
        }

        private void RenderWall(int y, int x, Player bluePlayer, Player redPlayer)
        {
            var isBlue = bluePlayer.Walls.GetBit(y, x);
            var isRed = redPlayer.Walls.GetBit(y, x);
            var color = isBlue ? ConsoleColor.Blue : isRed ? ConsoleColor.Red : ConsoleColor.Gray;
            Write($"{Wall} ", color);
        }

        private void RenderPlayer(int y, int x, Player bluePlayer, Player redPlayer)
        {
            var isBlue = bluePlayer.Position.GetBit(y, x);
            var isRed = redPlayer.Position.GetBit(y, x);

            if (isBlue)
            {
                Write($"{Blue} ", ConsoleColor.Blue);
            }
            else if (isRed)
            {
                Write($"{Red} ", ConsoleColor.Red);
            }
            else
            {
                Write($"{EmptyCharacter} ");
            }
        }

        private void RenderEmptyWall()
        {
            Write($"{EmptyWall} ");
        }

        private void RenderBoardNumber(int y)
        {
            Write($"{Symbols[y]} ", ConsoleColor.DarkMagenta);
        }

        private void RenderBoardLine()
        {
            Write(HorizontalLine, ConsoleColor.DarkCyan);
        }

        private void Write(string message)
        {
            Console.Write(message);
        }

        private void Write(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
    }
}
