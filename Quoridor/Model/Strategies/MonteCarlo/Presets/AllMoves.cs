namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;
    using Moves;
    using Players;

    public class AllMoves : Preset
    {
        public AllMoves(MoveVariationProvider moveVariationProvider, Field field, Player player) :
            base(moveVariationProvider, field, player)
        {
        }

        public override bool TryGetNextMove(MonteNode node, out List<IMove> moves)
        {
            moves = moveVariationProvider.AllMoves(node);
            return true;
        }

        public override bool IsExpired(MonteNode node)
        {
            return false;
        }
    }
}
