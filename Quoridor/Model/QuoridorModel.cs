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
        private const int PossibleWallsInLine = 8;

        private FieldMask blueCharacterStart =
            new(new[] { 0, 0, 0, 0, 1L << ExtraBits + BitboardCenter - 1 });

        private FieldMask redCharacterStart =
            new(new[] { 1L << (BitsBlockSize - BitboardCenter), 0, 0, 0, 0 });

        private FieldMask availableWalls;
        private FieldMask placedWalls;
        private FieldMask walls;
        private FieldMask blueCharacter;
        private FieldMask redCharacter;

        private Dictionary<FieldMask, Dictionary<FieldMask, FieldMask[]>> simplePLayersMoves;

        public QuoridorModel()
        {
            CreateAvailableWallsMask();
            var generatedWalls = GenerateWallMoves();
            foreach (var wall in generatedWalls)
            {
                wall.ToStr().Log();
            }
            // walls.ToStr(blueCharacterStart, redCharacterStart).Log();
            // // var a = (long)~0 >> (QuoridorModel.BitsBlockSize - 9 - 1);
            // redCharacterStart.ToStr().Log();
        }

        private void CreateAvailableWallsMask()
        {
            for (var i = 1; i < BitboardSize; i += 2)
            {
                for (var j = 1; j < BitboardSize; j += 2)
                {
                    availableWalls.SetBit(i, j, true);
                }
            }
        }

        public void PlaceWall(int y, int x, WallOrientation wallOrientation)
        {
            var wallMask = availableWalls.ExclusiveOr(placedWalls);
            if (CanPlaceWall(wallMask, y, x))
            {
                var wall = GenerateWall(y, x, wallOrientation);
                walls = walls.Or(wall);
                placedWalls.SetBit(y, x, true);
            }
        }

        private List<FieldMask> GenerateWallMoves()
        {
            var wallMask = availableWalls.ExclusiveOr(placedWalls);
            var generatedWalls = new List<FieldMask>(128);
            for (var i = 1; i < BitboardSize; i += 2)
            {
                for (var j = 1; j < BitboardSize; j += 2)
                {
                    if (CanPlaceWall(wallMask, i, j))
                    {
                        var wall = GenerateWall(i, j, WallOrientation.Horizontal);
                        generatedWalls.Add(wall);
                        wall = GenerateWall(i, j, WallOrientation.Vertical);
                        generatedWalls.Add(wall);
                    }
                }
            }

            return generatedWalls;
        }

        private bool CanPlaceWall(FieldMask mask, int y, int x)
        {
            return mask.GetBit(y, x);
        }

        private FieldMask GenerateWall(int y, int x, WallOrientation wallOrientation)
        {
            var wall = new FieldMask();
            for (var i = 0; i < 3; i++)
            {
                var yOffset = wallOrientation == WallOrientation.Vertical ? i - 1 : 0;
                var xOffset = wallOrientation == WallOrientation.Horizontal ? i - 1 : 0;
                wall.SetBit(y + yOffset, x + xOffset, true);
            }
            return wall;
        }

        // private bool IsValid(int index)
        // {
        // }

        private void Log()
        {
        }
    }
}
