namespace Quoridor.Controller.Game
{
    public class GameOptions
    {
        public readonly GameMode gameMode;
        public readonly StartColor color;

        public GameOptions(GameMode gameMode, StartColor color)
        {
            this.gameMode = gameMode;
            this.color = color;
        }
    }

    public enum GameMode
    {
        VersusBot,
        VersusPlayer,
        BotVersusBot,
    }

    public enum StartColor
    {
        None,
        White,
        Black
    }
}
