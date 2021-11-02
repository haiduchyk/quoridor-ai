namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;
    using Moves;
    using Players;

    public class MonteCarloMoveProvider
    {
        private readonly List<IPreset> presets;

        public MonteCarloMoveProvider(MoveVariationProvider moveVariationProvider, Field field, Player player)
        {
            presets = new List<IPreset>
            {
                new StandardOpeningPreset(moveVariationProvider, field, player),
                new EarlyMoves(moveVariationProvider, field, player),
                new EmptyPreset(moveVariationProvider, field, player),
                new HasNoWalls(moveVariationProvider, field, player),
                new EnemyWithoutWalls(moveVariationProvider, field, player),
                new BothPlayersWithoutWalls(moveVariationProvider, field, player),
                new AllMoves(moveVariationProvider, field, player),
            };
        }

        public List<IMove> FindMoves(MonteNode node)
        {
            foreach (var preset in presets)
            {
                if (!preset.IsExpired(node) && preset.TryGetNextMove(node, out var moves))
                {
                    return moves;
                }
            }
            return new List<IMove>();
        }
    }
}
