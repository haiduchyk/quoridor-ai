namespace Quoridor.Model.Strategies
{
    using Players;

    public class MonteThread
    {
        public MonteNode node;
        private int i;
        public Player montePlayer;
        public Player monteEnemy;
        public Field monteField;
        public HeuristicStrategy strategy;

        public int Simulate()
        {
            var firstPlayer = node.IsPlayerMove ? monteEnemy : montePlayer;
            var secondPlayer = node.IsPlayerMove ? montePlayer : monteEnemy;
            var moveCount = 0;

            var hasWalls = secondPlayer.HasWalls();
            while (!firstPlayer.HasReachedFinish() && !secondPlayer.HasReachedFinish() && moveCount < 1000)
            {
                var isPlayer = moveCount % 2 == 0;
                var player = isPlayer ? firstPlayer : secondPlayer;
                var move = strategy.FindMove(monteField, player, null, !isPlayer && hasWalls);
                if (move.IsValid())
                {
                    move.ExecuteForSimulation();
                    moveCount++;
                }
            }

            return montePlayer.HasReachedFinish() ? 1 : 0;
        }
    }
}