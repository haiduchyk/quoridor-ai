namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;
    using Moves;
    using Players;

    public class BothPlayersWithoutWalls : Preset
    {
        public BothPlayersWithoutWalls(MoveVariationProvider moveVariationProvider, Field field, Player player) :
            base(moveVariationProvider, field, player)
        {
        }

        public override bool TryGetNextMove(MonteNode node, out List<IMove> moves)
        {
            if (!player.HasWalls() && !player.Enemy.HasWalls())
            {
                moves = moveVariationProvider.MoveOnPath(node);
                return true;
            }
            moves = null;
            return false;
        }

        public override bool IsExpired(MonteNode node)
        {
            return false;
        }
    }
}
