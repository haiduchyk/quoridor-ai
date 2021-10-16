namespace Quoridor.Controller
{
    using Model.Moves;
    using Model.Players;

    public class PlayerMover
    {
        private readonly IGameProvider gameProvider;
        private readonly Player player;
        private readonly Player enemy;
        private readonly ConsoleReader consoleReader;
        private readonly IMoveParser moveParser;

        public PlayerMover(IGameProvider gameProvider, Player player, Player enemy, IMoveParser moveParser)
        {
            this.gameProvider = gameProvider;
            this.player = player;
            this.enemy = enemy;
            this.moveParser = moveParser;
            consoleReader = new ConsoleReader();
        }

        public Move WaitForMove()
        {
            return player.ShouldWaitForMove() ? ReadMoveFromConsole() : player.MakeMove(gameProvider.Game.Field, enemy);
        }

        private Move ReadMoveFromConsole()
        {
            var input = consoleReader.WaitForInput();
            var move = moveParser.Parse(gameProvider.Game.Field, player, enemy, input);
            return move;
        }
    }
}
