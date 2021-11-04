namespace Quoridor.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Players;
    using Strategies;

    public interface IMoveProvider
    {
        byte[] GetAvailableMoves(Field field, in byte playerIndex, in byte enemyIndex);

        (byte[] indexes, bool isSimple) GetAvailableMovesWithType(Field field, in byte playerMask, in byte enemyMask);

        int GetRow(in byte moveIndex);

        // bool TryMoveForward(Field field, in FieldMask playerMask, out FieldMask moveMask);

        bool IsSimple(Field field, in byte playerIndex, byte moveIndex);

        bool CanJump(Field field, Player player, out byte jump);
    }

    public class MoveProvider : IMoveProvider
    {
        //  <playerPosition 81, <enemyPosition ~3-4, wallInBetweenPosition>>
        private Dictionary<(byte playerPosition, byte enemyPosition), FieldMask> withEnemyMoveMasks =
            new();

        private readonly Dictionary<byte, int> rows = new();

        private readonly SimpleMoveCalculator simpleMoveCalculator;
        private readonly WithEnemyMoveCalculator withEnemyMoveCalculator;

        public MoveProvider()
        {
            CreateEnemyPlayerMasks();
            simpleMoveCalculator = new SimpleMoveCalculator();
            withEnemyMoveCalculator = new WithEnemyMoveCalculator();
        }

        public byte[] GetAvailableMoves(Field field, in byte playerIndex, in byte enemyIndex)
        {
            if (withEnemyMoveMasks.TryGetValue((playerIndex, enemyIndex), out var wallMask))
            {
                var isBetweenWalls = field.GetWallsForMask(in wallMask).IsNotZero();
                if (isBetweenWalls)
                {
                    return simpleMoveCalculator.GetAvailableMoves(field, in playerIndex);
                }

                return withEnemyMoveCalculator.GetAvailableMoves(field, in playerIndex, in enemyIndex);
            }

            return simpleMoveCalculator.GetAvailableMoves(field, in playerIndex);
        }

        public (byte[] indexes, bool isSimple) GetAvailableMovesWithType(Field field, in byte playerMask,
            in byte enemyMask)
        {
            if (withEnemyMoveMasks.TryGetValue((playerMask, enemyMask), out var wallMask))
            {
                var isBetweenWalls = field.GetWallsForMask(wallMask).IsNotZero();
                if (isBetweenWalls)
                {
                    return (simpleMoveCalculator.GetAvailableMoves(field, in playerMask), true);
                }

                return (withEnemyMoveCalculator.GetAvailableMoves(field, in playerMask, in enemyMask), false);
            }

            return (simpleMoveCalculator.GetAvailableMoves(field, in playerMask), true);
        }

        public int GetRow(in byte moveIndex)
        {
            return rows[moveIndex];
        }

        // public bool TryMoveForward(Field field, in FieldMask playerMask, out FieldMask moveMask)
        // {
        //     moveMask = simpleMoveCalculator.GetAvailableMoves(field, playerMask)[0];
        //     var playerRow = GetRow(in playerMask);
        //     var moveRow = GetRow(in moveMask);
        //     return Math.Abs(playerRow - moveRow) == 1;
        // }

        public bool IsSimple(Field field, in byte playerIndex, byte moveIndex)
        {
            var simpleMoves = simpleMoveCalculator.GetAvailableMoves(field, playerIndex);
            return simpleMoves.Any(m => m == moveIndex);
        }

        public bool CanJump(Field field, Player player, out byte jump)
        {
            var moves = GetAvailableMoves(field, in player.Position, player.Enemy.Position);
            var playerRow = GetRow(in player.Position);
            var jumpMoves = moves.Where(m => Math.Abs(playerRow - GetRow(m)) == 2).ToArray();
            if (jumpMoves.Length == 0)
            {
                jump = Constants.EmptyIndex;
                return false;
            }
            jump = jumpMoves[0];
            return true;
        }

        private void CreateEnemyPlayerMasks()
        {
            for (var y = 0; y < FieldMask.BitboardSize; y += 2)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x += 2)
                {
                    var playerIndex = FieldMask.GetPlayerIndex(y, x);

                    var variants = CreateSimplePlayerMovesMaskFor(y, x);
                    foreach (var variant in variants)
                    {
                        withEnemyMoveMasks[(playerIndex, variant.enemyPosition)] = variant.wallMaks;
                    }

                    rows[playerIndex] = y / 2;
                }
            }
        }

        private List<(byte enemyPosition, FieldMask wallMaks)> CreateSimplePlayerMovesMaskFor(int y, int x)
        {
            var result = new List<(byte enemyPosition, FieldMask wallMaks)>();

            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var wallPosition = new FieldMask();

                var wallY = y + yDelta;
                var wallX = x + xDelta;

                var enemyY = y + yDelta * 2;
                var enemyX = x + xDelta * 2;

                if (FieldMask.IsInRange(enemyY, enemyX))
                {
                    var enemyIndex = FieldMask.GetPlayerIndex(enemyY, enemyX);

                    wallPosition.SetBit(wallY, wallX, true);
                    result.Add((enemyIndex, wallPosition));
                }
            }
            return result;
        }
    }
}
