namespace Quoridor.Logic
{
    using System;
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
        public const int UsedBitsAmount = BitboardSize * BitboardSize; // 289, this is without redundant
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
        private FieldMask walls;
        private FieldMask blueCharacter;
        private FieldMask redCharacter;

        private Dictionary<FieldMask, Dictionary<FieldMask, FieldMask[]>> simplePLayersMoves;

        public QuoridorModel()
        {
            CreateAvailableWallsMask();
            var search = new DijkstraSearch();
            var position = BitboardExtension.Flatten(0, 8);
            PlaceWall(1, 1, WallOrientation.Horizontal);
            PlaceWall(1, 11, WallOrientation.Horizontal);
            PlaceWall(1, 15, WallOrientation.Horizontal);
            PlaceWall(3, 3, WallOrientation.Vertical);
            PlaceWall(5, 1, WallOrientation.Vertical);
            PlaceWall(5, 5, WallOrientation.Horizontal);
            PlaceWall(5, 9, WallOrientation.Horizontal);
            PlaceWall(5, 13, WallOrientation.Horizontal);
            PlaceWall(7, 3, WallOrientation.Horizontal);
            PlaceWall(7, 7, WallOrientation.Vertical);
            PlaceWall(7, 11, WallOrientation.Vertical);
            PlaceWall(7, 15, WallOrientation.Horizontal);
            // PlaceWall(9, 5, WallOrientation.Vertical);
            // PlaceWall(9, 9, WallOrientation.Vertical);
            // PlaceWall(9, 13, WallOrientation.Horizontal);

            if (search.HasPath(this, position, Direction.Down, out var path))
            {
                var mask = new FieldMask();
                foreach (var node in path.nodes)
                {
                    mask.SetBit(node, true);
                }
                walls.Or(mask).ToStr().Log();
                return;
            }
            Console.WriteLine($"Shit");
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
            var wallMask = availableWalls.ExclusiveOr(walls);
            wallMask.ToStr().Log();
            if (CanPlaceWall(wallMask, y, x, wallOrientation))
            {
                var wall = GenerateWall(y, x, wallOrientation);
                walls = walls.Or(wall);
            }
        }

        private List<FieldMask> GenerateWallMoves()
        {
            var wallMask = availableWalls.ExclusiveOr(walls);
            var generatedWalls = new List<FieldMask>(128);
            for (var i = 1; i < BitboardSize; i += 2)
            {
                for (var j = 1; j < BitboardSize; j += 2)
                {
                    if (CanPlaceWall(wallMask, i, j, WallOrientation.Horizontal))
                    {
                        var wall = GenerateWall(i, j, WallOrientation.Horizontal);
                        generatedWalls.Add(wall);
                    }
                    if (CanPlaceWall(wallMask, i, j, WallOrientation.Horizontal))
                    {
                        var wall = GenerateWall(i, j, WallOrientation.Vertical);
                        generatedWalls.Add(wall);
                    }
                }
            }

            return generatedWalls;
        }

        private bool CanPlaceWall(FieldMask mask, int y, int x, WallOrientation wallOrientation)
        {
            var yOffset = wallOrientation == WallOrientation.Vertical ? 1 : 0;
            var xOffset = wallOrientation == WallOrientation.Horizontal ? 1 : 0;
            return mask.GetBit(y, x) &&
                   !mask.GetBit(y + yOffset, x + xOffset) &&
                   !mask.GetBit(y - yOffset, x - xOffset);
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

        public bool IsInRange(int index)
        {
            return index is >= 0 and < UsedBitsAmount;
        }

        public bool IsInRange(int y, int x)
        {
            var index = BitboardExtension.Flatten(y, x);
            return IsInRange(index);
        }

        public bool CanMove(FieldMask moveMask)
        {
            return walls.And(moveMask).IsZero();
        }
    }
}
