namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;
    using Moves;
    using Players;

    public class EnemyWithoutWalls : Preset
    {
        public EnemyWithoutWalls(MoveVariationProvider moveVariationProvider, Field field, Player player) :
            base(moveVariationProvider, field, player)
        {
        }

        public override bool TryGetNextMove(MonteNode node, out List<IMove> moves)
        {
            if (node.IsPlayerMove && !player.Enemy.HasWalls())
            {
                moves = moveVariationProvider.ShiftsWithBlockingWalls(node);
                return true;
            }
            moves = null;
            return false;
        }

        public override bool IsExpired(MonteNode node)
        {
            return !player.HasWalls();
        }
    }
}
