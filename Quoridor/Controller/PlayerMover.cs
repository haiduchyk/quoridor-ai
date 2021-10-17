namespace Quoridor.Controller
{
    using Model;
    using Model.Moves;
    using Model.Players;

    public class PlayerMover
    {
        private readonly Game game;
        private readonly Player player;
        private readonly Player enemy;
        private readonly IMoveParser moveParser;
        private readonly IInputReader inputReader;

        public PlayerMover(Game game, Player player, IMoveParser moveParser, IInputReader inputReader)
        {
            this.game = game;
            this.player = player;
            enemy = GetEnemy();
            this.moveParser = moveParser;
            this.inputReader = inputReader;
        }

        private Player GetEnemy()
        {
            return player == game.BluePlayer ? game.RedPlayer : game.BluePlayer;
        }

        public IMove WaitForMove()
        {
            return player.ShouldWaitForMove() ? ReadMoveFromConsole() : player.MakeMove(game.Field, enemy);
        }

        private IMove ReadMoveFromConsole()
        {
            var input = inputReader.ReadInput();
            var move = moveParser.Parse(game.Field, player, enemy, input);
            return move;
        }
    }
}
