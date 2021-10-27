namespace Quoridor.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PathWithWallsRetriever
    {
        // <playerPosition <nextPLayerPosition, mask>>
        private Dictionary<FieldMask, Dictionary<FieldMask, FieldMask>> simpleMoveMasks = new();

        // <playerPosition <enemyPosition <nextPLayerPosition, mask>>>
        private Dictionary<FieldMask, Dictionary<FieldMask, Dictionary<FieldMask, FieldMask>>> withEnemyMoveMasks
            = new();

        public FieldMask RetrievePath(FieldMask playerPosition,
            Dictionary<FieldMask, (FieldMask mask, bool isSimple)> prevNodes, FieldMask enemyPosition)
        {
            var path = new FieldMask();
            path = path.Or(playerPosition);
            var prevNode = prevNodes[playerPosition];
            var nextPosition = prevNode.mask;
            
            while (!nextPosition.Equals(Constants.EmptyField))
            {
                if (prevNode.isSimple)
                {
                    path = simpleMoveMasks[playerPosition][nextPosition].Or(path);
                }
                else
                {
                    path = withEnemyMoveMasks[playerPosition][enemyPosition][nextPosition].Or(path);
                }

                playerPosition = nextPosition;
                prevNode = prevNodes[playerPosition];
                nextPosition = prevNode.mask;
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

                var playerNextMask = new FieldMask();
                var resultMask = new FieldMask();

                if (FieldMask.IsInRange(nextY, nextX))
                {
                    playerNextMask.SetBit(nextY, nextX, true);

                    resultMask.SetBit(nextY, nextX, true);
                    resultMask.SetBit(wallY, wallX, true);

                    result[playerNextMask] = resultMask;
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

        private Dictionary<FieldMask, Dictionary<FieldMask, FieldMask>> CreateWithEnemyMoveMask(int y, int x)
        {
            var result = new Dictionary<FieldMask, Dictionary<FieldMask, FieldMask>>();
            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var curY = y + yDelta * 2;
                var curX = x + xDelta * 2;

                var enemyMask = new FieldMask();

                if (FieldMask.IsInRange(curY, curX))
                {
                    enemyMask.SetBit(curY, curX, true);
                    var wallMasks = CalculateVariantFor(y, x, curY, curX);
                    result[enemyMask] = wallMasks;
                }
            }

            return result;
        }


        private Dictionary<FieldMask, FieldMask> CalculateVariantFor(int yPlayer, int xPlayer, int yEnemy,
            int xEnemy)
        {
            var result = new Dictionary<FieldMask, FieldMask>();

            var blockedY = (yEnemy - yPlayer) / 2 + yPlayer;
            var blockedX = (xEnemy - xPlayer) / 2 + xPlayer;

            var importantY = (yEnemy - yPlayer) + blockedY;
            var importantX = (xEnemy - xPlayer) + blockedX;

            // three simple moves
            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var wallNearPlayerY = yPlayer + yDelta;
                var wallNearPlayerX = xPlayer + xDelta;
                
                var nextPlayerY = yPlayer + yDelta * 2;
                var nextPlayerX = xPlayer + xDelta * 2;

                if (wallNearPlayerY != blockedY || wallNearPlayerX != blockedX)
                {
                    var wallMask = new FieldMask();
                    var playerMask = new FieldMask();
                    if (FieldMask.IsInRange(wallNearPlayerY, wallNearPlayerX))
                    {
                        wallMask.SetBit(wallNearPlayerY, wallNearPlayerX, true);
                        playerMask.SetBit(nextPlayerY, nextPlayerX, true);
                        result[playerMask] = wallMask.Or(playerMask);
                    }
                }
            }

            // one long jump
            CheckLongJump();
            void CheckLongJump()
            {
                var nextPlayerX = xEnemy + (xEnemy - xPlayer);
                var nextPlayerY = yEnemy + (yEnemy - yPlayer);
                
                var wallMask = new FieldMask();
                var playerMask = new FieldMask();
                
                if (FieldMask.IsInRange(nextPlayerY, nextPlayerX))
                {
                    wallMask.SetBit(blockedY, blockedX, true);
                    wallMask.SetBit(importantY, importantX, true);
                    playerMask.SetBit(nextPlayerY, nextPlayerX, true);
                    result[playerMask] = wallMask.Or(playerMask);
                }
            }

            // two diagonal jumps
            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var wallNearEnemyY = yEnemy + yDelta;
                var wallNearEnemyX = xEnemy + xDelta;
                
                var nextPlayerY = yEnemy + yDelta * 2;
                var nextPlayerX = xEnemy + xDelta * 2;
                
                if ((importantX != wallNearEnemyX || importantY != wallNearEnemyY) &&
                    (wallNearEnemyY != blockedY || wallNearEnemyX != blockedX))
                {
                    if (FieldMask.IsInRange(nextPlayerY, nextPlayerX))
                    {
                        var wallMask = new FieldMask();
                        var playerMask = new FieldMask();

                        wallMask.SetBit(wallNearEnemyY, wallNearEnemyX, true);
                        wallMask.SetBit(blockedY, blockedX, true);
                        playerMask.SetBit(nextPlayerY, nextPlayerX, true);
                        result[playerMask] = wallMask.Or(playerMask);
                    }
                }
            }

            return result;
        }
    }
}