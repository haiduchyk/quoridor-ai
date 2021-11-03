namespace Quoridor.Model.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moves;
    using Players;

    public class MonteCarloMoveProvider
    {
        private readonly List<IPreset> openings;
        private readonly List<IPreset> presets;

        private IPreset currentOpening;
        private Random random;

        public MonteCarloMoveProvider(MoveVariationProvider moveVariationProvider, Field field, Player player)
        {
            random = new Random();
            openings = new List<IPreset>()
            {
                // new StandardOpening(moveVariationProvider, field, player),
                // new GapOpening(moveVariationProvider, field, player),
                // new SidewallOpening(moveVariationProvider, field, player),
                // new RushOpening(moveVariationProvider, field, player),
                new ShatranjOpening(moveVariationProvider, field, player),
            };
            presets = new List<IPreset>
            {
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
            if (currentOpening == null && HasOpening(node))
            {
                currentOpening = ChooseOpening(node);
            }
            if (TryGetMoveFromOpening(node, out var moves))
            {
                return moves;
            }
            foreach (var preset in presets)
            {
                if (!preset.IsExpired(node) && preset.TryGetNextMove(node, out moves))
                {
                    return moves;
                }
            }
            return new List<IMove>();
        }

        private bool HasOpening(MonteNode node)
        {
            return openings.Any(o => !o.IsExpired(node));
        }

        private IPreset ChooseOpening(MonteNode node)
        {
            var availableOpenings = openings.Where(o => !o.IsExpired(node)).ToArray();
            return availableOpenings[random.Next(0, availableOpenings.Length)];
        }

        private bool TryGetMoveFromOpening(MonteNode node, out List<IMove> moves)
        {
            moves = null;
            return currentOpening != null && !currentOpening.IsExpired(node) &&
                   currentOpening.TryGetNextMove(node, out moves);
        }
    }
}
