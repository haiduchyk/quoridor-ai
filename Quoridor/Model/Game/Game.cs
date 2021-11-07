namespace Quoridor.Model
{
    using Players;

    public class Game
    {
        public Field Field { get; }

        public Player BluePlayer { get; }

        public Player RedPlayer { get; }

        public Game(Field field, Player bluePlayer, Player redPlayer)
        {
            Field = field;
            BluePlayer = bluePlayer;
            RedPlayer = redPlayer;
        }

        public bool HasFinished()
        {
            return BluePlayer.HasReachedFinish() || RedPlayer.HasReachedFinish();
        }
    }
}
