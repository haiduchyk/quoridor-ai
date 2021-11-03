namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;
    using Moves;
    using Players;

    public class GapOpening : StandardOpening
    {
        // Walls behind second player
        // 38, 40
        // 38 -> 44
        // 40 -> 34
        // Walls behind first player
        // 86, 88
        // 86 -> 92
        // 88 -> 82

        private readonly Dictionary<byte, List<(byte placed, byte gap)>> gaps = new()
        {
            { PlayerConstants.EndBlueDownIndexIncluding, new List<(byte placed, byte gap)> { (38, 44), (40, 34) } },
            { PlayerConstants.EndRedDownIndexIncluding, new List<(byte placed, byte gap)> { (86, 92), (88, 82) } },
        };

        public GapOpening(MoveVariationProvider moveVariationProvider, Field field, Player player) :
            base(moveVariationProvider, field, player)
        {
        }

        public override bool TryGetNextMove(MonteNode node, out List<IMove> moves)
        {
            if (base.TryGetNextMove(node, out moves))
            {
                return true;
            }
            if (IsNthMove(node, 4) && IsWallPlacedBehind() && CanWallBePlaceGap(out var wall))
            {
                moves = moveVariationProvider.FromWall(wall);
                return true;
            }
            moves = null;
            return false;
        }

        private bool IsWallPlacedBehind()
        {
            return (field.HasWall(38) || field.HasWall(40)) &&
                   (field.HasWall(86) || field.HasWall(88));
        }

        private bool CanWallBePlaceGap(out byte wall)
        {
            var possibleGaps = gaps[player.EndDownIndex];
            foreach (var (placed, gap) in possibleGaps)
            {
                if (field.HasWall(placed) && field.CanPlace(gap))
                {
                    wall = gap;
                    return true;
                }
            }
            wall = Constants.EmptyIndex;
            return false;
        }

        public override bool IsExpired(MonteNode node)
        {
            return IsGreaterNthMove(node, 4);
        }
    }
}
