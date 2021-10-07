namespace Quoridor.Model
{
    public class Constants
    {
        public const int WallsPerGame = 10;

        public static readonly FieldMask BlueCharacterPosition =
            new(new[] { 0, 0, 0, 0, 1L << FieldMask.ExtraBits + FieldMask.BitboardCenter - 1 });

        public static readonly FieldMask RedCharacterPosition =
            new(new[] { 1L << (FieldMask.BitsBlockSize - FieldMask.BitboardCenter), 0, 0, 0, 0 });
    }
}
