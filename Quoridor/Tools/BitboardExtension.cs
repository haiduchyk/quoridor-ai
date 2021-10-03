namespace Quoridor.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Logic;

    public static class BitboardExtension
    {
        public static int Flatten(int x, int y)
        {
            return x + y * QuoridorModel.BitsBlockSize;
        }

        // c
        public static (int i, int j) Nest(int index)
        {
            var i = index / QuoridorModel.BitsBlockSize;
            var j = index % QuoridorModel.BitsBlockSize;
            return (i, j);
        }

        // c
        public static bool GetBit(this long[] bitboard, int index)
        {
            var (i, j) = Nest(index);
            var block = bitboard[i];
            return (block & (1L << (QuoridorModel.BitsBlockSize - j - 1))) != 0;
        }

        public static bool GetBit(this long[] bitboard, int y, int x)
        {
            var index = x + y * QuoridorModel.BitboardSize;
            return bitboard.GetBit(index);
        }

        private static string wall = "⬛";
        private static string emptyWall = "⬜";
        private static string none = " ";
        private static string badNone = "x";
        private static string character = "●";
        private static string emptyCharacter = "○";
        private static string blue = "✱";
        private static string red = "❤";

        private static Dictionary<int, string> boardNumbers = new Dictionary<int, string>
        {
            [0] = "0", [1] = "1", [2] = "2", [3] = "3", [4] = "4", [5] = "5", [6] = "6", [7] = "7", [8] = "8", [9] = "9",
            [10] = "a", [11] = "b", [12] = "c", [13] = "d", [14] = "e", [15] = "f", [16] = "g", [17] = "h",
        };

        private static string horizontalLine = "  0 1 2 3 4 6 7 8 9 a b c d e f g h \n";

        public static string ToStr(this long[] bitboard)
        {
            var res = new StringBuilder();
            res.Append(horizontalLine);

            for (var y = 0; y < QuoridorModel.BitboardSize; y++)
            {
                res.Append($"{boardNumbers[y]} ");

                for (var x = 0; x < QuoridorModel.BitboardSize; x++)
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
        
        public static string ToStr(this long[] bitboard, long[] blueBitboard, long[] redBitboard)
        {
            var res = new StringBuilder();
            res.Append(horizontalLine);

            for (var y = 0; y < QuoridorModel.BitboardSize; y++)
            {
                res.Append($"{boardNumbers[y]} ");

                for (var x = 0; x < QuoridorModel.BitboardSize; x++)
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

        public static string ToStrB(this long[] bitboard)
        {
            var res = new StringBuilder();
            res.Append("  0 1 2 3 4 6 7 8 9 a b c d e f g h \n");

            for (var y = 0; y < QuoridorModel.BitboardSize; y++)
            {
                res.Append($"{boardNumbers[y]} ");

                for (var x = 0; x < QuoridorModel.BitboardSize; x++)
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
    }
}