namespace Quoridor.Model.Strategies
{
    using System;
    using System.Collections.Generic;

    public class WithEnemyMoveCalculator
    {
        // <playerPosition 81 <enemyPosition 4 <wallMask 64, possibleMoves>>>
        private Dictionary<(FieldMask playerPosition, FieldMask enemyPosition, FieldMask wallMask), FieldMask[]>
            withEnemyPlayersMoves =
                new();

        //  <playerPosition 81, <enemyPosition 4, wallMaskForThisPlayerAndEnemyPosition>
        private Dictionary<(FieldMask, FieldMask), FieldMask> withEnemyPlayersMovesMasks = new();

        public FieldMask[] GetAvailableMoves(Field field, in FieldMask playerMask, in FieldMask enemyMask)
        {
            // Actual code 
            // var wallMask = withEnemyPlayersMovesMasks[(playerMask, enemyMask)];
            // var currentWallMask = field.GetWallsForMask(wallMask);
            // return withEnemyPlayersMoves[(playerMask, enemyMask, currentWallMask)];
            return withEnemyPlayersMoves[
                (playerMask, enemyMask, field.GetWallsForMask(withEnemyPlayersMovesMasks[(playerMask, enemyMask)]))];
        }

        public WithEnemyMoveCalculator()
        {
            CreateWithEnemyPlayerMovesMasks();
            CreateWithEnemyPlayerMoves();
        }

        private void CreateWithEnemyPlayerMovesMasks()
        {
            for (var y = 0; y < FieldMask.BitboardSize; y += 2)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x += 2)
                {
                    var playerMask = new FieldMask();
                    playerMask.SetBit(y, x, true);
                    var variants = CreateWithEnemyPlayerMovesMaskFor(y, x);
                    foreach (var (enemyPosition, wallMask) in variants)
                    {
                        withEnemyPlayersMovesMasks[(playerMask, enemyPosition)] = wallMask;
                    }
                }
            }
        }

        private List<(FieldMask enemyPosition, FieldMask wallMask)> CreateWithEnemyPlayerMovesMaskFor(int y, int x)
        {
            var result = new List<(FieldMask enemyPosition, FieldMask wallMask)>();
            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var curY = y + yDelta * 2;
                var curX = x + xDelta * 2;

                var enemyMask = new FieldMask();

                if (FieldMask.IsInRange(curY, curX))
                {
                    enemyMask.SetBit(curY, curX, true);
                    result.Add((enemyMask, GetWallMaskFor(y, x, curY, curX)));
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
            for (var y = 0; y < FieldMask.BitboardSize; y += 2)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x += 2)
                {
                    var playerMask = new FieldMask();
                    playerMask.SetBit(y, x, true);

                    var moves = CreateWithEnemyPlayerMovesFor(y, x);
                    foreach (var (enemyPosition, wallMoves) in moves)
                    {
                        foreach (var (wallMask, movesPositions) in wallMoves)
                        {
                            withEnemyPlayersMoves[(playerMask, enemyPosition, wallMask)] =
                                movesPositions;
                        }
                    }
                }
            }
        }

        private List<(FieldMask enemyPosition, List<(FieldMask wallMask, FieldMask[] movesPositions)> wallMoves)>
            CreateWithEnemyPlayerMovesFor(int y, int x)
        {
            var result = new List<(FieldMask, List<(FieldMask, FieldMask[])>)>();
            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var curY = y + yDelta * 2;
                var curX = x + xDelta * 2;

                var enemyMask = new FieldMask();

                if (FieldMask.IsInRange(curY, curX))
                {
                    enemyMask.SetBit(curY, curX, true);
                    result.Add((enemyMask, CalculateUniqueVariantFor(y, x, curY, curX)));
                }
            }

            return result;
        }


        private List<(FieldMask, FieldMask[])> CalculateUniqueVariantFor(int yPlayer, int xPlayer, int yEnemy,
            int xEnemy)
        {
            var result = new List<(FieldMask, FieldMask[])>();

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
                                if (!FieldMask.IsInRange(importantY, importantX) ||
                                    wallMask.GetBit(importantY, importantX))
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

                result.Add((wallMask, playerMoveMasks.ToArray()));
            }

            return result;
        }
    }
}