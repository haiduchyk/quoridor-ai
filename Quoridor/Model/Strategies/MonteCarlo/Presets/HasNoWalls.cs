namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;
    using Moves;
    using Players;

    public class HasNoWalls : Preset
    {
        public HasNoWalls(MoveVariationProvider moveVariationProvider, Field field, Player player) :
            base(moveVariationProvider, field, player)
        {
        }

        public override bool TryGetNextMove(MonteNode node, out List<IMove> moves)
        {
            if (!TurnPlayer(node).HasWalls())
            {
                moves = moveVariationProvider.Shifts(node);
                return true;
            }
            moves = null;
            return false;
        }

        public override bool IsExpired(MonteNode node)
        {
            return !player.Enemy.HasWalls();
        }
    }
}
