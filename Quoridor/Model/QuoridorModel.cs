namespace Quoridor.Logic
{
    using System.Collections.Generic;
    using Tools;

    public class QuoridorModel
    {
        private const int FieldSize = 9;

        public const int BitsBlockSize = 64;
        public const int BitBlocksAmount = 5;
        public const int BitboardSize = 17;

        public const int BitboardCenter = BitboardSize / 2 + 1;
        private const int TotalBitsAmount = BitsBlockSize * BitBlocksAmount; // 320, this is with redundant
        private const int UsedBitsAmount = BitboardSize * BitboardSize; // 289, this is without redundant
        private const int ExtraBits = TotalBitsAmount - UsedBitsAmount; // 31 redundant bits

        private const int SimpleMoveBitsAmount = 3;
        private const int MoveWithEnemyBitsAmount = 7;

        private ushort blueCharacterWalls;
        private ushort redCharacterWalls;

        private const int WallsPerGame = 10;

        private FieldMask blueCharacterStart =
            new(new[] { 0, 0, 0, 0, 1L << ExtraBits + BitboardCenter - 1 });

        private FieldMask redCharacterStart =
            new(new[] { 1L << (BitsBlockSize - BitboardCenter), 0, 0, 0, 0 });


        // private FieldMask walls = new FieldMask();
        private FieldMask walls;
        private FieldMask blueCharacter;
        private FieldMask redCharacter;

        private Dictionary<FieldMask, Dictionary<FieldMask, FieldMask[]>> simplePLayersMoves;


        public QuoridorModel()
        {
            walls.ToStr(blueCharacterStart, redCharacterStart).Log();
            // var a = (long)~0 >> (QuoridorModel.BitsBlockSize - 9 - 1);
            redCharacterStart.ToStr().Log();
        }

        // private bool IsValid(int index)
        // {
        // }

        private void Log()
        {
        }
    }
}