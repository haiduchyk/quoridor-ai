namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;
    using Moves;
    using Players;

    public class EarlyMoves : Preset
    {
        public EarlyMoves(MoveVariationProvider moveVariationProvider, Field field, Player player) :
            base(moveVariationProvider, field, player)
        {
        }

        public override bool TryGetNextMove(MonteNode node, out List<IMove> moves)
        {
            if (IsLessNthMove(node, 5) && IsLessRow(player, 3))
            {
                moves = moveVariationProvider.MoveOnPath(node);
                return true;
            }
            moves = null;
            return false;
        }

        public override bool IsExpired(MonteNode node)
        {
            return IsGreaterRow(player, 3) || IsGreaterNthMove(node, 5);
        }
    }
}
