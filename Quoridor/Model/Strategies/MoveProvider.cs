namespace Quoridor.Model
{
    using System;
    using System.Collections.Generic;
    using Strategies;

    public interface IMoveProvider
    {
        FieldMask[] GetAvailableMoves(Field field, ref FieldMask playerMask, FieldMask enemyMask);
    }

    public class MoveProvider : IMoveProvider
    {
        //  <playerPosition 81, <enemyPlayerPosition, wallInBetweenPosition> ~3-4>
        private Dictionary<FieldMask, Dictionary<FieldMask, FieldMask>> withEnemyMoveMasks = new();

        private SimpleMoveCalculator simpleMoveCalculator;
        private WithEnemyMoveCalculator withEnemyMoveCalculator;

        public MoveProvider()
        {
            CreateEnemyPlayerMasks();
            simpleMoveCalculator = new SimpleMoveCalculator();
            withEnemyMoveCalculator = new WithEnemyMoveCalculator();
        }

        public FieldMask[] GetAvailableMoves(Field field, ref FieldMask playerMask, FieldMask enemyMask)
        {
            var enemyMoveMasks = withEnemyMoveMasks[playerMask];
            if (enemyMoveMasks.TryGetValue(enemyMask, out var wallMask))
            {
                var isBetweenWalls = field.GetWallsForMask(ref wallMask).IsNotZero();
                if (isBetweenWalls)
                {
                    return simpleMoveCalculator.GetAvailableMoves(field, ref playerMask);
                }
                return simpleMoveCalculator.GetAvailableMoves(field, ref playerMask);
                // return withEnemyMoveCalculator.GetAvailableMoves(field, ref playerMask, enemyMask);
            }
            
            
            return simpleMoveCalculator.GetAvailableMoves(field, ref playerMask);
        }

        private void CreateEnemyPlayerMasks()
        {
            for (var y = 0; y < FieldMask.BitboardSize; y++)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x++)
                {
                    if (y % 2 == 0 && x % 2 == 0)
                    {
                        var playerMask = new FieldMask();
                        playerMask.SetBit(y, x, true);
                        withEnemyMoveMasks[playerMask] = CreateSimplePlayerMovesMaskFor(y, x);
                    }
                }
            }
        }

        private Dictionary<FieldMask, FieldMask> CreateSimplePlayerMovesMaskFor(int y, int x)
        {
            var result = new Dictionary<FieldMask, FieldMask>();

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
                    result[enemyPosition] = wallPosition;
                }
            }

            return result;
        }
    }
}