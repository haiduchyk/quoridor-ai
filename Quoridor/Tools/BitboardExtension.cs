namespace Quoridor
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Model;

    // Debug tools
    public static class BitboardExtension
    {
        private static string wall = "⬛";
        private static string emptyWall = "⬜";
        private static string none = " ";
        private static string badNone = "⬛";
        private static string character = "●";
        private static string emptyCharacter = "○";
        private static string blue = "✱";
        private static string red = "❤";

        private static Dictionary<int, string> boardNumbers = new Dictionary<int, string>
        {
            [0] = "0", [1] = "1", [2] = "2", [3] = "3", [4] = "4", [5] = "5", [6] = "6", [7] = "7", [8] = "8",
            [9] = "9",
            [10] = "a", [11] = "b", [12] = "c", [13] = "d", [14] = "e", [15] = "f", [16] = "g", [17] = "h",
        };

        private static string horizontalLine = "  0 1 2 3 4 5 6 7 8 9 a b c d e f g \n";

        public static string ToStr(this FieldMask bitboard)
        {
            var res = new StringBuilder();
            res.Append(horizontalLine);

            for (var y = 0; y < FieldMask.BitboardSize; y++)
            {
                res.Append($"{boardNumbers[y]} ");

                for (var x = 0; x < FieldMask.BitboardSize; x++)
                {
                    var isTrue = bitboard.GetBit(y, x);

                    if (y % 2 != 0)
                    {
                        if (x % 2 != 0)
                        {
                            res.Append(isTrue ? $"{badNone} " : $"{none} ");
                        }
                        else
                        {
                            res.Append(isTrue ? $"{wall} " : $"{emptyWall} ");
                        }
                    }
                    else
                    {
                        if (x % 2 != 0)
                        {
                            res.Append(isTrue ? $"{wall} " : $"{emptyWall} ");
                        }
                        else
                        {
                            res.Append(isTrue ? $"{character} " : $"{emptyCharacter} ");
                        }
                    }
                }

                res.Append($"{boardNumbers[y]} \n");
            }

            res.Append(horizontalLine);

            return res.ToString();
        }

        public static void Log(this FieldMask bitboard)
        {
            bitboard.ToStr().Log();
        }

        public static string ToStr(this FieldMask bitboard, FieldMask blueBitboard, FieldMask redBitboard)
        {
            var res = new StringBuilder();
            res.Append(horizontalLine);

            for (var y = 0; y < FieldMask.BitboardSize; y++)
            {
                res.Append($"{boardNumbers[y]} ");

                for (var x = 0; x < FieldMask.BitboardSize; x++)
                {
                    var isTrue = bitboard.GetBit(y, x);

                    if (y % 2 != 0)
                    {
                        if (x % 2 != 0)
                        {
                            res.Append(isTrue ? $"{badNone} " : $"{none} ");
                        }
                        else
                        {
                            res.Append(isTrue ? $"{wall} " : $"{emptyWall} ");
                        }
                    }
                    else
                    {
                        if (x % 2 != 0)
                        {
                            res.Append(isTrue ? $"{wall} " : $"{emptyWall} ");
                        }
                        else
                        {
                            var isRed = redBitboard.GetBit(y, x);
                            var isBlue = blueBitboard.GetBit(y, x);

                            if (isRed)
                            {
                                res.Append($"{red} ");
                            }

                            if (isBlue)
                            {
                                res.Append($"{blue} ");
                            }

                            if (!isRed && !isBlue)
                            {
                                res.Append($"{emptyCharacter} ");
                            }
                        }
                    }
                }

                res.Append($"{boardNumbers[y]} \n");
            }

            res.Append(horizontalLine);

            return res.ToString();
        }

        public static string ToStrB(this FieldMask bitboard)
        {
            var res = new StringBuilder();
            res.Append("  0 1 2 3 4 6 7 8 9 a b c d e f g h \n");

            for (var y = 0; y < FieldMask.BitboardSize; y++)
            {
                res.Append($"{boardNumbers[y]} ");

                for (var x = 0; x < FieldMask.BitboardSize; x++)
                {
                    var isTrue = bitboard.GetBit(y, x);
                    res.Append(isTrue ? $"{wall} " : $"{emptyWall} ");
                }

                res.Append($"{boardNumbers[y]} ");
                res.Append(" \n");
            }

            res.Append("  0 1 2 3 4 6 7 8 9 a b c d e f g h \n");

            return res.ToString();
        }

        public static void Log(this string s)
        {
            Console.WriteLine($"{s}");
        }

        public static string ToPos(this FieldMask mask)
        {
            for (var i = 0; i < FieldMask.BitboardSize; i++)
            {
                for (var j = 0; j < FieldMask.BitboardSize; j++)
                {
                    if (mask.GetBit(i, j))
                    {
                        return $"{i} {j}";
                    }
                }
            }
            return "";
        }
    }
}
