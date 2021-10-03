namespace Quoridor.Logic
{
    using Tools;

    public class QuoridorModel
    {
        private const int FieldSize = 9;

        public const int BitsBlockSize = 64;
        private const int BitBlocksAmount = 5;
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

        private long[] blueCharacterStart = new long[BitBlocksAmount]
            { 0, 0, 0, 0, 1L << ExtraBits + BitboardCenter - 1};

        private long[] redCharacterStart = new long[BitBlocksAmount]
        { 1L << (BitsBlockSize - BitboardCenter), 0, 0, 0, 0 };


        // private long[] walls = new long[BitBlocksAmount];
        private long[] walls = new long[BitBlocksAmount];
        private long[] blueCharacter = new long[BitBlocksAmount];
        private long[] redCharacter = new long[BitBlocksAmount];

        // private Dictionary<int, Dictionary<long, long[]>>


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