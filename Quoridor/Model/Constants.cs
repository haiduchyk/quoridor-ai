namespace Quoridor.Model
{
    public class Constants
    {
        public const int WallsPerGame = 10;

        public static readonly FieldMask BluePlayerPosition =
            new(new[] { 0, 0, 0, 0, 1L << FieldMask.ExtraBits + FieldMask.BitboardCenter - 1 });

        // Need to calculate mask
        public static readonly FieldMask BlueEndPositions =
            new(new[] { 1L << (FieldMask.BitsBlockSize - FieldMask.BitboardCenter), 0, 0, 0, 0 });

        public static readonly FieldMask RedPlayerPosition =
            new(new[] { 1L << (FieldMask.BitsBlockSize - FieldMask.BitboardCenter), 0, 0, 0, 0 });

        // Need to calculate mask
        public static readonly FieldMask RedEndPositions =
            new(new[] { 0, 0, 0, 0, 1L << FieldMask.ExtraBits + FieldMask.BitboardCenter - 1 });
    }
}
