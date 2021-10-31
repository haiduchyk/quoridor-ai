namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;

    public static class WallConstants
    {
        private const int WallSize = 3;
        public const int MaxWallCount = 128;
        public static FieldMask[] allWalls;
        public static FieldMask nearEdgeWallMask;
        public static readonly Dictionary<(FieldMask position, FieldMask endPosition), FieldMask> behindPlayerWall = new();
        public static readonly Dictionary<FieldMask, FieldMask> nearPlayerWallsMasks = new();
        public static readonly Dictionary<FieldMask, FieldMask> nearWallsMasks = new();
        public static readonly Dictionary<FieldMask, FieldMask[]> nearWalls = new();

        static WallConstants()
        {
            GenerateAllWallMoves();
            GenerateNearEdgeWallMask();
            GenerateNearPlayerWalls();
            GenerateNearWallMasks();
            GenerateNearWall();
        }
        
        public static void GenerateAllWallMoves()
        {
            allWalls = new FieldMask[MaxWallCount];
            var count = 0;
            for (var i = 1; i < FieldMask.BitboardSize; i += 2)
            {
                for (var j = 1; j < FieldMask.BitboardSize; j += 2)
                {
                    allWalls[count++] = GenerateWall(i, j, WallOrientation.Horizontal);
                    allWalls[count++] = GenerateWall(i, j, WallOrientation.Vertical);
                }
            }
        }

        private static void GenerateNearEdgeWallMask()
        {
            for (var i = 1; i < FieldMask.BitboardSize; i += 2)
            {
                nearEdgeWallMask.SetBit(i, 0, true);
                nearEdgeWallMask.SetBit(i, FieldMask.BitboardSize - 1, true);
            }
        }

        private static void GenerateNearPlayerWalls()
        {
            for (var i = 0; i < FieldMask.BitboardSize; i += 2)
            {
                for (var j = 0; j < FieldMask.BitboardSize; j += 2)
                {
                    var position = new FieldMask();
                    position.SetBit(i, j, true);
                    nearPlayerWallsMasks[position] = GetWallMask(i, j);
                    SetUpWallMask(i, j, position);
                    SetDownWallMask(i, j, position);
                }
            }
        }
        
        private static FieldMask GetWallMask(int i, int j)
        {
            var wallMask = new FieldMask();
            foreach (var (offsetY, offsetX) in Constants.Directions)
            {
                var y = i + offsetY;
                var x = j + offsetX;
                wallMask.TrySetBit(y, x, true);
            }

            return wallMask;
        }

        private static void SetUpWallMask(int i, int j, FieldMask position)
        {
            if (i == 0)
            {
                return;
            }

            j = j == 0 ? 1 : j - 1;
            i -= 1;
            var wallMask = GenerateWall(i, j, WallOrientation.Horizontal);
            behindPlayerWall[(position, Constants.RedEndPositions)] = wallMask;
        }

        private static void SetDownWallMask(int i, int j, FieldMask position)
        {
            if (i == 16)
            {
                return;
            }

            j = j == 0 ? 1 : j - 1;
            i += 1;
            var wallMask = GenerateWall(i, j, WallOrientation.Horizontal);
            behindPlayerWall[(position, Constants.BlueEndPositions)] = wallMask;
        }
        
         private static void GenerateNearWall()
        {
            for (var i = 0; i < allWalls.Length; i++)
            {
                var currentNearWalls = new List<FieldMask>();
                var currentWall = allWalls[i];
                if (i % 2 == 0)
                {
                    AddIfInRange(i + 1);
                    AddIfInRange(i - 2);
                    AddIfInRange(i + 2);
                }
                else
                {
                    AddIfInRange(i - 1);
                    AddIfInRange(i - FieldMask.BitboardSize + 1);
                    AddIfInRange(i + FieldMask.BitboardSize - 1);
                }
                
                nearWalls[currentWall] = currentNearWalls.ToArray();

                void AddIfInRange(int index)
                {
                    if (index >= 0 && index < allWalls.Length)
                    {
                        var wall = allWalls[index];
                        currentNearWalls.Add(wall);
                    }
                }
            }
        }


        private static void GenerateNearWallMasks()
        {
            var count = 0;
            for (var i = 1; i < FieldMask.BitboardSize; i += 2)
            {
                for (var j = 1; j < FieldMask.BitboardSize; j += 2)
                {
                    var horizontal = allWalls[count++];
                    var nearWallsMask = GenerateNearWallsForHorizontal(i, j);
                    nearWallsMasks[horizontal] = nearWallsMask;

                    var vertical = allWalls[count++];
                    nearWallsMask = GenerateNearWallsForVertical(i, j);
                    nearWallsMasks[vertical] = nearWallsMask;
                }
            }
        }


        // TODO how fast and good with this flag on and off
        private static bool needUpPoint = false;

        private static FieldMask GenerateNearWallsForHorizontal(int i, int j)
        {
            var mask = new FieldMask();
            foreach (var (offsetY, offsetX) in Constants.Directions)
            {
                mask.TrySetBit(i + offsetY, j + 2 * offsetX, true);
                if (needUpPoint || offsetY == 0)
                {
                    mask.TrySetBit(i + 2 * offsetY, j + 3 * offsetX, true);
                }
            }

            foreach (var (offsetY, offsetX) in Constants.Diagonals)
            {
                mask.TrySetBit(i + offsetY, j + 2 * offsetX, true);
            }

            return mask;
        }

        private static FieldMask GenerateNearWallsForVertical(int i, int j)
        {
            var mask = new FieldMask();
            foreach (var (offsetY, offsetX) in Constants.Directions)
            {
                mask.TrySetBit(i + 2 * offsetY, j + offsetX, true);
                if (needUpPoint || offsetX == 0)
                {
                    mask.TrySetBit(i + 3 * offsetY, j + 2 * offsetX, true);
                }
            }

            foreach (var (offsetY, offsetX) in Constants.Diagonals)
            {
                mask.TrySetBit(i + 2 * offsetY, j + offsetX, true);
            }

            return mask;
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
    }
}