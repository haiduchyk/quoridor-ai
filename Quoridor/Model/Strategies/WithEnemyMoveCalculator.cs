namespace Quoridor.Model.Strategies
{
    using System;
    using System.Collections.Generic;

    public class WithEnemyMoveCalculator
    {
        private const int MoveWithEnemyBitsAmount = 7;

        // <playerPosition <enemyPosition <wallMask, possibleMoves>>>
        private Dictionary<FieldMask, Dictionary<FieldMask, Dictionary<FieldMask, FieldMask[]>>> withEnemyPlayersMoves =
            new();

        //  <playerPosition, wallMaskForThisPlayerPosition>
        private Dictionary<FieldMask, FieldMask> withEnemyPlayersMovesMasks = new();

        public FieldMask[] GetAvailableMoves(Field field, ref FieldMask playerMask, FieldMask enemyMask)
        {
            Console.WriteLine($"WithEnemyMoveCalculator");

            return new[] { playerMask };
        }
    }
}