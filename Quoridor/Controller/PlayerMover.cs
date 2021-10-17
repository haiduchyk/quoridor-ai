namespace Quoridor.Controller
{
    using Model.Moves;
    using Model.Players;

    public class PlayerMover
    {
        private readonly IGameProvider gameProvider;
        private readonly Player player;
        private readonly Player enemy;
        private readonly IMoveParser moveParser;
        private readonly IInputReader inputReader;

        public PlayerMover(IGameProvider gameProvider, Player player, Player enemy, IMoveParser moveParser,
            IInputReader inputReader)
        {
            this.gameProvider = gameProvider;
            this.player = player;
            this.enemy = enemy;
            this.moveParser = moveParser;
            this.inputReader = inputReader;
        }

        public Move WaitForMove()
        {
            return player.ShouldWaitForMove() ? ReadMoveFromConsole() : player.MakeMove(gameProvider.Game.Field, enemy);
        }

        private Move ReadMoveFromConsole()
        {
            var input = inputReader.ReadInput();
            var move = moveParser.Parse(gameProvider.Game.Field, player, enemy, input);
            return move;
        }
    }
}
