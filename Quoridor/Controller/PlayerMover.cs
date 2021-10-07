namespace Quoridor.Controller
{
    using Model.Moves;
    using Model.Players;

    public class PlayerMover
    {
        private readonly IGameProvider gameProvider;
        private readonly Player player;
        private readonly ConsoleReader consoleReader;
        private readonly IMoveParser moveParser;

        public PlayerMover(IGameProvider gameProvider, Player player, IMoveParser moveParser)
        {
            this.gameProvider = gameProvider;
            this.player = player;
            this.moveParser = moveParser;
            consoleReader = new ConsoleReader();
        }

        public Move WaitForMove()
        {
            return player.ShouldWaitForMove() ? ReadMoveFromConsole() : player.MakeMove(gameProvider.Game.Field);
        }

        private Move ReadMoveFromConsole()
        {
            var input = consoleReader.WaitForInput();
            var move = moveParser.Parse(gameProvider.Game.Field, player, input);
            return move;
        }
    }
}
