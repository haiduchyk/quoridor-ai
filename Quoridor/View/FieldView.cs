namespace Quoridor.View
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Model;
    using Model.Players;

    public class FieldView
    {
        private const char Wall = '⬛';
        private const char EmptyWall = ' ';
        private const char EmptyCharacter = '○';
        private const char Blue = '✱';
        private const char Red = '❤';

        private static readonly Dictionary<int, string> BoardNumbers = new Dictionary<int, string>
        {
            [0] = "0", [1] = "1", [2] = "2", [3] = "3", [4] = "4", [5] = "5", [6] = "6", [7] = "7", [8] = "8",
            [9] = "9",
            [10] = "a", [11] = "b", [12] = "c", [13] = "d", [14] = "e", [15] = "f", [16] = "g", [17] = "h",
        };

        private const string HorizontalLine = "  0 1 2 3 4 5 6 7 8 9 a b c d e f g \n";

        public void Draw(Field field, Player bluePlayer, Player redPlayer)
        {
            var res = new StringBuilder();
            res.Append(HorizontalLine);

            for (var y = 0; y < FieldMask.BitboardSize; y++)
            {
                res.Append($"{BoardNumbers[y]} ");

                for (var x = 0; x < FieldMask.BitboardSize; x++)
                {
                    if (field.HasWall(y, x))
                    {
                        res.Append(Wall);
                        res.Append(' ');
                        continue;
                    }

                    if (y % 2 == 0 && x % 2 == 0)
                    {
                        var isBlue = bluePlayer.Position.GetBit(y, x);
                        var isRed = redPlayer.Position.GetBit(y, x);

                        if (isBlue)
                        {
                            res.Append(Blue);
                        }
                        else if (isRed)
                        {
                            res.Append(Red);
                        }
                        else
                        {
                            res.Append(EmptyCharacter);
                        }
                    }
                    else
                    {
                        res.Append(EmptyWall);
                    }

                    res.Append(' ');
                }

                res.Append($"{BoardNumbers[y]} \n");
            }

            res.Append(HorizontalLine);

            Console.WriteLine(res.ToString());
        }
    }
}
