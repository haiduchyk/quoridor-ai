namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;
    using Moves;
    using Players;

    public class ShatranjOpening : Preset
    {
        // Sidewalls
        // 121, 7

        private readonly Dictionary<byte, byte> sidewall = new()
        {
            { PlayerConstants.EndBlueDownIndexIncluding, 121 },
            { PlayerConstants.EndRedDownIndexIncluding, 7 },
        };

        public ShatranjOpening(MoveVariationProvider moveVariationProvider, Field field, Player player) :
            base(moveVariationProvider, field, player)
        {
        }

        public override bool TryGetNextMove(MonteNode node, out List<IMove> moves)
        {
            if (IsNthMove(node, 0) && CanPlaceSidewall(out var wall))
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
            return IsGreaterNthMove(node, 0);
        }
    }
}
