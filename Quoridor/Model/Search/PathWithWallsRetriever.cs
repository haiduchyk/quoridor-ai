namespace Quoridor.Model
{
    using System;
    using System.Collections.Generic;

    public class PathWithWallsRetriever
    {
        private Dictionary<FieldMask, Dictionary<FieldMask, FieldMask>> simpleMoveMasks = new();

        private Dictionary<FieldMask, Dictionary<FieldMask, Dictionary<FieldMask, FieldMask>>> withEnemyMoveMasks 
            = new();

        public FieldMask RetrievePath(FieldMask position,
            Dictionary<FieldMask, (FieldMask mask, bool isSimple)> prevNodes, FieldMask enemy)
        {
            var path = new FieldMask();
            while (!position.Equals(Constants.EmptyField))
            {
                path = path.Or(position);
                var prevNode = prevNodes[position];
                position = prevNode.mask;
            }

            return path;
        }

        public PathWithWallsRetriever()
        {
            CreateSimplePlayerMoves();
            CreateWithEnemyMoves();
        }

        private void CreateSimplePlayerMoves()
        {
            for (var y = 0; y < FieldMask.BitboardSize; y++)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x++)
                {
                    if (y % 2 == 0 && x % 2 == 0)
                    {
                        var playerMask = new FieldMask();
                        playerMask.SetBit(y, x, true);

                        var moves = CreateSimpleMoveMask(y, x);
                        simpleMoveMasks[playerMask] = moves;
                    }
                }
            }
        }


        private Dictionary<FieldMask, FieldMask> CreateSimpleMoveMask(int y, int x)
        {
            var result = new Dictionary<FieldMask, FieldMask>();
            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var nextY = y + yDelta * 2;
                var nextX = x + xDelta * 2;

                var wallY = y + yDelta;
                var wallX = x + xDelta;

                var enemyMask = new FieldMask();
                var resultMask = new FieldMask();

                if (FieldMask.IsInRange(nextY, nextX))
                {
                    enemyMask.SetBit(nextY, nextX, true);

                    resultMask.SetBit(nextY, nextX, true);
                    resultMask.SetBit(wallY, wallX, true);

                    result[enemyMask] = resultMask;
                }
            }

            return result;
        }


        private void CreateWithEnemyMoves()
        {
            for (var y = 0; y < FieldMask.BitboardSize; y++)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x++)
                {
                    if (y % 2 == 0 && x % 2 == 0)
                    {
                        var playerMask = new FieldMask();
                        playerMask.SetBit(y, x, true);

                        var moves = CreateWithEnemyMoveMask(y, x);
                        withEnemyMoveMasks[playerMask] = moves;
                    }
                }
            }
        }

        private Dictionary<FieldMask, FieldMask> CreateWithEnemyMoveMask(int y, int x)
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
                    var wallMask = CalculateUniqueVariantFor(y, x, curY, curX);
                    result[enemyMask] = enemyMask.Or(wallMask);
                }
            }

            return result;
        }


        private FieldMask CalculateUniqueVariantFor(int yPlayer, int xPlayer, int yEnemy, int xEnemy)
        {
            var result = new FieldMask();

            var uniqueVariants = Math.Pow(2, 6);


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

                var isTrue = (1 << uniquePosition) != 0;

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

            return result;
        }
    }
}