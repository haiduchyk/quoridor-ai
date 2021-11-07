namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;

    public static class WallConstants
    {
        private const int WallSize = 3;
        private const int MaxWallCount = 128;

        public static readonly FieldMask[] AllWalls = new FieldMask[MaxWallCount];
        public static readonly byte[] AllIndexes = new byte[MaxWallCount];

        public static readonly byte[] NearEdgeWalls = new byte[16];

        // TODO index
        public static readonly Dictionary<(byte position, byte endPosition), byte> BehindPlayerWall = new();

        public static readonly byte[][] NearPlayerWalls = new byte[FieldMask.PlayerFieldArea][];
        public static readonly byte[][] NearWallsToCheck = new byte[MaxWallCount][];
        public static readonly byte[][] NearWallsToPlace = new byte[MaxWallCount][];
        public static readonly byte[][] OverlappedWalls = new byte[MaxWallCount][];

        static WallConstants()
        {
            GenerateAllWallMoves();
            GenerateNearEdgeWallMask();
            GenerateNearPlayerWalls();
            GenerateNearWallMasks();
            GenerateOverlappedWall();
        }

        public static void GenerateAllWallMoves()
        {
            var count = 0;
            for (var i = 1; i < FieldMask.BitboardSize; i += 2)
            {
                for (var j = 1; j < FieldMask.BitboardSize; j += 2)
                {
                    AllWalls[count++] = GenerateWall(i, j, WallOrientation.Horizontal);
                    AllWalls[count++] = GenerateWall(i, j, WallOrientation.Vertical);
                }
            }

            for (byte i = 0; i < 128; i++)
            {
                AllIndexes[i] = i;
            }
        }

        private static void GenerateNearEdgeWallMask()
        {
            var count = 0;
            for (var i = 1; i < FieldMask.BitboardSize; i += 2)
            {
                NearEdgeWalls[count++] = ToIndex(i, 1, WallOrientation.Horizontal);
                NearEdgeWalls[count++] = ToIndex(i, 15, WallOrientation.Horizontal);
                
                NearEdgeWalls[count++] = ToIndex(1, i, WallOrientation.Vertical);
                NearEdgeWalls[count++] = ToIndex(15, i, WallOrientation.Vertical);
            }
        }

        private static void GenerateNearPlayerWalls()
        {
            for (var i = 0; i < FieldMask.BitboardSize; i += 2)
            {
                for (var j = 0; j < FieldMask.BitboardSize; j += 2)
                {
                    var index = FieldMask.GetPlayerIndex(i, j);
                    NearPlayerWalls[index] = GetNearWalls(i, j);
                    SetUpWallMask(i, j, index);
                    SetDownWallMask(i, j, index);
                }
            }
        }

        private static byte[] GetNearWalls(int i, int j)
        {
            var walls = new List<byte>();
            foreach (var (offsetY, offsetX) in Constants.Diagonals)
            {
                var y = i + offsetY;
                var x = j + offsetX;
                if (FieldMask.IsInRange(y, x))
                {
                    walls.Add(ToIndex(y, x, WallOrientation.Horizontal));
                    walls.Add(ToIndex(y, x, WallOrientation.Vertical));
                }
            }

            return walls.ToArray();
        }

        private static void SetUpWallMask(int i, int j, byte position)
        {
            if (i == 0)
            {
                return;
            }

            j = j == 0 ? 1 : j - 1;
            i -= 1;
            BehindPlayerWall[(position, PlayerConstants.EndRedDownIndexIncluding)] =
                ToIndex(i, j, WallOrientation.Horizontal);
        }

        private static void SetDownWallMask(int i, int j, byte position)
        {
            if (i == 16)
            {
                return;
            }

            j = j == 0 ? 1 : j - 1;
            i += 1;
            BehindPlayerWall[(position, PlayerConstants.EndBlueDownIndexIncluding)] =
                ToIndex(i, j, WallOrientation.Horizontal);
        }

        private static void GenerateOverlappedWall()
        {
            var count = 0;
            for (var i = 1; i < FieldMask.BitboardSize; i += 2)
            {
                for (var j = 1; j < FieldMask.BitboardSize; j += 2)
                {
                    var currentNearWallsIndexes = new List<byte>();
                    AddIfInRange(i, j, WallOrientation.Vertical);
                    AddIfInRange(i, j - 2, WallOrientation.Horizontal);
                    AddIfInRange(i, j + 2, WallOrientation.Horizontal);
                    OverlappedWalls[count++] = currentNearWallsIndexes.ToArray();

                    currentNearWallsIndexes = new List<byte>();
                    AddIfInRange(i, j, WallOrientation.Horizontal);
                    AddIfInRange(i - 2, j, WallOrientation.Vertical);
                    AddIfInRange(i + 2, j, WallOrientation.Vertical);
                    OverlappedWalls[count++] = currentNearWallsIndexes.ToArray();

                    void AddIfInRange(int y, int x, WallOrientation orientation)
                    {
                        if (FieldMask.IsInRange(y, x))
                        {
                            currentNearWallsIndexes.Add(ToIndex(y, x, orientation));
                        }
                    }
                }
            }
        }

        private static void GenerateNearWallMasks()
        {
            byte count = 0;
            for (var i = 1; i < FieldMask.BitboardSize; i += 2)
            {
                for (var j = 1; j < FieldMask.BitboardSize; j += 2)
                {
                    var nearWalls = GenerateNearWallsForHorizontal(i, j, false);
                    NearWallsToCheck[count] = nearWalls;
                    nearWalls = GenerateNearWallsForHorizontal(i, j, true);
                    NearWallsToPlace[count] = nearWalls;
                    count++;

                    nearWalls = GenerateNearWallsForVertical(i, j, false);
                    NearWallsToCheck[count] = nearWalls;
                    nearWalls = GenerateNearWallsForVertical(i, j, true);
                    NearWallsToPlace[count] = nearWalls;
                    count++;
                }
            }
        }

        private static byte[] GenerateNearWallsForHorizontal(int i, int j, bool withAdditional)
        {
            var walls = new List<byte>();

            AddIfInRange(i, j - 4, WallOrientation.Horizontal);
            AddIfInRange(i, j + 4, WallOrientation.Horizontal);
            AddIfInRange(i + 2, j - 2, WallOrientation.Vertical);
            AddIfInRange(i + 2, j, WallOrientation.Vertical);
            AddIfInRange(i + 2, j + 2, WallOrientation.Vertical);
            AddIfInRange(i - 2, j - 2, WallOrientation.Vertical);
            AddIfInRange(i - 2, j, WallOrientation.Vertical);
            AddIfInRange(i - 2, j + 2, WallOrientation.Vertical);
            AddIfInRange(i, j - 2, WallOrientation.Vertical);
            AddIfInRange(i, j + 2, WallOrientation.Vertical);

            if (withAdditional)
            {
                AddIfInRange(i, j - 4, WallOrientation.Vertical);
                AddIfInRange(i, j + 4, WallOrientation.Vertical);
            }

            return walls.ToArray();

            void AddIfInRange(int y, int x, WallOrientation orientation)
            {
                if (FieldMask.IsInRange(y, x))
                {
                    walls.Add(ToIndex(y, x, orientation));
                }
            }
        }

        private static byte[] GenerateNearWallsForVertical(int i, int j, bool withAdditional)
        {
            var walls = new List<byte>();

            AddIfInRange(i - 4, j, WallOrientation.Vertical);
            AddIfInRange(i + 4, j, WallOrientation.Vertical);
            AddIfInRange(i - 2, j + 2, WallOrientation.Horizontal);
            AddIfInRange(i, j + 2, WallOrientation.Horizontal);
            AddIfInRange(i + 2, j + 2, WallOrientation.Horizontal);
            AddIfInRange(i - 2, j - 2, WallOrientation.Horizontal);
            AddIfInRange(i, j - 2, WallOrientation.Horizontal);
            AddIfInRange(i + 2, j - 2, WallOrientation.Horizontal);
            AddIfInRange(i - 2, j, WallOrientation.Horizontal);
            AddIfInRange(i + 2, j, WallOrientation.Horizontal);

            if (withAdditional)
            {
                AddIfInRange(i - 4, j, WallOrientation.Horizontal);
                AddIfInRange(i + 4, j, WallOrientation.Horizontal);
            }

            return walls.ToArray();

            void AddIfInRange(int y, int x, WallOrientation orientation)
            {
                if (FieldMask.IsInRange(y, x))
                {
                    walls.Add(ToIndex(y, x, orientation));
                }
            }
        }

        public static FieldMask GenerateWall(int y, int x, WallOrientation wallOrientation)
        {
            var (yOffset, xOffset) = GetOffset(wallOrientation);
            var wall = new FieldMask();
            for (var i = 0; i < WallSize; i++)
            {
                var placeY = y + (i - 1) * yOffset;
                var placeX = x + (i - 1) * xOffset;
                wall.SetBit(placeY, placeX, true);
            }

            return wall;
        }

        private static (int yOffset, int xOffset) GetOffset(WallOrientation wallOrientation)
        {
            var yOffset = wallOrientation == WallOrientation.Vertical ? 1 : 0;
            var xOffset = wallOrientation == WallOrientation.Horizontal ? 1 : 0;
            return (yOffset, xOffset);
        }

        public static byte ToIndex(int i, int j, WallOrientation wallOrientation)
        {
            i /= 2;
            j /= 2;
            var offset = wallOrientation == WallOrientation.Horizontal ? 0 : 1;
            return (byte) ((i * 8 + j) * 2 + offset);
        }

        public static (int i, int j, WallOrientation orientation) Flatten(byte wall)
        {
            var y = wall / 16;
            var x = (wall - y * 16) / 2;
            var orientation = wall % 2 == 0 ? WallOrientation.Horizontal : WallOrientation.Vertical;
            return (y, x, orientation);
        }
    }
}