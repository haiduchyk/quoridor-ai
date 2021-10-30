namespace Quoridor.Model
{
    using System;
    using System.Collections.Generic;
    using Strategies;

    public interface IMoveProvider
    {
        FieldMask[] GetAvailableMoves(Field field, in FieldMask playerMask, in FieldMask enemyMask);

        (FieldMask[] masks, bool isSimple) GetAvailableMovesWithType(Field field, in FieldMask playerMask, in FieldMask enemyMask);

        int GetRow(in FieldMask moveMask);

        bool TryMoveForward(Field field, in FieldMask playerMask, out FieldMask moveMask);
    }

    public class MoveProvider : IMoveProvider
    {
        //  <playerPosition 81, <enemyPosition ~3-4, wallInBetweenPosition>>
        private Dictionary<(FieldMask playerPosition, FieldMask enemyPosition), FieldMask> withEnemyMoveMasks =
            new();

        private readonly Dictionary<FieldMask, int> rows = new();

        private SimpleMoveCalculator simpleMoveCalculator;
        private WithEnemyMoveCalculator withEnemyMoveCalculator;

        public MoveProvider()
        {
            CreateEnemyPlayerMasks();
            simpleMoveCalculator = new SimpleMoveCalculator();
            withEnemyMoveCalculator = new WithEnemyMoveCalculator();
        }

        public FieldMask[] GetAvailableMoves(Field field, in FieldMask playerMask, in FieldMask enemyMask)
        {
            if (withEnemyMoveMasks.TryGetValue((playerMask, enemyMask), out var wallMask))
            {
                var isBetweenWalls = field.GetWallsForMask(in wallMask).IsNotZero();
                if (isBetweenWalls)
                {
                    return simpleMoveCalculator.GetAvailableMoves(field, in playerMask);
                }

                return withEnemyMoveCalculator.GetAvailableMoves(field, in playerMask, in enemyMask);
            }

            return simpleMoveCalculator.GetAvailableMoves(field, in playerMask);
        }

        public (FieldMask[] masks, bool isSimple) GetAvailableMovesWithType(Field field, in FieldMask playerMask,
            in FieldMask enemyMask)
        {
            if (withEnemyMoveMasks.TryGetValue((playerMask, enemyMask), out var wallMask))
            {
                var isBetweenWalls = field.GetWallsForMask(wallMask).IsNotZero();
                if (isBetweenWalls)
                {
                    return (simpleMoveCalculator.GetAvailableMoves(field, playerMask), true);
                }

                return (withEnemyMoveCalculator.GetAvailableMoves(field, playerMask, enemyMask), false);
            }

            return (simpleMoveCalculator.GetAvailableMoves(field, playerMask), true);
        }

        public int GetRow(in FieldMask moveMask)
        {
            return rows[moveMask];
        }

        public bool TryMoveForward(Field field, in FieldMask playerMask, out FieldMask moveMask)
        {
            moveMask = simpleMoveCalculator.GetAvailableMoves(field, in playerMask)[0];
            var playerRow = GetRow(in playerMask);
            var moveRow = GetRow(in moveMask);
            return Math.Abs(playerRow - moveRow) == 1;
        }

        private void CreateEnemyPlayerMasks()
        {
            for (var y = 0; y < FieldMask.BitboardSize; y += 2)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x += 2)
                {
                    var playerMask = new FieldMask();
                    playerMask.SetBit(y, x, true);
                    var variants = CreateSimplePlayerMovesMaskFor(y, x);
                    foreach (var variant in variants)
                    {
                        withEnemyMoveMasks[(playerMask, variant.enemyPosition)] = variant.wallMaks;
                    }
                    rows[playerMask] = y / 2;
                }
            }
        }

        private List<(FieldMask enemyPosition, FieldMask wallMaks)> CreateSimplePlayerMovesMaskFor(int y, int x)
        {
            var result = new List<(FieldMask enemyPosition, FieldMask wallMaks)>();

            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var enemyPosition = new FieldMask();
                var wallPosition = new FieldMask();

                var wallY = y + yDelta;
                var wallX = x + xDelta;

                var enemyY = y + yDelta * 2;
                var enemyX = x + xDelta * 2;

                if (FieldMask.IsInRange(enemyY, enemyX))
                {
                    enemyPosition.SetBit(enemyY, enemyX, true);
                    wallPosition.SetBit(wallY, wallX, true);
                    result.Add((enemyPosition, wallPosition));
                }
            }

            return result;
        }
    }
}
