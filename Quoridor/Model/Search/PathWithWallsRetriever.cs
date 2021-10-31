namespace Quoridor.Model
{
    using System.Collections.Generic;
    using Strategies;

    public class PathWithWallsRetriever
    {
        // <playerPosition <nextPlayerPosition, mask>>
        private readonly Dictionary<byte, Dictionary<byte, FieldMask>> simpleMoveMasks = new();

        // <playerPosition <enemyPosition <nextPlayerPosition, mask>>>
        private readonly Dictionary<byte, Dictionary<byte, Dictionary<byte, FieldMask>>> withEnemyMoveMasks = new();

        public FieldMask RetrievePath(byte playerPosition, Dictionary<byte, (byte mask, bool isSimple)> prevNodes,
            byte enemyPosition)
        {
            var path = new FieldMask();
            path = path.Or(PlayerConstants.allPositions[playerPosition]);
            var prevNode = prevNodes[playerPosition];
            var nextPosition = prevNode.mask;

            while (nextPosition != Constants.EmptyIndex)
            {
                var mask = prevNode.isSimple
                    ? simpleMoveMasks[nextPosition][playerPosition]
                    : withEnemyMoveMasks[nextPosition][enemyPosition][playerPosition];
                path = mask.Or(path);
                playerPosition = nextPosition;
                prevNode = prevNodes[playerPosition];
                nextPosition = prevNode.mask;
            }

            path = path.Or(PlayerConstants.allPositions[playerPosition]);
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
                        var index = FieldMask.GetPlayerIndex(y, x);
                        var moves = CreateSimpleMoveMask(y, x);
                        simpleMoveMasks[index] = moves;
                    }
                }
            }
        }

        private Dictionary<byte, FieldMask> CreateSimpleMoveMask(int y, int x)
        {
            var result = new Dictionary<byte, FieldMask>();
            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var nextY = y + yDelta * 2;
                var nextX = x + xDelta * 2;

                var wallY = y + yDelta;
                var wallX = x + xDelta;

                if (FieldMask.IsInRange(nextY, nextX))
                {
                    var index = FieldMask.GetPlayerIndex(nextY, nextX);

                    var resultMask = new FieldMask();
                    resultMask.SetBit(nextY, nextX, true);
                    resultMask.SetBit(wallY, wallX, true);
                    result[index] = resultMask;
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
                        var index = FieldMask.GetPlayerIndex(y, x);
                        var moves = CreateWithEnemyMoveMask(y, x);
                        withEnemyMoveMasks[index] = moves;
                    }
                }
            }
        }

        private Dictionary<byte, Dictionary<byte, FieldMask>> CreateWithEnemyMoveMask(int y, int x)
        {
            var result = new Dictionary<byte, Dictionary<byte, FieldMask>>();
            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var curY = y + yDelta * 2;
                var curX = x + xDelta * 2;

                if (FieldMask.IsInRange(curY, curX))
                {
                    var index = FieldMask.GetPlayerIndex(curY, curX);
                    var wallMasks = CalculateVariantFor(y, x, curY, curX);
                    result[index] = wallMasks;
                }
            }

            return result;
        }

        private Dictionary<byte, FieldMask> CalculateVariantFor(int yPlayer, int xPlayer, int yEnemy,
            int xEnemy)
        {
            var result = new Dictionary<byte, FieldMask>();

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
                        var index = FieldMask.GetPlayerIndex(nextPlayerY, nextPlayerX);
                        result[index] = wallMask.Or(playerMask);
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
                    var index = FieldMask.GetPlayerIndex(nextPlayerY, nextPlayerX);
                    result[index] = wallMask.Or(playerMask);
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
                        var index = FieldMask.GetPlayerIndex(nextPlayerY, nextPlayerX);
                        result[index] = wallMask.Or(playerMask);
                    }
                }
            }

            return result;
        }
    }
}
