namespace Quoridor.Model
{
    using Players;

    public class GameResult
    {
        public Player Winner { get; }

        public int MoveCount { get; }

        public GameResult(Player winner, int moveCount)
        {
            Winner = winner;
            MoveCount = moveCount;
        }
    }
}
