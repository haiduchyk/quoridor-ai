namespace Quoridor.Model.Strategies
{
    using System;
    using System.Collections.Generic;

    public class WithEnemyMoveCalculator
    {
        // <playerPosition 81 <enemyPosition 4 <wallMask 64, possibleMoves>>>
        private Dictionary<FieldMask, Dictionary<FieldMask, Dictionary<FieldMask, FieldMask[]>>> withEnemyPlayersMoves =
            new();

        //  <playerPosition 81, <enemyPosition 4, wallMaskForThisPlayerAndEnemyPosition>83 8
        private Dictionary<FieldMask, Dictionary<FieldMask, FieldMask>> withEnemyPlayersMovesMasks = new();

        public FieldMask[] GetAvailableMoves(Field field, FieldMask playerMask, FieldMask enemyMask)
        {
            // Actual code but we optimize these shit
            // var moves = withEnemyPlayersMoves[playerMask][enemyMask];
            // var wallMask = withEnemyPlayersMovesMasks[playerMask][enemyMask];
            // var currentWallMask = field.GetWallsForMask(wallMask);
            // return moves[currentWallMask];
            return withEnemyPlayersMoves[playerMask][enemyMask][field.GetWallsForMask(withEnemyPlayersMovesMasks[playerMask][enemyMask])];
        }

        public WithEnemyMoveCalculator()
        {
            CreateWithEnemyPlayerMovesMasks();
            CreateWithEnemyPlayerMoves();
        }

        private void CreateWithEnemyPlayerMovesMasks()
        {
            for (var y = 0; y < FieldMask.BitboardSize; y++)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x++)
                {
                    if (y % 2 == 0 && x % 2 == 0)
                    {
                        var playerMask = new FieldMask();
                        playerMask.SetBit(y, x, true);
                        withEnemyPlayersMovesMasks[playerMask] = CreateWithEnemyPlayerMovesMaskFor(y, x);
                    }
                }
            }
        }

        private Dictionary<FieldMask, FieldMask> CreateWithEnemyPlayerMovesMaskFor(int y, int x)
        {
            var result = new Dictionary<FieldMask, FieldMask>();
            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var curY = y + yDelta * 2;
                var curX = x + xDelta * 2;

                var enemyMask = new FieldMask();

                if (FieldMask.IsInRange(curY, curX))
                {
                    enemyMask.SetBit(curY, curX, true);
                    result[enemyMask] = GetWallMaskFor(y, x, curY, curX);
                }
            }

            return result;
        }

        private FieldMask GetWallMaskFor(int yPlayer, int xPlayer, int yEnemy, int xEnemy)
        {
            var wallMask = new FieldMask();

            var blockedY = (yEnemy - yPlayer) / 2 + yPlayer;
            var blockedX = (xEnemy - xPlayer) / 2 + xPlayer;

            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var wallNearEnemyY = yEnemy + yDelta;
                var wallNearEnemyX = xEnemy + xDelta;

                var wallNearPlayerY = yPlayer + yDelta;
                var wallNearPlayerX = xPlayer + xDelta;

                SetIfNotBlocked(wallNearEnemyY, wallNearEnemyX);
                SetIfNotBlocked(wallNearPlayerY, wallNearPlayerX);
            }

            void SetIfNotBlocked(int y, int x)
            {
                if (y != blockedY || x != blockedX)
                {
                    wallMask.TrySetBit(y, x, true);
                }
            }

            return wallMask;
        }

        private void CreateWithEnemyPlayerMoves()
        {
            for (var y = 0; y < FieldMask.BitboardSize; y++)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x++)
                {
                    // if (y != 8 || x != 8)
                    // {
                    //     continue;
                    // }

                    if (y % 2 == 0 && x % 2 == 0)
                    {
                        var playerMask = new FieldMask();
                        playerMask.SetBit(y, x, true);

                        var moves = CreateWithEnemyPlayerMovesFor(y, x);
                        withEnemyPlayersMoves[playerMask] = moves;
                    }
                }
            }
        }

        private Dictionary<FieldMask, Dictionary<FieldMask, FieldMask[]>> CreateWithEnemyPlayerMovesFor(int y, int x)
        {
            var result = new Dictionary<FieldMask, Dictionary<FieldMask, FieldMask[]>>();
            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var curY = y + yDelta * 2;
                var curX = x + xDelta * 2;

                var enemyMask = new FieldMask();

                if (FieldMask.IsInRange(curY, curX))
                {
                    enemyMask.SetBit(curY, curX, true);
                    result[enemyMask] = CalculateUniqueVariantFor(y, x, curY, curX);
                }
            }

            return result;
        }


        private Dictionary<FieldMask, FieldMask[]> CalculateUniqueVariantFor(int yPlayer, int xPlayer, int yEnemy,
            int xEnemy)
        {
            var result = new Dictionary<FieldMask, FieldMask[]>();

            var uniqueVariants = Math.Pow(2, 6);

            for (var unique = 0; unique < uniqueVariants; unique++)
            {
                var playerMoveMasks = new List<FieldMask>();
                var wallMask = new FieldMask();

                var uniquePosition = 0;

                var blockedY = (yEnemy - yPlayer) / 2 + yPlayer;
                var blockedX = (xEnemy - xPlayer) / 2 + xPlayer;

                var importantY = (yEnemy - yPlayer) + blockedY;
                var importantX = (xEnemy - xPlayer) + blockedX;

                foreach (var (yDelta, xDelta) in Constants.Directions)
                {
                    var wallNearPlayerY = yPlayer + yDelta;
                    var wallNearPlayerX = xPlayer + xDelta;

                    UpdateFieldMask(yPlayer, xPlayer, wallNearPlayerY, wallNearPlayerX, yDelta, xDelta, false);
                }

                UpdateFieldMask(yEnemy, xEnemy, importantY, importantX, (yEnemy - yPlayer) / 2,
                    (xEnemy - xPlayer) / 2, false);


                foreach (var (yDelta, xDelta) in Constants.Directions)
                {
                    var wallNearEnemyY = yEnemy + yDelta;
                    var wallNearEnemyX = xEnemy + xDelta;
                    if (importantX != wallNearEnemyX && importantY != wallNearEnemyY)
                    {
                        UpdateFieldMask(yEnemy, xEnemy, wallNearEnemyY, wallNearEnemyX, yDelta, xDelta, true);
                    }
                }

                void UpdateFieldMask(int playerY, int playerX, int wallY, int wallX, int yDelta, int xDelta,
                    bool isBigJump)
                {
                    if (wallY == blockedY && wallX == blockedX)
                    {
                        return;
                    }

                    var isTrue = (1 << uniquePosition & unique) != 0;

                    uniquePosition++;

                    if (FieldMask.IsInRange(wallY, wallX))
                    {
                        wallMask.SetBit(wallY, wallX, isTrue);

                        if (!isTrue)
                        {
                            if (isBigJump)
                            {
                                if (wallMask.GetBit(importantY, importantX))
                                {
                                    SimpleUpdate();
                                }
                            }
                            else
                            {
                                SimpleUpdate();
                            }
                        }
                    }

                    void SimpleUpdate()
                    {
                        var newPlayerX = playerX + xDelta * 2;
                        var newPlayerY = playerY + yDelta * 2;

                        if (FieldMask.IsInRange(newPlayerY, newPlayerX))
                        {
                            var playerMask = new FieldMask();
                            playerMask.SetBit(newPlayerY, newPlayerX, true);
                            playerMoveMasks.Add(playerMask);
                        }
                    }
                }

                result[wallMask] = playerMoveMasks.ToArray();
            }

            return result;
        }
    }
}
