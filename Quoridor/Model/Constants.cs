namespace Quoridor.Model
{
    using System.Collections.Generic;

    public class Constants
    {
        public const int WallsPerGame = 10;

        public static readonly FieldMask BluePlayerPosition =
            new(new[] { 0, 0, 0, 0, 1L << FieldMask.ExtraBits + FieldMask.BitboardCenter - 1 });

        public static readonly string BluePlayerName = "Blue";

        public static readonly FieldMask BlueEndPositions =
            new(new[] { -6148961603732635648, 0, 0, 0, 0 });

        public static readonly FieldMask RedPlayerPosition =
            new(new[] { 1L << (FieldMask.BitsBlockSize - FieldMask.BitboardCenter), 0, 0, 0, 0 });

        public static readonly string RedPlayerName = "Blue";

        public static readonly FieldMask RedEndPositions =
            new(new[] { 0, 0, 0, 0, 1431650304L });

        public static readonly List<(int y, int x)> Directions = new() { (1, 0), (0, 1), (-1, 0), (0, -1) };
    }
}
