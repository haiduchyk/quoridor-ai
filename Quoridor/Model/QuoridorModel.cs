namespace Quoridor.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        private Dictionary<FieldMask, Dictionary<FieldMask, FieldMask[]>> simplePlayersMoves = new();
        private Dictionary<FieldMask, FieldMask> simplePlayersMovesMasks = new();
        private Random random = new();

        public QuoridorModel()
        {
            CreateSimplePlayerMovesMasks();
            CreateSimplePlayerMoves();
            CreateAvailableWallsMask();
            PutPlayersOnStartPosition();

            SetupSimpleCorridor();

            walls.ToStr(blueCharacterStart, redCharacterStart).Log();

            for (var i = 0; i < 100; i++)
            {
                MakeRandomRedMove();
                MakeRandomBlueMove();
                walls.ToStr(blueCharacter, redCharacter).Log();
            }


            walls.ToStr(blueCharacterStart, redCharacterStart).Log();
            // var a = (long)~0 >> (QuoridorModel.BitsBlockSize - 9 - 1);
            redCharacterStart.ToStr().Log();
        }

        private void PutPlayersOnStartPosition()
        {
            redCharacter = redCharacterStart;
            blueCharacter = blueCharacterStart;
        }

        private void SetupSimpleCorridor()
        {
            PlaceWall(1, 1, WallOrientation.Horizontal);
            PlaceWall(1, 3, WallOrientation.Horizontal);
            PlaceWall(1, 5, WallOrientation.Horizontal);
            PlaceWall(1, 7, WallOrientation.Horizontal);
            PlaceWall(1, 9, WallOrientation.Horizontal);
            PlaceWall(1, 11, WallOrientation.Horizontal);
            PlaceWall(1, 13, WallOrientation.Horizontal);
        }

        private void MakeRandomRedMove()
        {
            redCharacter = MakeRandomMove(redCharacter);
        }

        private void MakeRandomBlueMove()
        {
            blueCharacter = MakeRandomMove(blueCharacter);
        }

        private FieldMask MakeRandomMove(FieldMask playerMask)
        {
            var moves = simplePlayersMoves[playerMask];
            var wallMask = simplePlayersMovesMasks[playerMask];
            var currentWallMask = wallMask.And(walls);
            var availableMoves = moves[currentWallMask];
            var move = availableMoves[random.Next(0, availableMoves.Length)];
            return move;
        }

        private void CreateSimplePlayerMovesMasks()
        {
            for (var y = 0; y < BitboardSize; y++)
            {
                for (var x = 0; x < BitboardSize; x++)
                {
                    if (y % 2 == 0 && x % 2 == 0)
                    {
                        var playerMask = new FieldMask();
                        playerMask.SetBit(y, x, true);
                        simplePlayersMovesMasks[playerMask] = CreateSimplePlayerMovesMaskFor(y, x);
                    }
                }
            }
        }

        private FieldMask CreateSimplePlayerMovesMaskFor(int y, int x)
        {
            var fieldMask = new FieldMask();
            for (var yDelta = -SimpleMoveBitsAmount / 2; yDelta < SimpleMoveBitsAmount / 2 + 1; yDelta++)
            {
                for (var xDelta = -SimpleMoveBitsAmount / 2; xDelta < SimpleMoveBitsAmount / 2 + 1; xDelta++)
                {
                    var curY = y + yDelta;
                    var curX = x + xDelta;
                    CalculateSimplePlayerMovesMaskFor(curY, curX, ref fieldMask);
                }
            }

            return fieldMask;
        }

        private void CalculateSimplePlayerMovesMaskFor(int curY, int curX, ref FieldMask fieldMask)
        {
            if (curY % 2 != 0)
            {
                if (curX % 2 == 0)
                {
                    TrySetBit(curY, curX, ref fieldMask);
                }
            }
            else
            {
                if (curX % 2 != 0)
                {
                    TrySetBit(curY, curX, ref fieldMask);
                }
            }
        }

        private void TrySetBit(int y, int x, ref FieldMask mask)
        {
            if (IsInRange(y, x))
            {
                mask.SetBit(y, x, true);
            }
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

        private void CreateSimplePlayerMoves()
        {
            for (var y = 0; y < BitboardSize; y++)
            {
                for (var x = 0; x < BitboardSize; x++)
                {
                    if (y % 2 == 0 && x % 2 == 0)
                    {
                        var playerMask = new FieldMask();
                        playerMask.SetBit(y, x, true);
                        var moves = CalculateUniqueVariantFor(y, x);
                        simplePlayersMoves[playerMask] = moves;
                    }
                }
            }
        }

        private Dictionary<FieldMask, FieldMask[]> CalculateUniqueVariantFor(int y, int x)
        {
            var result = new Dictionary<FieldMask, FieldMask[]>();

            var uniqueVariants = Math.Pow(4, 2);

            for (var unique = 0; unique < uniqueVariants; unique++)
            {
                var playerMoveMasks = new List<FieldMask>();
                var fieldMask = new FieldMask();

                var uniquePosition = 0;
                for (var yDelta = -SimpleMoveBitsAmount / 2; yDelta < SimpleMoveBitsAmount / 2 + 1; yDelta++)
                {
                    for (var xDelta = -SimpleMoveBitsAmount / 2; xDelta < SimpleMoveBitsAmount / 2 + 1; xDelta++)
                    {
                        var curY = y + yDelta;
                        var curX = x + xDelta;

                        if (curY % 2 != 0)
                        {
                            if (curX % 2 == 0)
                            {
                                UpdateFieldMask();
                            }
                        }
                        else
                        {
                            if (curX % 2 != 0)
                            {
                                UpdateFieldMask();
                            }
                        }

                        void UpdateFieldMask()
                        {
                            var isTrue = (1 << uniquePosition & unique) != 0;
                            if (IsInRange(curY, curX))
                            {
                                fieldMask.SetBit(curY, curX, isTrue);

                                if (!isTrue)
                                {
                                    var newPlayerX = x + xDelta * 2;
                                    var newPlayerY = y + yDelta * 2;

                                    if (IsInRange(newPlayerY, newPlayerX))
                                    {
                                        var playerMask = new FieldMask();
                                        playerMask.SetBit(newPlayerY, newPlayerX, true);
                                        playerMoveMasks.Add(playerMask);
                                    }
                                }
                            }

                            uniquePosition++;
                        }
                    }
                }

                result[fieldMask] = playerMoveMasks.ToArray();
            }

            return result;
        }
        
        public bool IsInRange(int index)
        {
            return index is >= 0 and < UsedBitsAmount;
        }

        public bool IsInRange(int y, int x)
        {
            return y is >= 0 and < BitboardSize && x is >= 0 and < BitboardSize;
        }

        public bool CanMove(FieldMask moveMask)
        {
            return walls.And(moveMask).IsZero();
        }
    }
}
