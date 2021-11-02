namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;
    using Moves;
    using Players;

    public class StandardOpeningPreset : Preset
    {
        public StandardOpeningPreset(MoveVariationProvider moveVariationProvider, Field field, Player player) :
            base(moveVariationProvider, field, player)
        {
        }

        public override bool TryGetNextMove(MonteNode node, out List<IMove> moves)
        {
            if (CanMoveForward(node, 0, out var move))
            {
                moves = moveVariationProvider.FromMove(move);
                return true;
            }
            if (CanMoveForward(node, 1, out move))
            {
                moves = moveVariationProvider.FromMove(move);
                return true;
            }
            if (CanMoveForward(node, 2, out move))
            {
                moves = moveVariationProvider.FromMove(move);
                return true;
            }
            if (CanPlaceWallBehind(node, out var wall))
            {
                moves = moveVariationProvider.FromWall(wall);
                return true;
            }
            moves = null;
            return false;
        }

        private bool CanMoveForward(MonteNode node, int n, out byte move)
        {
            move = Constants.EmptyIndex;
            return IsNthMove(node, n) && moveVariationProvider.TryMoveForward(field, player, out move);
        }

        private bool CanPlaceWallBehind(MonteNode node, out byte wall)
        {
            wall = Constants.EmptyIndex;
            return IsNthMove(node, 3) && IsOnRow(player, 3) &&
                   moveVariationProvider.TryGetWallBehind(field, player, out wall);
        }

        public override bool IsExpired(MonteNode node)
        {
            return IsGreaterNthMove(node, 3);
        }
    }
}
