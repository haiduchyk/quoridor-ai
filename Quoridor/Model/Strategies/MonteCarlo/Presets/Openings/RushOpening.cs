namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;
    using Moves;
    using Players;

    public class RushOpening : Preset
    {
        // Sidewalls
        // 55, 73

        private readonly Dictionary<byte, byte> sidewall = new()
        {
            { PlayerConstants.EndBlueDownIndexIncluding, 55 },
            { PlayerConstants.EndRedDownIndexIncluding, 73 },
        };

        public RushOpening(MoveVariationProvider moveVariationProvider, Field field, Player player) :
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
            if (IsNthMove(node, 3) && CanPlaceSidewall(out var wall))
            {
                moves = moveVariationProvider.FromWall(wall);
                return true;
            }
            moves = null;
            return false;
        }

        private bool CanPlaceSidewall(out byte wall)
        {
            wall = sidewall[player.EndDownIndex];
            return field.CanPlace(wall);
        }

        public override bool IsExpired(MonteNode node)
        {
            return IsGreaterNthMove(node, 3);
        }
    }
}
