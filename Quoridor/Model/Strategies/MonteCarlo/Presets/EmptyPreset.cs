namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;
    using Moves;
    using Players;

    public class EmptyPreset : Preset
    {
        public EmptyPreset(MoveVariationProvider moveVariationProvider, Field field, Player player) :
            base(moveVariationProvider, field, player)
        {
        }

        public override bool TryGetNextMove(MonteNode node, out List<IMove> moves)
        {
            if (TurnPlayer(node).HasReachedFinish())
            {
                moves = new List<IMove>();
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
