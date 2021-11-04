namespace Quoridor.Controller.Moves
{
    using System.Linq;
    using Model;
    using Model.Moves;
    using Model.Players;

    public interface IMoveConverter
    {
        IMove ParseMove(Field field, Player player, string input);

        string GetCode(Field field, Player player, IMove move);
    }

    public class MoveConverter : IMoveConverter
    {
        private readonly IPositionConverter positionConverter;
        private readonly IMoveProvider moveProvider;
        private readonly IWallProvider wallProvider;
        private readonly ISearch search;

        public MoveConverter(IPositionConverter positionConverter, IMoveProvider moveProvider,
            IWallProvider wallProvider, ISearch search)
        {
            this.positionConverter = positionConverter;
            this.moveProvider = moveProvider;
            this.wallProvider = wallProvider;
            this.search = search;
        }

        public IMove ParseMove(Field field, Player player, string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                var masks = moveProvider.GetAvailableMoves(field, in player.Position, in player.Enemy.Position);
                return new PlayerMove(player, masks.First(), field, search, wallProvider);
            }
            var commands = input.Split(" ");
            if (commands.Length != 2)
            {
                return new DefaultMove();
            }

            var argument = commands[1];
            switch (commands[0])
            {
                case "move":
                    var cellPosition = positionConverter.TryParseCellPosition(argument);
                    if (cellPosition.HasValue)
                    {
                        return new PlayerMove(player, cellPosition.Value, field, search, wallProvider);
                    }
                    break;

                case "jump":
                    cellPosition = positionConverter.TryParseCellPosition(argument);
                    if (cellPosition.HasValue)
                    {
                        return new PlayerMove(player, cellPosition.Value, field, search, wallProvider);
                    }
                    break;

                case "wall":
                    var wallPosition = positionConverter.TryParseWallPosition(argument);
                    if (wallPosition.HasValue)
                    {
                        return new WallMove(field, player, search, wallProvider, wallPosition.Value);
                    }
                    break;
            }

            return new DefaultMove();
        }

        public string GetCode(Field field, Player player, IMove move)
        {
            switch (move)
            {
                case PlayerMove step when moveProvider.IsSimple(field, in player.Position, step.Id):
                    return $"move {positionConverter.CellPositionToCode(step.Id)}";
                case PlayerMove step when !moveProvider.IsSimple(field, in player.Position, step.Id):
                    return $"jump {positionConverter.CellPositionToCode(step.Id)}";
                case WallMove wall:
                    return $"wall {positionConverter.WallPositionToCode(wall.Id)}";
            }

            return "unknown";
        }
    }
}
