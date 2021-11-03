namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;
    using Moves;
    using Players;

    public class SidewallOpening : Preset
    {
        // Sidewalls
        // 23, 105
        // Counters
        // 23 -> 20
        // 105 -> 106
        // Blocking walls
        // 23 -> 40, 44, 31
        // 105 -> 86, 82, 97

        private readonly Dictionary<byte, byte> sidewall = new()
        {
            { PlayerConstants.EndBlueDownIndexIncluding, 23 },
            { PlayerConstants.EndRedDownIndexIncluding, 105 },
        };

        private readonly Dictionary<byte, List<byte>> blockingWalls = new()
        {
            { PlayerConstants.EndBlueDownIndexIncluding, new List<byte>() { 40, 44, 31 } },
            { PlayerConstants.EndRedDownIndexIncluding, new List<byte>() { 86, 82, 97 } },
        };

        public SidewallOpening(MoveVariationProvider moveVariationProvider, Field field, Player player) :
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
            if (IsNthMove(node, 1) && IsOnRow(player.Enemy, 1) && CanPlaceSidewall(out var wall))
            {
                moves = moveVariationProvider.FromWall(wall);
                return true;
            }
            if (IsNthMove(node, 2) && CanPlaceBlockingWall(node, 0, out wall))
            {
                moves = moveVariationProvider.FromWall(wall);
                return true;
            }
            if (CanMoveForward(node, 3, out move))
            {
                moves = moveVariationProvider.FromMove(move);
                return true;
            }
            if (IsNthMove(node, 4) && CanPlaceBlockingWall(node, 1, out wall))
            {
                moves = moveVariationProvider.FromWall(wall);
                return true;
            }
            if (IsNthMove(node, 5) && CanPlaceBlockingWall(node, 2, out wall))
            {
                moves = moveVariationProvider.FromWall(wall);
                return true;
            }
            if (CanMoveForward(node, 6, out move))
            {
                moves = moveVariationProvider.FromMove(move);
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

        private bool CanPlaceSidewall(out byte wall)
        {
            wall = sidewall[player.EndDownIndex];
            return field.CanPlace(wall);
        }

        private bool CanPlaceBlockingWall(MonteNode node, int n, out byte wall)
        {
            wall = blockingWalls[player.EndDownIndex][n];
            return field.CanPlace(wall);
        }

        public override bool IsExpired(MonteNode node)
        {
            return !player.IsFirstPlayer() || IsGreaterNthMove(node, 6);
        }
    }
}
