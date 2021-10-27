namespace Quoridor.Controller
{
    using Model;
    using Model.Moves;
    using Model.Players;

    public class PlayerMover
    {
        private readonly Game game;
        private readonly Player player;
        private readonly IMoveParser moveParser;
        private readonly IInputReader inputReader;

        public PlayerMover(Game game, Player player, IMoveParser moveParser, IInputReader inputReader)
        {
            this.game = game;
            this.player = player;
            this.moveParser = moveParser;
            this.inputReader = inputReader;
        }

        public IMove WaitForMove()
        {
            return player.ShouldWaitForMove() ? ReadMoveFromConsole() : player.FindMove(game.Field);
        }

        private IMove ReadMoveFromConsole()
        {
            var input = inputReader.ReadInput();
            var move = moveParser.Parse(game.Field, player, input);
            return move;
        }
    }
}
