namespace Quoridor.Model.Strategies
{
    public static class PlayerConstants
    {
        public static FieldMask[] allPositions;
        public const byte EndRedIndexIncluding = 72;
        public const byte EndBlueIndexIncluding = 8;

        static PlayerConstants()
        {
            GenerateAllPositions();
        }

        private static void GenerateAllPositions()
        {
            allPositions = new FieldMask[FieldMask.PlayerFieldSize * FieldMask.PlayerFieldSize];
            for (var y = 0; y < FieldMask.BitboardSize; y += 2)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x += 2)
                {
                    var playerIndex = FieldMask.GetPlayerIndex(y, x);

                    var playerMask = new FieldMask();
                    playerMask.SetBit(y, x, true);
                    allPositions[playerIndex] = playerMask;
                }
            }
        }
    }
}